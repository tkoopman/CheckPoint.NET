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

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// An active management session. This class handles all communications to the Management server.
    /// </summary>
    /// <example>
    /// <code>
    /// var session = Session.Login(
    ///     managementServer: "192.168.1.1",
    ///     userName: "admin",
    ///     password: "***",
    ///     certificateValidation: false
    /// );
    /// </code>
    /// </example>
    /// <seealso cref="System.IDisposable" />
    public partial class Session : IDisposable
    {
        #region Fields

        private static Random random = new Random();
        private HttpClient _httpClient = null;
        private bool _isDisposed = false;
        private SemaphoreSlim HttpSemaphore;

        #endregion Fields

        #region Constructors

        private Session(CertificateValidation certificateValidation, DetailLevelActions detailLevelAction, bool indentJson, int maxConnections, string certificateHash)
        {
            CertificateValidator = new CertificateValidator(certificateValidation, certificateHash);
            DetailLevelAction = detailLevelAction;
            IndentJson = indentJson;
            MaxConnections = maxConnections;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// API Server version.
        /// </summary>
        /// <value>The API server version.</value>
        [JsonProperty(PropertyName = "api-server-version")]
        public string APIServerVersion { get; private set; }

        /// <summary>
        /// Gets or sets the debug writer. All API posts and responses will be sent to this writer.
        /// They are sent in the RAW JSON format as sent and received to/from the server.
        /// </summary>
        /// <value>The text writer to send all debug output to.</value>
        public TextWriter DebugWriter { get; set; }

        /// <summary>
        /// Gets the action to be taken when current detail level is too low.
        /// </summary>
        /// <value>The detail level action to take.</value>
        public DetailLevelActions DetailLevelAction { get; }

        /// <summary>
        /// Information about the available disk space on the management server.
        /// </summary>
        /// <value>The disk space message.</value>
        [JsonProperty(PropertyName = "disk-space-message")]
        public string DiskSpaceMessage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether JSON data sent to server should be indented. Useful for debugging.
        /// </summary>
        /// <value><c>true</c> to indent json; otherwise, <c>false</c>.</value>
        public bool IndentJson { get; }

        /// <summary>
        /// Timestamp when administrator last accessed the management server.
        /// </summary>
        /// <value>The last login was at.</value>
        [JsonProperty(PropertyName = "last-login-was-at")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime LastLoginWasAt { get; private set; }

        /// <summary>
        /// Gets the login message.
        /// </summary>
        /// <value>The login message.</value>
        [JsonProperty(PropertyName = "login-message")]
        public LoginMessage LoginMessage { get; private set; }

        /// <summary>
        /// Gets the max number of connections to management server allowed.
        /// </summary>
        /// <value>The maximum connections.</value>
        public int MaxConnections { get; }

        /// <summary>
        /// Session is read only status.
        /// </summary>
        /// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "read-only")]
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// Session expiration timeout in seconds.
        /// </summary>
        /// <value>The session timeout.</value>
        [JsonProperty(PropertyName = "session-timeout")]
        public int SessionTimeout { get; private set; }

        /// <summary>
        /// Session unique identifier.
        /// </summary>
        /// <value>The SID.</value>
        [JsonProperty(PropertyName = "sid")]
        public string SID { get; private set; }

        /// <summary>
        /// True if this management server is in standby mode.
        /// </summary>
        /// <value><c>true</c> if standby; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "standby")]
        public bool Standby { get; private set; }

        /// <summary>
        /// Session object unique identifier.
        /// </summary>
        /// <value>The UID.</value>
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; private set; }

        /// <summary>
        /// URL that was used to reach the API server.
        /// </summary>
        /// <value>The URL.</value>
        [JsonProperty(PropertyName = "url")]
        public string URL { get; private set; }

        /// <summary>
        /// Gets the JSON formatting setting.
        /// </summary>
        /// <value>The JSON formatting.</value>
        protected internal Formatting JsonFormatting => (IndentJson) ? Formatting.Indented : Formatting.None;

        private CertificateValidator CertificateValidator { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Logins the specified management server, creating a new session.
        /// </summary>
        /// <param name="managementServer">The management server.</param>
        /// <param name="userName">Administrator user name.</param>
        /// <param name="password">The administrator password.</param>
        /// <param name="readOnly">Weather session should be read only.</param>
        /// <param name="sessionName">Name of the session.</param>
        /// <param name="comments">Session comments.</param>
        /// <param name="description">Session description.</param>
        /// <param name="domain">Domain to connect to.</param>
        /// <param name="continueLastSession">Weather to continue last session.</param>
        /// <param name="enterLastPublishedSession">Weather to enter last published session.</param>
        /// <param name="certificateValidation">
        /// Level of server certificate validation that should be performed.
        /// </param>
        /// <param name="detailLevelAction">
        /// Action to take when performing actions on objects and the current detail level is too low.
        /// </param>
        /// <param name="indentJson">
        /// if set to <c>true</c> json data sent to server will be indented. Helpful with debugging.
        /// </param>
        /// <param name="port">The management server API port.</param>
        /// <param name="timeout">The session timeout.</param>
        /// <param name="maxConnections">The maximum connections to establish to management server.</param>
        /// <param name="debugWriter">
        /// The debug writer. WARNING: Setting debug writer here will output you login credentials to
        /// the debug writer in the clear. Set <see cref="DebugWriter" /> after Login to prevent this.
        /// </param>
        /// <param name="certificateHash">
        /// Used to check the the server SSL certificate matches this hash. Valid only if
        /// CertificateValidation contains the flag <see cref="CertificateValidation.Auto" /> or <see cref="CertificateValidation.CertificatePinning" />
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the New
        /// Logged in Session object
        /// </returns>
        public static async Task<Session> Login(string managementServer, string userName, string password,
                    bool? readOnly = null,
                    string sessionName = null,
                    string comments = null,
                    string description = null,
                    string domain = null,
                    bool? continueLastSession = null,
                    bool? enterLastPublishedSession = null,
                    CertificateValidation certificateValidation = CertificateValidation.Auto,
                    DetailLevelActions detailLevelAction = DetailLevelActions.ThrowException,
                    bool indentJson = false,
                    int port = 443,
                    int? timeout = null,
                    int maxConnections = 5,
                    TextWriter debugWriter = null,
                    string certificateHash = null,
                    CancellationToken cancellationToken = default)
        {
            var session = new Session(
                    certificateValidation: certificateValidation,
                    detailLevelAction: detailLevelAction,
                    indentJson: indentJson,
                    maxConnections: maxConnections,
                    certificateHash: certificateHash
                )
            {
                DebugWriter = debugWriter,
                HttpSemaphore = new SemaphoreSlim(maxConnections + 1),
                URL = $"https://{managementServer}:{port}/web_api/"
            };

            var data = new JObject()
            {
                { "user", userName },
                { "password", password }
            };
            data.AddIfNotNull("read-only", readOnly);
            data.AddIfNotNull("session-name", sessionName);
            data.AddIfNotNull("session-comments", comments);
            data.AddIfNotNull("session-description", description);
            data.AddIfNotNull("session-timeout", timeout);
            data.AddIfNotNull("domain", domain);
            data.AddIfNotNull("continue-last-session", continueLastSession);
            data.AddIfNotNull("enter-last-published-session", enterLastPublishedSession);

            string jsonData = JsonConvert.SerializeObject(data, session.JsonFormatting);

            string result = await session.PostAsync("login", jsonData, cancellationToken);

            JsonConvert.PopulateObject(result, session);

            return session;
        }

        /// <summary>
        /// Logout of session and continue the session in smartconsole.
        /// </summary>
        /// <param name="uid">The session uid. <c>null</c> for current session.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ContinueSessionInSmartconsole(string uid = null, CancellationToken cancellationToken = default)
        {
            string jsonData = UIDToJson(uid);
            await PostAsync("continue-session-in-smartconsole", jsonData, cancellationToken);
            if (uid == null || uid.Equals(UID))
                Dispose();
        }

        /// <summary>
        /// Discard changes made in session.
        /// </summary>
        /// <param name="uid">The session uid. <c>null</c> for current session.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Discard(string uid = null, CancellationToken cancellationToken = default)
        {
            string jsonData = UIDToJson(uid);
            return PostAsync("discard", jsonData, cancellationToken);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_httpClient != null)
            {
                ((IDisposable)_httpClient).Dispose();
                _httpClient = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Gets the login message.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the LoginMessageDetails
        /// </returns>
        public async Task<LoginMessageDetails> GetLoginMessage(CancellationToken cancellationToken = default)
        {
            string result = await PostAsync("show-login-message", "{ }", cancellationToken);

            return JsonConvert.DeserializeObject<LoginMessageDetails>(result);
        }

        /// <summary>
        /// Logout of this instance. Also disposes this current session object.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Logout(CancellationToken cancellationToken = default)
        {
            await PostAsync("logout", "{}", cancellationToken);
            Dispose();
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
        public async System.Threading.Tasks.Task<string> PostAsync(string command, string json, CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Session", "This session has already been disposed!");

            await HttpSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                string result = null;

                string debugIP = WriteDebug(command, json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                if (command != "login")
                    content.Headers.Add("X-chkp-sid", SID);

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

        /// <summary>
        /// Publishes the session.
        /// </summary>
        /// <param name="uid">The session uid. <c>null</c> for current session.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Publish(string uid = null, CancellationToken cancellationToken = default)
        {
            string jsonData = UIDToJson(uid);
            return PostAsync("publish", jsonData, cancellationToken);
        }

        /// <summary>
        /// Sends the keep alive.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task SendKeepAlive(CancellationToken cancellationToken = default) => PostAsync("keepalive", "{}", cancellationToken);

        /// <summary>
        /// Sets the login message. All <c>null</c> values will not be changed.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="showMessage">Whether to show login message.</param>
        /// <param name="warning">Add warning sign.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the LoginMessageDetails
        /// </returns>
        public async Task<LoginMessageDetails> SetLoginMessage(string header = null, string message = null, bool? showMessage = null, bool? warning = null, CancellationToken cancellationToken = default)
        {
            var data = new JObject();

            data.AddIfNotNull("header", header);
            data.AddIfNotNull("message", message);
            data.AddIfNotNull("show-message", showMessage);
            data.AddIfNotNull("warning", warning);

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("set-login-message", jsonData, cancellationToken);

            return JsonConvert.DeserializeObject<LoginMessageDetails>(result);
        }

        internal HttpClient GetHttpClient()
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
                    if (validator.CertificateValidation != CertificateValidator.CertificateValidation)
                        throw new Exception("Cannot use different certificate validation methods under .NET 4.5.");
                    if (validator.CertificateValidation.HasFlag(CertificateValidation.CertificatePinning) && validator.ExpectedCertificateHash != CertificateValidator.ExpectedCertificateHash)
                        throw new Exception("Cannot have two differently pinned certificates under .NET 4.5.");
                }
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateValidator.ValidateServerCertficate);
                ServicePointManager.DefaultConnectionLimit = MaxConnections;
#else
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => CertificateValidator.ValidateServerCertficate(message, cert, chain, errors);
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

        private string UIDToJson(string uid)
        {
            var jObject = new JObject();
            if (uid != null)
                jObject.Add("uid", uid);
            string jsonData = JsonConvert.SerializeObject(jObject, JsonFormatting);
            return jsonData;
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