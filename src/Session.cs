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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// An active management session. This class handles all communications to the Management server.
    /// </summary>
    /// <example>
    /// <code>
    /// var session = new Session(
    ///     managementServer: "192.168.1.1",
    ///     userName: "admin",
    ///     password: "***",
    ///     certificateValidation: false
    /// );
    /// </code>
    /// </example>
    /// <seealso cref="System.IDisposable" />
    public class Session : IDisposable
    {
        #region Fields

        private static Random random = new Random();
        private HttpClient _httpClient = null;
        private bool _isDisposed = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Establishes and logs in to new management session.
        /// </summary>
        /// <param name="managementServer">The management server.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="readOnly">if set to <c>true</c> a read only connection is made.</param>
        /// <param name="sessionName">Name of the session.</param>
        /// <param name="comments">The session comments.</param>
        /// <param name="description">The session description.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="continueLastSession">
        /// When <c>true</c> the new session would continue where the last session was stopped. This
        /// option is available when the administrator has only one session that can be continued. If
        /// there is more than one session, see <see cref="Session.SwitchSession(string)" />
        /// </param>
        /// <param name="enterLastPublishedSession">
        /// Login to the last published session. Such login is done with the Read Only permissions.
        /// </param>
        /// <param name="certificateValidation">
        /// if set to <c>true</c> certificate validation is performed.
        /// </param>
        /// <param name="detailLevelAction">The detail level action.</param>
        /// <param name="indentJson">if set to <c>true</c> json data sent to server will be indented.</param>
        /// <param name="port">The management server port.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="debugWriter">
        /// The debug writer. WARNING: If set here the debug output WILL include your password in the
        /// clear! Should only set here if trying to debug the login calls. Use
        /// <see cref="Session.DebugWriter" /> to set after the login has completed to avoid
        /// including your password.
        /// </param>
        public Session(string managementServer, string userName, string password,
            bool? readOnly = null,
            string sessionName = null,
            string comments = null,
            string description = null,
            string domain = null,
            bool? continueLastSession = null,
            bool? enterLastPublishedSession = null,
            bool certificateValidation = true,
            DetailLevelActions detailLevelAction = DetailLevelActions.ThrowException,
            bool indentJson = false,
            int port = 443,
            int? timeout = null,
            TextWriter debugWriter = null)
        {
            DebugWriter = debugWriter;
            CertificateValidation = certificateValidation;
            DetailLevelAction = detailLevelAction;
            IndentJson = indentJson;

            URL = $"https://{managementServer}:{port}/web_api/";

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

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("login", jsonData);

            JsonConvert.PopulateObject(result, this);
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
        /// Gets a value indicating whether SSL certificate should be valid.
        /// </summary>
        /// <value><c>true</c> if certificate validation enabled otherwise, <c>false</c>.</value>
        public bool CertificateValidation { get; }

        /// <summary>
        /// Gets or sets the debug writer. All API posts and responses will be sent to this writer.
        /// They are sent in the RAW JSON format as sent and recived to/from the server.
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

        #endregion Properties

        #region Session Methods

        /// <summary>
        /// Logout of session and continue the session in smartconsole.
        /// </summary>
        /// <param name="uid">The session uid. <c>null</c> for current session.</param>
        public void ContinueSessionInSmartconsole(string uid = null)
        {
            string jsonData = UIDToJson(uid);
            Post("continue-session-in-smartconsole", jsonData);
            if (uid == null || uid.Equals(UID)) { Dispose(); }
        }

        /// <summary>
        /// Discard changes made in session.
        /// </summary>
        /// <param name="uid">The session uid. <c>null</c> for current session.</param>
        public void Discard(string uid = null)
        {
            string jsonData = UIDToJson(uid);
            Post("discard", jsonData);
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
        /// <returns>LoginMessageDetails</returns>
        public LoginMessageDetails GetLoginMessage()
        {
            string result = Post("show-login-message", "{ }");

            return JsonConvert.DeserializeObject<LoginMessageDetails>(result);
        }

        /// <summary>
        /// Logout of this instance.
        /// </summary>
        public void Logout()
        {
            Post("logout", "{}");
            Dispose();
        }

        /// <summary>
        /// Posts the specified command with the JSON data supplied. This can be used to send any
        /// commands this .NET package doesn't implement yet.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="json">The json.</param>
        /// <returns>JSON Response Data</returns>
        /// <exception cref="Exceptions.GenericException">
        /// This or one of the Exceptions that inherit this will be thrown if the management server
        /// returns an error.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Session - This session has already been disposed!
        /// </exception>
        /// <seealso href="https://sc1.checkpoint.com/documents/latest/APIs/index.html#web/introduction~v1.1%20" target="_blank" alt="Check Point Management API">
        /// Check Point Management API
        /// </seealso>
        public string Post(string command, string json)
        {
            try
            {
                return PostAsync(command, json).Result;
            }
            catch (Exception e)
            {
                throw e.InnerException ?? e;
            }
        }

        /// <summary>
        /// Async posts the specified command with the JSON data supplied. This can be used to send
        /// any commands this .NET package doesn't implement yet.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="json">The json.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>JSON Response Data</returns>
        /// <inheritdoc cref="Post(string, string)" select="exception|seealso" />
        public async System.Threading.Tasks.Task<string> PostAsync(string command, string json, CancellationToken cancellationToken = default)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Session", "This session has already been disposed!");
            }
            string result = null;

            string debugIP = WriteDebug(command, json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            if (command != "login")
            {
                content.Headers.Add("X-chkp-sid", SID);
            }

            var response = await GetHttpClient().PostAsync(command, content, cancellationToken);

            try
            {
                result = await response.Content.ReadAsStringAsync();

                WriteDebug(debugIP, response.StatusCode, result);

                if (!response.IsSuccessStatusCode)
                {
                    throw CheckPointError.CreateException(result, response.StatusCode);
                }
            }
            finally
            {
                response.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Publishes the session.
        /// </summary>
        /// <param name="uid">The session uid. <c>null</c> for current session.</param>
        public void Publish(string uid = null)
        {
            string jsonData = UIDToJson(uid);
            Post("publish", jsonData);
        }

        /// <summary>
        /// Sends the keep alive.
        /// </summary>
        public void SendKeepAlive() => Post("keepalive", "{}");

        /// <summary>
        /// Sets the login message. All <c>null</c> values will not be changed.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="showMessage">Whether to show login message.</param>
        /// <param name="warning">Add warning sign.</param>
        /// <returns>LoginMessageDetails</returns>
        public LoginMessageDetails SetLoginMessage(string header = null, string message = null, bool? showMessage = null, bool? warning = null)
        {
            var data = new JObject();

            data.AddIfNotNull("header", header);
            data.AddIfNotNull("message", message);
            data.AddIfNotNull("show-message", showMessage);
            data.AddIfNotNull("warning", warning);

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("set-login-message", jsonData);

            return JsonConvert.DeserializeObject<LoginMessageDetails>(result);
        }

        internal HttpClient GetHttpClient()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Session", "This session has already been disposed!");
            }
            if (_httpClient == null)
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

#if NET45
                if (!CertificateValidation)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                }
                else
                {
                    ServicePointManager.ServerCertificateValidationCallback = null;
                }
#else
                if (!CertificateValidation)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                }
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
            if (DebugWriter != null)
            {
                DebugWriter.WriteLine(message);
                DebugWriter.Flush();
            }
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string UIDToJson(string uid)
        {
            var jObject = new JObject();
            if (uid != null) { jObject.Add("uid", uid); }
            string jsonData = JsonConvert.SerializeObject(jObject, JsonFormatting); ;
            return jsonData;
        }

        private string WriteDebug(string command, string data)
        {
            if (DebugWriter != null)
            {
                string id = RandomString(8);

                DebugWriter.WriteLine($@"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")} Start Post ID:{id} Command: {command}
{data}
");
                DebugWriter.Flush();

                return id;
            }
            else return null;
        }

        private void WriteDebug(string id, HttpStatusCode code, string data)
        {
            if (DebugWriter != null)
            {
                DebugWriter.WriteLine($@"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")} Start Response ID:{id} Code: {code}
{data}
");
                DebugWriter.Flush();
            }
        }

        #region SessionInfo Methods

        /// <summary>
        /// Finds all sessions.
        /// </summary>
        /// <param name="viewPublishedSessions">if set to <c>true</c> returns published sessions.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of SessionInfos</returns>
        public SessionInfo[] FindAllSessions
            (
                bool viewPublishedSessions = false,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            int offset = 0;
            var sessions = new List<SessionInfo>();

            while (true)
            {
                var data = new Dictionary<string, dynamic>
                {
                    { "view-published-sessions", viewPublishedSessions },
                    { "limit", limit },
                    { "offset", offset },
                    { "order", (order == null)? null:new IOrder[] { order } },
                    { "details-level", DetailLevels.Full }
                };

                string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

                string result = Post("show-sessions", jsonData);

                var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<SessionInfo>>(result);

                foreach (var o in results)
                    sessions.Add(o);

                if (results.To == results.Total)
                    return sessions.ToArray();

                offset = results.To;
            }
        }

        /// <summary>
        /// Finds a session.
        /// </summary>
        /// <param name="uid">The UID to find. <c>null</c> for current session information</param>
        /// <returns>SessionInfo object</returns>
        public SessionInfo FindSession
            (
                string uid = null
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "uid", uid }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("show-session", jsonData);

            return JsonConvert.DeserializeObject<SessionInfo>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, DetailLevels.Full) } });
        }

        /// <summary>
        /// Finds sessions.
        /// </summary>
        /// <param name="viewPublishedSessions">if set to <c>true</c> returns published sessions.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SessionInfos</returns>
        public NetworkObjectsPagingResults<SessionInfo> FindSessions
            (
                bool viewPublishedSessions = false,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "view-published-sessions", viewPublishedSessions },
                { "limit", limit },
                { "offset", offset },
                { "order", (order == null)? null:new IOrder[] { order } },
                { "details-level", DetailLevels.Full }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("show-sessions", jsonData);

            var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<SessionInfo>>(result);

            if (results != null)
            {
                results.Next = delegate ()
                {
                    if (results.To == results.Total) { return null; }
                    return FindSessions(viewPublishedSessions, limit, results.To, order);
                };
            }

            return results;
        }

        /// <summary>
        /// Edit user's current session. All <c>null</c> values will not be changed.
        /// </summary>
        /// <param name="name">The session name.</param>
        /// <param name="description">The session description.</param>
        /// <param name="tags">The session tags.</param>
        /// <param name="color">The session color.</param>
        /// <param name="comments">The session comments.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <returns>Updated SessionInfo</returns>
        public SessionInfo SetSessionInfo(string name = null, string description = null, string[] tags = null, Colors? color = null, string comments = null, Ignore ignore = Ignore.No)
        {
            var data = new JObject()
            {
                { "new-name", name },
                { "description", description },
                { "comments", comments }
            };

            if (tags != null) data.Add("tags", new JArray(tags));
            if (color != null) data.Add("color", JToken.FromObject(color));

            data.AddIgnore(ignore);

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            string result = Post("set-session", jsonData);

            return JsonConvert.DeserializeObject<SessionInfo>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, DetailLevels.Full) } });
        }

        /// <summary>
        /// Switch to another session.
        /// </summary>
        /// <param name="uid">The UID to switch to.</param>
        /// <returns>SessionInfo object</returns>
        public SessionInfo SwitchSession
            (
                string uid
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "uid", uid }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("switch-session", jsonData);

            var si = JsonConvert.DeserializeObject<SessionInfo>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, DetailLevels.Full) } });

            UID = si.UID;

            return si;
        }

        #endregion SessionInfo Methods

        #endregion Session Methods

        #region Object Methods

        #region Get Object(s)

        /// <summary>
        /// Finds all objects that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="type">The object type.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of IObjectSummary</returns>
        public IObjectSummary[] FindAllObjects
            (
                string filter = null,
                string type = null,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<IObjectSummary>
                (
                    Session: this,
                    Type: type,
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds an object by UID.
        /// </summary>
        /// <param name="uid">The UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>IObjectSummary object</returns>
        public IObjectSummary FindObject
            (
                string uid,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke
                (
                    Session: this,
                    uid: uid,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds objects that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="type">The object type.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of IObjectSummary</returns>
        public NetworkObjectsPagingResults<IObjectSummary> FindObjects
            (
                string filter = null,
                string type = null,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<IObjectSummary>
                (
                    Session: this,
                    Type: type,
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion Get Object(s)

        #region AddressRange Methods

        /// <summary>
        /// Deletes an address range.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteAddressRange
        (
            string value,
            Ignore ignore = Delete.Defaults.ignore
        )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-address-range",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds an address range.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>AddressRange object</returns>
        public AddressRange FindAddressRange
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<AddressRange>
                (
                    Session: this,
                    Command: "show-address-range",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds address ranges.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of AddressRanges</returns>
        public NetworkObjectsPagingResults<AddressRange> FindAddressRanges
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<AddressRange>
                (
                    Session: this,
                    Command: "show-address-ranges",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds address ranges that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of AddressRanges</returns>
        public NetworkObjectsPagingResults<AddressRange> FindAddressRanges
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<AddressRange>
                (
                    Session: this,
                    Type: "address-range",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all address ranges.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of AddressRanges</returns>
        public AddressRange[] FindAllAddressRanges
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<AddressRange>
                (
                    Session: this,
                    Command: "show-address-ranges",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all address ranges that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of AddressRanges</returns>
        public AddressRange[] FindAllAddressRanges
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<AddressRange>
                (
                    Session: this,
                    Type: "address-range",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        #endregion AddressRange Methods

        #region Group Methods

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteGroup
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-group",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Groups</returns>
        public Group[] FindAllGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Group>
                (
                    Session: this,
                    Type: "group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Groups</returns>
        public Group[] FindAllGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Group>
                (
                    Session: this,
                    Command: "show-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Group object</returns>
        public Group FindGroup
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<Group>
                (
                    Session: this,
                    Command: "show-group",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Groups</returns>
        public NetworkObjectsPagingResults<Group> FindGroups
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Group>
                (
                    Session: this,
                    Type: "group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Groups</returns>
        public NetworkObjectsPagingResults<Group> FindGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Group>
                (
                    Session: this,
                    Command: "show-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion Group Methods

        #region GroupWithExclusion Methods

        /// <summary>
        /// Deletes a group with exclusion.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteGroupWithExclusion
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-group-with-exclusion",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all groups with exclusion that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of GroupWithExclusions</returns>
        public GroupWithExclusion[] FindAllGroupsWithExclusion
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<GroupWithExclusion>
                (
                    Session: this,
                    Type: "group-with-exclusion",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all groups with exclusion.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of GroupWithExclusions</returns>
        public GroupWithExclusion[] FindAllGroupsWithExclusion
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<GroupWithExclusion>
                (
                    Session: this,
                    Command: "show-groups-with-exclusion",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds groups with exclusion that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of GroupWithExclusions</returns>
        public NetworkObjectsPagingResults<GroupWithExclusion> FindGroupsWithExclusion
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<GroupWithExclusion>
                (
                    Session: this,
                    Type: "group-with-exclusion",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds groups with exclusion.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of GroupWithExclusions</returns>
        public NetworkObjectsPagingResults<GroupWithExclusion> FindGroupsWithExclusion
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<GroupWithExclusion>
                (
                    Session: this,
                    Command: "show-groups-with-exclusion",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a group with exclusion.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>GroupWithExclusion object</returns>
        public GroupWithExclusion FindGroupWithExclusion
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<GroupWithExclusion>
                (
                    Session: this,
                    Command: "show-group-with-exclusion",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        #endregion GroupWithExclusion Methods

        #region Host Methods

        /// <summary>
        /// Deletes a host.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteHost
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-host",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all hosts that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Hosts</returns>
        public Host[] FindAllHosts
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Host>
                (
                    Session: this,
                    Type: "host",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all hosts.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Hosts</returns>
        public Host[] FindAllHosts
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Host>
                (
                    Session: this,
                    Command: "show-hosts",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a host.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Host object</returns>
        public Host FindHost
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<Host>
                (
                    Session: this,
                    Command: "show-host",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds hosts that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Hosts</returns>
        public NetworkObjectsPagingResults<Host> FindHosts
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Host>
                (
                    Session: this,
                    Type: "host",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds hosts.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Hosts</returns>
        public NetworkObjectsPagingResults<Host> FindHosts
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Host>
                (
                    Session: this,
                    Command: "show-hosts",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion Host Methods

        #region MulticastAddressRange Methods

        /// <summary>
        /// Deletes a multicast address range.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteMulticastAddressRange
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-multicast-address-range",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all multicast address ranges that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of MulticastAddressRanges</returns>
        public MulticastAddressRange[] FindAllMulticastAddressRanges
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<MulticastAddressRange>
                (
                    Session: this,
                    Type: "multicast-address-range",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all multicast address ranges.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of MulticastAddressRanges</returns>
        public MulticastAddressRange[] FindAllMulticastAddressRanges
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<MulticastAddressRange>
                (
                    Session: this,
                    Command: "show-multicast-address-ranges",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a multicast address range.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>MulticastAddressRange object</returns>
        public MulticastAddressRange FindMulticastAddressRange
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<MulticastAddressRange>
                (
                    Session: this,
                    Command: "show-multicast-address-range",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds multicast address ranges that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of MulticastAddressRanges</returns>
        public NetworkObjectsPagingResults<MulticastAddressRange> FindMulticastAddressRanges
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<MulticastAddressRange>
                (
                    Session: this,
                    Type: "multicast-address-range",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds multicast address ranges.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of MulticastAddressRanges</returns>
        public NetworkObjectsPagingResults<MulticastAddressRange> FindMulticastAddressRanges
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<MulticastAddressRange>
                (
                    Session: this,
                    Command: "show-multicast-address-ranges",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion MulticastAddressRange Methods

        #region Network Methods

        /// <summary>
        /// Deletes a network.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteNetwork
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-network",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all networks that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Networks</returns>
        public Network[] FindAllNetworks
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Network>
                (
                    Session: this,
                    Type: "network",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all networks.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Networks</returns>
        public Network[] FindAllNetworks
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Network>
                (
                    Session: this,
                    Command: "show-networks",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a network.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Network object</returns>
        public Network FindNetwork
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<Network>
                (
                    Session: this,
                    Command: "show-network",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds networks that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Networks</returns>
        public NetworkObjectsPagingResults<Network> FindNetworks
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Network>
                (
                    Session: this,
                    Type: "network",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds networks.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Networks</returns>
        public NetworkObjectsPagingResults<Network> FindNetworks
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Network>
                (
                    Session: this,
                    Command: "show-networks",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion Network Methods

        #region SimpleGateway Methods

        /// <summary>
        /// Deletes a simple gateway.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteSimpleGateway
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-simple-gateway",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all simple gateways that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of SimpleGateways</returns>
        public SimpleGateway[] FindAllSimpleGateways
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<SimpleGateway>
                (
                    Session: this,
                    Type: "simple-gateway",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all simple gateways.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of SimpleGateways</returns>
        public SimpleGateway[] FindAllSimpleGateways
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateways",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a simple gateway.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>SimpleGateway object</returns>
        public SimpleGateway FindSimpleGateway
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateway",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds simple gateways that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SimpleGateways</returns>
        public NetworkObjectsPagingResults<SimpleGateway> FindSimpleGateways
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<SimpleGateway>
                (
                    Session: this,
                    Type: "simple-gateway",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds simple gateways.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SimpleGateways</returns>
        public NetworkObjectsPagingResults<SimpleGateway> FindSimpleGateways
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateways",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion SimpleGateway Methods

        #region SecurityZone Methods

        /// <summary>
        /// Deletes a security zone.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteSecurityZone
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-security-zone",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all security zones that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of SecurityZones</returns>
        public SecurityZone[] FindAllSecurityZones
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<SecurityZone>
                (
                    Session: this,
                    Type: "security-zone",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all security zones.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of SecurityZones</returns>
        public SecurityZone[] FindAllSecurityZones
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<SecurityZone>
                (
                    Session: this,
                    Command: "show-security-zones",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a security zone.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>SecurityZone object</returns>
        public SecurityZone FindSecurityZone
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<SecurityZone>
                (
                    Session: this,
                    Command: "show-security-zone",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds security zones that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SecurityZones</returns>
        public NetworkObjectsPagingResults<SecurityZone> FindSecurityZones
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<SecurityZone>
                (
                    Session: this,
                    Type: "security-zone",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds security zones.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SecurityZones</returns>
        public NetworkObjectsPagingResults<SecurityZone> FindSecurityZones
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<SecurityZone>
                (
                    Session: this,
                    Command: "show-security-zones",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion SecurityZone Methods

        #region Tag Methods

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteTag
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-tag",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all tags that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Tags</returns>
        public Tag[] FindAllTags
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Tag>
                (
                    Session: this,
                    Type: "tag",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all tags.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Tags</returns>
        public Tag[] FindAllTags
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Tag>
                (
                    Session: this,
                    Command: "show-tags",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a tag.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Tag object</returns>
        public Tag FindTag
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<Tag>
                (
                    Session: this,
                    Command: "show-tag",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds tags that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Tags</returns>
        public NetworkObjectsPagingResults<Tag> FindTags
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Tag>
                (
                    Session: this,
                    Type: "tag",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds tags.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Tags</returns>
        public NetworkObjectsPagingResults<Tag> FindTags
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Tag>
                (
                    Session: this,
                    Command: "show-tags",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion Tag Methods

        #region Time Methods

        /// <summary>
        /// Deletes a time object.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteTime
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-time",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all time objects that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Times</returns>
        public Time[] FindAllTimes
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Time>
                (
                    Session: this,
                    Type: "time",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all time objects.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Times</returns>
        public Time[] FindAllTimes
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Time>
                (
                    Session: this,
                    Command: "show-times",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a time object.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Time object</returns>
        public Time FindTime
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<Time>
                (
                    Session: this,
                    Command: "show-time",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds time objects that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Times</returns>
        public NetworkObjectsPagingResults<Time> FindTimes
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Time>
                (
                    Session: this,
                    Type: "time",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds time objects.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Times</returns>
        public NetworkObjectsPagingResults<Time> FindTimes
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<Time>
                (
                    Session: this,
                    Command: "show-times",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion Time Methods

        #region TimeGroup Methods

        /// <summary>
        /// Deletes a time group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteTimeGroup
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-time-group",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all time groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of TimeGroups</returns>
        public TimeGroup[] FindAllTimeGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<TimeGroup>
                (
                    Session: this,
                    Type: "time-group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all time groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of TimeGroups</returns>
        public TimeGroup[] FindAllTimeGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<TimeGroup>
                (
                    Session: this,
                    Command: "show-time-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a time group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>TimeGroup object</returns>
        public TimeGroup FindTimeGroup
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<TimeGroup>
                (
                    Session: this,
                    Command: "show-time-group",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds time groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of TimeGroups</returns>
        public NetworkObjectsPagingResults<TimeGroup> FindTimeGroups
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<TimeGroup>
                (
                    Session: this,
                    Type: "time-group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds time groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of TimeGroups</returns>
        public NetworkObjectsPagingResults<TimeGroup> FindTimeGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<TimeGroup>
                (
                    Session: this,
                    Command: "show-time-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion TimeGroup Methods

        #endregion Object Methods

        #region Service Methods

        #region ApplicationCategory Methods

        /// <summary>
        /// Deletes an application category.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteApplicationCategory
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-application-site-category",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all application categories that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ApplicationCategory</returns>
        public ApplicationCategory[] FindAllApplicationCategories
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ApplicationCategory>
                (
                    Session: this,
                    Type: "application-site-category",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all application categories.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ApplicationCategory</returns>
        public ApplicationCategory[] FindAllApplicationCategories
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ApplicationCategory>
                (
                    Session: this,
                    Command: "show-application-site-categories",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds application categories that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ApplicationCategory</returns>
        public NetworkObjectsPagingResults<ApplicationCategory> FindApplicationCategories
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ApplicationCategory>
                (
                    Session: this,
                    Type: "application-site-category",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds application categories.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ApplicationCategory</returns>
        public NetworkObjectsPagingResults<ApplicationCategory> FindApplicationCategories
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ApplicationCategory>
                (
                    Session: this,
                    Command: "show-application-site-categories",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds an application category.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ApplicationCategory object</returns>
        public ApplicationCategory FindApplicationCategory
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ApplicationCategory>
                (
                    Session: this,
                    Command: "show-application-site-category",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        #endregion ApplicationCategory Methods

        #region ApplicationGroup Methods

        /// <summary>
        /// Deletes an application group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteApplicationGroup
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-application-site-group",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all application groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ApplicationGroup</returns>
        public ApplicationGroup[] FindAllApplicationGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ApplicationGroup>
                (
                    Session: this,
                    Type: "application-site-group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all application groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ApplicationGroup</returns>
        public ApplicationGroup[] FindAllApplicationGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ApplicationGroup>
                (
                    Session: this,
                    Command: "show-application-site-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds an application group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ApplicationGroup object</returns>
        public ApplicationGroup FindApplicationGroup
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ApplicationGroup>
                (
                    Session: this,
                    Command: "show-application-site-group",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds application groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ApplicationGroup</returns>
        public NetworkObjectsPagingResults<ApplicationGroup> FindApplicationGroups
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ApplicationGroup>
                (
                    Session: this,
                    Type: "application-site-group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds application groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ApplicationGroup</returns>
        public NetworkObjectsPagingResults<ApplicationGroup> FindApplicationGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ApplicationGroup>
                (
                    Session: this,
                    Command: "show-application-site-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ApplicationGroup Methods

        #region ApplicationSite Methods

        /// <summary>
        /// Deletes an application site.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteApplicationSite
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-application-site",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all application sites that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ApplicationSite</returns>
        public ApplicationSite[] FindAllApplicationSites
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ApplicationSite>
                (
                    Session: this,
                    Type: "application-site",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all application sites.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ApplicationSite</returns>
        public ApplicationSite[] FindAllApplicationSites
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ApplicationSite>
                (
                    Session: this,
                    Command: "show-application-sites",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds an application site.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ApplicationSite object</returns>
        public ApplicationSite FindApplicationSite
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ApplicationSite>
                (
                    Session: this,
                    Command: "show-application-site",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds an application site.
        /// </summary>
        /// <param name="applicationID">The application identifier.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ApplicationSite object</returns>
        public ApplicationSite FindApplicationSite
            (
                int applicationID,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "application-id", applicationID },
                { "details-level", detailLevel.ToString() }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("show-application-site", jsonData);

            return JsonConvert.DeserializeObject<ApplicationSite>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, detailLevel) } });
        }

        /// <summary>
        /// Finds application sites that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ApplicationSite</returns>
        public NetworkObjectsPagingResults<ApplicationSite> FindApplicationSites
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ApplicationSite>
                (
                    Session: this,
                    Type: "application-site",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds application sites.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ApplicationSite</returns>
        public NetworkObjectsPagingResults<ApplicationSite> FindApplicationSites
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ApplicationSite>
                (
                    Session: this,
                    Command: "show-application-sites",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ApplicationSite Methods

        #region ServiceDceRpc Methods

        /// <summary>
        /// Deletes a DCE-RPC service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceDceRpc
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-dce-rpc",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all DCE-RPC services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceDceRpc[] FindAllServicesDceRpc
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Type: "service-dce-rpc",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all DCE-RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceDceRpc[] FindAllServicesDceRpc
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Command: "show-services-dce-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a DCE-RPC service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceDceRpc object</returns>
        public ServiceDceRpc FindServiceDceRpc
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Command: "show-service-dce-rpc",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds DCE-RPC services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceDceRpc</returns>
        public NetworkObjectsPagingResults<ServiceDceRpc> FindServicesDceRpc
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Type: "service-dce-rpc",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds DCE-RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceDceRpc</returns>
        public NetworkObjectsPagingResults<ServiceDceRpc> FindServicesDceRpc
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Command: "show-services-dce-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ServiceDceRpc Methods

        #region ICMP Methods

        /// <summary>
        /// Deletes a service-icmp.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceICMP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-icmp",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all service-icmps that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Objects</returns>
        public ServiceICMP[] FindAllServicesICMP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceICMP>
                (
                    Session: this,
                    Type: "service-icmp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all services ICMP.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Objects</returns>
        public ServiceICMP[] FindAllServicesICMP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceICMP>
                (
                    Session: this,
                    Command: "show-services-icmp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a service-icmp.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Object object</returns>
        public ServiceICMP FindServiceICMP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceICMP>
                (
                    Session: this,
                    Command: "show-service-icmp",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds service-icmps that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public NetworkObjectsPagingResults<ServiceICMP> FindServicesICMP
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceICMP>
                (
                    Session: this,
                    Type: "service-icmp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds services ICMP.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public NetworkObjectsPagingResults<ServiceICMP> FindServicesICMP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceICMP>
                (
                    Session: this,
                    Command: "show-services-icmp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ICMP Methods

        #region ICMP6 Methods

        /// <summary>
        /// Deletes a service-icmp6.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceICMP6
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-icmp6",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all service-icmp6s that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Objects</returns>
        public ServiceICMP6[] FindAllServicesICMP6
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Type: "service-icmp6",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all services ICMP6.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of Objects</returns>
        public ServiceICMP6[] FindAllServicesICMP6
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Command: "show-services-icmp6",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a service-icmp6.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>Object object</returns>
        public ServiceICMP6 FindServiceICMP6
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Command: "show-service-icmp6",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds service-icmp6s that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public NetworkObjectsPagingResults<ServiceICMP6> FindServicesICMP6
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Type: "service-icmp6",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds services ICMP6.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public NetworkObjectsPagingResults<ServiceICMP6> FindServicesICMP6
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Command: "show-services-icmp6",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ICMP6 Methods

        #region ServiceOther Methods

        /// <summary>
        /// Deletes a other service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceOther
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-other",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all other services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceOther</returns>
        public ServiceOther[] FindAllServicesOther
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceOther>
                (
                    Session: this,
                    Type: "service-other",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all other services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceOther</returns>
        public ServiceOther[] FindAllServicesOther
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceOther>
                (
                    Session: this,
                    Command: "show-services-other",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a other service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceOther object</returns>
        public ServiceOther FindServiceOther
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceOther>
                (
                    Session: this,
                    Command: "show-service-other",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds other services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceOther</returns>
        public NetworkObjectsPagingResults<ServiceOther> FindServicesOther
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceOther>
                (
                    Session: this,
                    Type: "service-other",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds other services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceOther</returns>
        public NetworkObjectsPagingResults<ServiceOther> FindServicesOther
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceOther>
                (
                    Session: this,
                    Command: "show-services-other",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ServiceOther Methods

        #region ServiceRPC Methods

        /// <summary>
        /// Deletes a RPC service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceRPC
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-rpc",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all RPC services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceRPC</returns>
        public ServiceRPC[] FindAllServicesRPC
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceRPC>
                (
                    Session: this,
                    Type: "service-rpc",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceRPC</returns>
        public ServiceRPC[] FindAllServicesRPC
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceRPC>
                (
                    Session: this,
                    Command: "show-services-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a RPC service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceRPC object</returns>
        public ServiceRPC FindServiceRPC
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceRPC>
                (
                    Session: this,
                    Command: "show-service-rpc",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds RPC services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceRPC</returns>
        public NetworkObjectsPagingResults<ServiceRPC> FindServicesRPC
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceRPC>
                (
                    Session: this,
                    Type: "service-rpc",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceRPC</returns>
        public NetworkObjectsPagingResults<ServiceRPC> FindServicesRPC
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceRPC>
                (
                    Session: this,
                    Command: "show-services-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ServiceRPC Methods

        #region ServiceSCTP Methods

        /// <summary>
        /// Deletes a SCTP service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceSCTP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-sctp",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all SCTP services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceSCTP</returns>
        public ServiceSCTP[] FindAllServicesSCTP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Type: "service-sctp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all SCTP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceSCTP</returns>
        public ServiceSCTP[] FindAllServicesSCTP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Command: "show-services-sctp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a SCTP service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceSCTP object</returns>
        public ServiceSCTP FindServiceSCTP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Command: "show-service-sctp",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds SCTP services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceSCTP</returns>
        public NetworkObjectsPagingResults<ServiceSCTP> FindServicesSCTP
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Type: "service-sctp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds SCTP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceSCTP</returns>
        public NetworkObjectsPagingResults<ServiceSCTP> FindServicesSCTP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Command: "show-services-sctp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ServiceSCTP Methods

        #region ServiceTCP Methods

        /// <summary>
        /// Deletes a TCP service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceTCP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-tcp",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all TCP services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceTCP[] FindAllServicesTCP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceTCP>
                (
                    Session: this,
                    Type: "service-tcp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all TCP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceTCP[] FindAllServicesTCP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceTCP>
                (
                    Session: this,
                    Command: "show-services-tcp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds TCP services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public NetworkObjectsPagingResults<ServiceTCP> FindServicesTCP
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceTCP>
                (
                    Session: this,
                    Type: "service-tcp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds TCP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public NetworkObjectsPagingResults<ServiceTCP> FindServicesTCP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceTCP>
                (
                    Session: this,
                    Command: "show-services-tcp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a TCP service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceTCP object</returns>
        public ServiceTCP FindServiceTCP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceTCP>
                (
                    Session: this,
                    Command: "show-service-tcp",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        #endregion ServiceTCP Methods

        #region ServiceUDP Methods

        /// <summary>
        /// Deletes a UDP service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceUDP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-udp",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all UDP services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceUDP[] FindAllServicesUDP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceUDP>
                (
                    Session: this,
                    Type: "service-udp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all UDP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceUDP[] FindAllServicesUDP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceUDP>
                (
                    Session: this,
                    Command: "show-services-udp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds UDP services that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public NetworkObjectsPagingResults<ServiceUDP> FindServicesUDP
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceUDP>
                (
                    Session: this,
                    Type: "service-udp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds UDP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public NetworkObjectsPagingResults<ServiceUDP> FindServicesUDP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceUDP>
                (
                    Session: this,
                    Command: "show-services-udp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a UDP service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceUDP object</returns>
        public ServiceUDP FindServiceUDP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceUDP>
                (
                    Session: this,
                    Command: "show-service-udp",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        #endregion ServiceUDP Methods

        #region ServiceGroup Methods

        /// <summary>
        /// Deletes a service group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        public void DeleteServiceGroup
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore
            )
        {
            Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-group",
                    Value: value,
                    Ignore: ignore
                );
        }

        /// <summary>
        /// Finds all service groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceGroup[] FindAllServiceGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceGroup>
                (
                    Session: this,
                    Type: "service-group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds all service groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of ServiceTCP</returns>
        public ServiceGroup[] FindAllServiceGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<ServiceGroup>
                (
                    Session: this,
                    Command: "show-service-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order
                );
        }

        /// <summary>
        /// Finds a service group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <returns>ServiceGroup object</returns>
        public ServiceGroup FindServiceGroup
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel
            )
        {
            return Find.Invoke<ServiceGroup>
                (
                    Session: this,
                    Command: "show-service-group",
                    Value: value,
                    DetailLevel: detailLevel
                );
        }

        /// <summary>
        /// Finds service groups that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public NetworkObjectsPagingResults<ServiceGroup> FindServiceGroups
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceGroup>
                (
                    Session: this,
                    Type: "service-group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        /// <summary>
        /// Finds service groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public NetworkObjectsPagingResults<ServiceGroup> FindServiceGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order
            )
        {
            return Finds.Invoke<ServiceGroup>
                (
                    Session: this,
                    Command: "show-service-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
                );
        }

        #endregion ServiceGroup Methods

        #endregion Service Methods

        #region Misc. Methods

        #region Task Methods

        /// <summary>
        /// Find a task.
        /// </summary>
        /// <param name="taskID">The task identifier.</param>
        /// <returns>Task</returns>
        public Task FindTask
            (
                string taskID
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "task-id", taskID },
                { "details-level", DetailLevels.Full }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("show-task", jsonData);

            var results = JsonConvert.DeserializeObject<JObject>(result);
            var array = (JArray)results.GetValue("tasks");

            var tasks = JsonConvert.DeserializeObject<Task[]>(array.ToString(), new JsonSerializerSettings() { Converters = { new SessionConstructorConverter(this) } });

            return tasks?[0];
        }

        /// <summary>
        /// Runs the script on multiple targets.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The script arguments.</param>
        /// <param name="targets">The targets.</param>
        /// <param name="comments">Script comments.</param>
        /// <returns>A read-only dictionary detailing the task ID for each target.</returns>
        public IReadOnlyDictionary<string, string> RunScript(
                string scriptName,
                string script,
                string args,
                string[] targets,
                string comments = null
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "script-name", scriptName },
                { "script", script },
                { "args", args },
                { "targets", targets },
                { "comments", comments }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("run-script", jsonData);

            var results = JsonConvert.DeserializeObject<JObject>(result);
            var array = (JArray)results.GetValue("tasks");

            var dicResults = new Dictionary<string, string>();

            foreach (var r in array)
            {
                var j = r as JObject;
                dicResults.Add(j.GetValue("target").ToString(), j.GetValue("task-id").ToString());
            }

            return new ReadOnlyDictionary<string, string>(dicResults);
        }

        /// <summary>
        /// Runs the script on a single target.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="target">The target.</param>
        /// <param name="comments">The script comments.</param>
        /// <returns>Task ID</returns>
        public string RunScript(
                string scriptName,
                string script,
                string args,
                string target,
                string comments = null
            )
        {
            var results = RunScript(
                    scriptName,
                    script,
                    args,
                    new string[] { target },
                    comments
                );

            return results?.Values.First();
        }

        #endregion Task Methods

        #region Policy Methods

        /// <summary>
        /// Installs the policy to gateways.
        /// </summary>
        /// <param name="policy">The policy to install.</param>
        /// <param name="targets">The target gateways.</param>
        /// <param name="access">if set to <c>true</c> installs the access policy.</param>
        /// <param name="threatPrevention">
        /// if set to <c>true</c> installs the threat prevention policy.
        /// </param>
        /// <param name="installOnAllClusterMembersOrFail">
        /// if set to <c>true</c> will fail if it cannot install policy to all cluster members. if
        /// set to <c>false</c> can complete with partial success if not all cluster members available.
        /// </param>
        /// <param name="prepareOnly">if set to <c>true</c> will prepare only.</param>
        /// <param name="revision">The revision of the policy to install.</param>
        /// <returns>Task ID</returns>
        public string InstallPolicy(
            string policy,
            string[] targets,
            bool access,
            bool threatPrevention,
            bool installOnAllClusterMembersOrFail = true,
            bool prepareOnly = false,
            string revision = null)
        {
            var data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy },
                { "targets", targets },
                { "access", access },
                { "threat-prevention", threatPrevention },
                { "install-on-all-cluster-members-or-fail", installOnAllClusterMembersOrFail },
                { "prepare-only", prepareOnly },
                { "revision", revision }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("install-policy", jsonData);

            var taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        /// <summary>
        /// Verifies the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns>Task ID</returns>
        public string VerifyPolicy(string policy)
        {
            var data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("verify-policy", jsonData);

            var taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        #endregion Policy Methods

        #region WhereUsed Methods

        /// <summary>
        /// Searches for usage of the target object in other objects and rules.
        /// </summary>
        /// <param name="identifier">The object identifier to search for.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="indirect">if set to <c>true</c> results will include indirect uses.</param>
        /// <param name="indirectMaxDepth">The indirect maximum depth.</param>
        /// <returns>WhereUsed object</returns>
        public WhereUsed FindWhereUsed(string identifier, DetailLevels detailLevel = DetailLevels.Standard, bool indirect = false, int indirectMaxDepth = 5)
        {
            var data = new JObject()
            {
                { identifier.isUID() ? "uid" : "name", identifier },
                { "details-level", detailLevel.ToString() }
            };

            if (indirect)
            {
                data.Add("indirect", true);
                data.Add("indirect-max-depth", indirectMaxDepth);
            }

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("where-used", jsonData);

            var objectConverter = new ObjectConverter(this, detailLevel, detailLevel);

            var whereUsed = JsonConvert.DeserializeObject<WhereUsed>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            objectConverter.PostDeserilization(whereUsed.UsedDirectly?.Objects);
            objectConverter.PostDeserilization(whereUsed.UsedIndirectly?.Objects);

            return whereUsed;
        }

        #endregion WhereUsed Methods

        #region Unused Methods

        /// <summary>
        /// Searches for unusage objects.
        /// </summary>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <returns>Array of IObjectSummary objects</returns>
        public IObjectSummary[] FindAllUnusedObjects(DetailLevels detailLevel = DetailLevels.Standard, int limit = 50, IOrder order = null)
        {
            int offset = 0;
            var objectConverter = new ObjectConverter(this, detailLevel, detailLevel);
            var objs = new List<IObjectSummary>();

            while (true)
            {
                var data = new Dictionary<string, dynamic>
                {
                    { "details-level", detailLevel.ToString() },
                    { "limit", limit },
                    { "offset", offset },
                    { "order", (order == null)? null:new IOrder[] { order } }
                };

                string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

                string result = Post("show-unused-objects", jsonData);

                var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<IObjectSummary>>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

                foreach (var o in results)
                    objs.Add(o);

                if (results.To == results.Total)
                {
                    objectConverter.PostDeserilization(objs);
                    return objs.ToArray();
                }

                offset = results.To;
            }
        }

        /// <summary>
        /// Searches for unusage objects.
        /// </summary>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of IObjectSummary objects</returns>
        public NetworkObjectsPagingResults<IObjectSummary> FindUnusedObjects(DetailLevels detailLevel = DetailLevels.Standard, int limit = 50, int offset = 0, IOrder order = null)
        {
            var objectConverter = new ObjectConverter(this, detailLevel, detailLevel);

            var data = new Dictionary<string, dynamic>
            {
                { "details-level", detailLevel.ToString() },
                { "limit", limit },
                { "offset", offset },
                { "order", (order == null)? null:new IOrder[] { order } }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = Post("show-unused-objects", jsonData);

            var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<IObjectSummary>>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            if (results != null)
            {
                objectConverter.PostDeserilization(results);
                results.Next = delegate ()
                {
                    if (results.To == results.Total) { return null; }
                    return FindUnusedObjects(detailLevel, limit, results.To, order);
                };
            }

            return results;
        }

        #endregion Unused Methods

        #endregion Misc. Methods
    }
}