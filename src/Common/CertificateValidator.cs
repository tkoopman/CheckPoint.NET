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
        /// <param name="expectedCertificateHash">The expected certificate hash if pinning.</param>
        public CertificateValidator(CertificateValidation certificateValidation, string expectedCertificateHash)
        {
            var certValid = certificateValidation;
            if (certificateValidation.HasFlag(CertificateValidation.Auto))
                certValid = (string.IsNullOrEmpty(expectedCertificateHash)) ? CertificateValidation.ValidCertificate : CertificateValidation.CertificatePinning;

            CertificateValidation = certValid;
            ExpectedCertificateHash = expectedCertificateHash;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the server SSL certificate hash.
        /// </summary>
        /// <value>Server SSL certificate hash.</value>
        public string CertificateHash { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether SSL certificate should be valid.
        /// </summary>
        /// <value><c>true</c> if certificate validation enabled otherwise, <c>false</c>.</value>
        public CertificateValidation CertificateValidation { get; }

        /// <summary>
        /// Gets the expected server SSL certificate hash.
        /// </summary>
        /// <value>Expected Server SSL certificate hash.</value>
        public string ExpectedCertificateHash { get; }

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
        /// Validates the server certificate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns><c>true</c> if server's certificate passes validation; otherwise <c>false</c></returns>
        public bool ValidateServerCertficate(
                    object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
#if NETSTANDARD1_6
            CertificateHash = BitConverter.ToString(certificate?.GetCertHash()).Replace("-", "");
#else
            CertificateHash = certificate?.GetCertHashString();
#endif

            if (CertificateValidation.HasFlag(CertificateValidation.ValidCertificate) && sslPolicyErrors != SslPolicyErrors.None)
                return false;
            if (CertificateValidation.HasFlag(CertificateValidation.CertificatePinning) && ExpectedCertificateHash != CertificateHash)
                return false;

            return true;
        }

        #endregion Methods
    }
}