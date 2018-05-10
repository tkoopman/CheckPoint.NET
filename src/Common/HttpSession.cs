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

using Koopman.CheckPoint.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Base HTTP Session Class
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public abstract class HttpSession : IDisposable
    {
        #region Fields

        private static readonly Random random = new Random();
        private readonly SemaphoreSlim HttpSemaphore;
        private HttpClient _httpClient = null;
        private bool _isDisposed = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSession" /> class.
        /// </summary>
        /// <param name="url">The base URL.</param>
        /// <param name="certificateValidation">The certificate validation method(s) to use.</param>
        /// <param name="certificateHash">The certificate hash if pinning certificate.</param>
        /// <param name="debugWriter">The debug writer.</param>
        /// <param name="indentJson">if set to <c>true</c> json sent to server will be indented.</param>
        /// <param name="maxConnections">The maximum connections to server.</param>
        protected HttpSession(string url, CertificateValidation certificateValidation, string certificateHash, TextWriter debugWriter, bool indentJson, int maxConnections)
        {
            var uri = new Uri(url);
            HostName = uri.Host;
            CertificateHash = certificateHash;
            CertificateValidation = certificateValidation;
            DebugWriter = debugWriter;
            IndentJson = indentJson;
            MaxConnections = maxConnections;
            URL = url;
            HttpSemaphore = new SemaphoreSlim(maxConnections + 1);
        }

        #endregion Constructors

        private readonly string CertificateHash;
        private readonly CertificateValidation CertificateValidation;
        private readonly string HostName;

        #region Properties

        /// <summary>
        /// Gets or sets the debug writer. All API posts and responses will be sent to this writer.
        /// They are sent in the RAW JSON format as sent and received to/from the server.
        /// </summary>
        /// <value>The text writer to send all debug output to.</value>
        public TextWriter DebugWriter { get; set; }

        /// <summary>
        /// Gets a value indicating whether JSON data sent to server should be indented. Useful for debugging.
        /// </summary>
        /// <value><c>true</c> to indent json; otherwise, <c>false</c>.</value>
        public bool IndentJson { get; }

        /// <summary>
        /// Gets the max number of connections to management server allowed.
        /// </summary>
        /// <value>The maximum connections.</value>
        public int MaxConnections { get; }

        /// <summary>
        /// URL that was used to reach the API server.
        /// </summary>
        /// <value>The URL.</value>
        [JsonProperty(PropertyName = "url")]
        public string URL { get; protected set; }

        /// <summary>
        /// Gets the JSON formatting setting.
        /// </summary>
        /// <value>The JSON formatting.</value>
        protected internal Formatting JsonFormatting => (IndentJson) ? Formatting.Indented : Formatting.None;

        /// <summary>
        /// Gets the HTTP headers to add to each request made by <see cref="PostAsync(string, string, CancellationToken)" />.
        /// </summary>
        /// <value>The HTTP headers.</value>
        protected Dictionary<string, string> HttpHeaders { get; } = new Dictionary<string, string>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_httpClient != null)
            {
                ((IDisposable)_httpClient).Dispose();
                _httpClient = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Async posts the specified command with the JSON data supplied. This can be used to send
        /// any commands this .NET package doesn't implement yet.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="json">The json.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the JSON
        /// Response Data
        /// </returns>
        public async System.Threading.Tasks.Task<string> PostAsync(string command, string json, CancellationToken cancellationToken = default)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Session", "This session has already been disposed!");

            await HttpSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                string result = null;

                string debugIP = WriteDebug(command, json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                foreach (var h in HttpHeaders)
                    content.Headers.Add(h.Key, h.Value);

                var response = await GetHttpClient().PostAsync(command, content, cancellationToken).ConfigureAwait(false);

                try
                {
                    result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    WriteDebug(debugIP, response.StatusCode, result);

                    if (!response.IsSuccessStatusCode)
                        throw CheckPointError.CreateException(result, response.StatusCode);
                }
                finally
                {
                    response.Dispose();
                }

                return result;
            }
            finally
            {
                HttpSemaphore.Release();
            }
        }

        internal void WriteDebug(string message)
        {
            if (DebugWriter == null) return;
            DebugWriter?.WriteLine(message);
            DebugWriter?.Flush();
        }

        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private HttpClient GetHttpClient()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Session", "This session has already been disposed!");

            if (_httpClient == null)
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

#if NET45
                if (ServicePointManager.ServerCertificateValidationCallback?.Target is CertificateValidator validator)
                {
                    bool valid = validator.ValidateHosts.TryGetValue(HostName, out var value);

                    if (valid)
                    {
                        if (value.Item1 != CertificateValidation)
                            throw new Exception("Cannot use different certificate validation methods under .NET 4.5.");
                        if (value.Item1.HasFlag(CertificateValidation.CertificatePinning) && value.Item2 != CertificateHash)
                            throw new Exception("Cannot have two differently pinned certificates under .NET 4.5.");
                    } else
                    {
                        if (!validator.ValidateHosts.TryAdd(HostName, new Tuple<CertificateValidation, string>(CertificateValidation, CertificateHash)))
                            throw new Exception("Failed to add new host name to CertificateValidator");
                    }
                }
                else
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(new CertificateValidator(CertificateValidation, HostName, CertificateHash).ValidateServerCertificate);
                }
                ServicePointManager.DefaultConnectionLimit = MaxConnections;
#else
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => new CertificateValidator(CertificateValidation, HostName, CertificateHash).ValidateServerCertificate(message, cert, chain, errors);
                handler.MaxConnectionsPerServer = MaxConnections;
#endif

                _httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri($"{URL}/")
                };
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            return _httpClient;
        }

        private string WriteDebug(string command, string data)
        {
            if (DebugWriter == null) return null;
            string id = RandomString(8);
            WriteDebug($@"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")} Start Post ID:{id} Command: {command}
{data}
");
            return id;
        }

        private void WriteDebug(string id, HttpStatusCode code, string data)
        {
            if (DebugWriter == null) return;
            WriteDebug($@"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")} Start Response ID:{id} Code: {code}
{data}
");
        }

        #endregion Methods
    }
}