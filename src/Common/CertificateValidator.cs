// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Used to validate the server's SSL certificate.
    /// </summary>
    public class CertificateValidator
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateValidator" /> class.
        /// </summary>
        /// <param name="certificateValidation">The certificate validation method(s) to use.</param>
        /// <param name="hostName">Host name for validation.</param>
        /// <param name="expectedCertificateHash">The expected certificate hash if pinning.</param>
        public CertificateValidator(CertificateValidation certificateValidation, string hostName, string expectedCertificateHash)
        {
            AddHostCertificateValidation(certificateValidation, hostName, expectedCertificateHash);
        }

        #endregion Constructors

        #region Properties

        internal readonly ConcurrentDictionary<string, Tuple<CertificateValidation, string>> ValidateHosts = new ConcurrentDictionary<string, Tuple<CertificateValidation, string>>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the server certificate hash.
        /// </summary>
        /// <param name="url">The URL to get server certificate from.</param>
        /// <returns>Tuple where Item1 is the certificate subject, and Item2 is the Hash.</returns>
        public static Tuple<string, string> GetServerCertificateHash(string url)
        {
            X509Certificate2 serverCert = null;
#if NET45
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            response.Close();
            serverCert = new X509Certificate2(request.ServicePoint.Certificate);
#else
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    serverCert = new X509Certificate2(cert.RawData);
                    return true;
                }
            };
            using (var httpClient = new HttpClient(handler))
            {
                using (var r = httpClient.GetAsync(url).GetAwaiter().GetResult()) { }
            }
#endif
#if NETSTANDARD1_6
            string CertificateHash = BitConverter.ToString(serverCert?.GetCertHash()).Replace("-", "");
#else
            string CertificateHash = serverCert?.GetCertHashString();
#endif
            return new Tuple<string, string>(serverCert?.Subject, CertificateHash);
        }

        /// <summary>
        /// Adds the host certificate validation.
        /// </summary>
        /// <param name="certificateValidation">The certificate validation method(s) to use.</param>
        /// <param name="hostName">Host name for validation.</param>
        /// <param name="expectedCertificateHash">The expected certificate hash if pinning.</param>
        /// <exception cref="Exception">
        /// Cannot use different certificate validation methods under .NET 4.5. or Cannot have two
        /// differently pinned certificates for the same host under .NET 4.5. or Failed to add new
        /// host name to CertificateValidator
        /// </exception>
        public void AddHostCertificateValidation(CertificateValidation certificateValidation, string hostName, string expectedCertificateHash)
        {
            var certValid = certificateValidation;
            if (certificateValidation.HasFlag(CertificateValidation.Auto))
                certValid = (string.IsNullOrEmpty(expectedCertificateHash)) ? CertificateValidation.ValidCertificate : CertificateValidation.CertificatePinning;

            if (ValidateHosts.TryGetValue(hostName, out var value))
            {
                if (value.Item1 != certValid)
                    throw new Exception("Cannot use different certificate validation methods under .NET 4.5.");
                if (value.Item1.HasFlag(CertificateValidation.CertificatePinning) && value.Item2 != expectedCertificateHash)
                    throw new Exception("Cannot have two differently pinned certificates for the same host under .NET 4.5.");
            }
            else
            {
                if (!ValidateHosts.TryAdd(hostName, new Tuple<CertificateValidation, string>(certValid, expectedCertificateHash)))
                    throw new Exception("Failed to add new host name to CertificateValidator");
            }
        }

        /// <summary>
        /// Validates the server certificate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns><c>true</c> if server's certificate passes validation; otherwise <c>false</c></returns>
        public bool ValidateServerCertificate(
                    object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            string hostName;

            if (sender is HttpRequestMessage r1)
                hostName = r1.RequestUri.Host;
#if (NETSTANDARD1_6 == false)
            else if (sender is HttpWebRequest r2)
                hostName = r2.Address.Host;
#endif
            else return sslPolicyErrors == SslPolicyErrors.None;

            if (!ValidateHosts.TryGetValue(hostName, out var value) || value == null)
                return sslPolicyErrors == SslPolicyErrors.None;

#if NETSTANDARD1_6
            string CertificateHash = BitConverter.ToString(certificate?.GetCertHash()).Replace("-", "");
#else
            string CertificateHash = certificate?.GetCertHashString();
#endif

            if (value.Item1.HasFlag(CertificateValidation.ValidCertificate) && sslPolicyErrors != SslPolicyErrors.None)
                return false;
            if (value.Item1.HasFlag(CertificateValidation.CertificatePinning) && value.Item2 != CertificateHash)
                return false;

            return true;
        }

        #endregion Methods
    }
}