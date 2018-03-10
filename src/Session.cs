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
    /// var session = new Session( new SessionOptions() {
    ///     ManagementServer = 192.168.1.1,
    ///     User = "admin",
    ///     Password = "***",
    ///     CertificateValidation = false }
    /// );
    /// </code>
    /// </example>
    /// <seealso cref="System.IDisposable" />
    public class Session : IDisposable
    {
        #region Fields

        private HttpClient _httpClient = null;
        private bool _isDisposed = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Establishes and logs in to new management session.
        /// </summary>
        /// <param name="options">The session options.</param>
        /// <param name="debugWriter">
        /// The debug writer. WARNING: If set here the debug output WILL include your password in the
        /// clear! Should only set here if trying to debug the login calls. Use
        /// <see cref="Session.DebugWriter" /> to set after the login has completed to avoid
        /// including your password.
        /// </param>
        public Session(SessionOptions options, TextWriter debugWriter = null)
        {
            Options = options;
            DebugWriter = debugWriter;

            URL = $"https://{Options.ManagementServer}:{Options.Port}/web_api/";

            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "user", Options.User },
                { "password", Options.Password },
                { "read-only", Options.ReadOnly }
            };

            Options.Password = null;

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = this.Post("login", jsonData);

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
        /// Gets or sets the debug writer. All API posts and responses will be sent to this writer.
        /// They are sent in the RAW JSON format as sent and recived to/from the server.
        /// </summary>
        /// <value>The text writer to send all debug output to.</value>
        public TextWriter DebugWriter { get; set; }

        /// <summary>
        /// Information about the available disk space on the management server.
        /// </summary>
        /// <value>The disk space message.</value>
        [JsonProperty(PropertyName = "disk-space-message")]
        public string DiskSpaceMessage { get; private set; }

        /// <summary>
        /// Timestamp when administrator last accessed the management server.
        /// </summary>
        /// <value>The last login was at.</value>
        [JsonProperty(PropertyName = "last-login-was-at")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime LastLoginWasAt { get; private set; }

        /// <summary>
        /// Session is read only status.
        /// </summary>
        /// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "read-only")]
        public bool ReadOnly { get; private set; }

        //TODO login-message
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

        internal SessionOptions Options { get; private set; }

        /// <summary>
        /// Gets the JSON formatting setting.
        /// </summary>
        /// <value>The JSON formatting.</value>
        protected internal Formatting JsonFormatting => (Options.IndentJson) ? Formatting.Indented : Formatting.None;

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
        public async System.Threading.Tasks.Task<string> PostAsync(string command, string json, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Session", "This session has already been disposed!");
            }
            string result = null;

            if (DebugWriter != null)
            {
                DebugWriter.WriteLine($@"{$" Posting Command: {command} ".CenterString(60, '-')}
{json}");
            }

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            if (command != "login")
            {
                content.Headers.Add("X-chkp-sid", SID);
            }

            HttpResponseMessage response = await GetHttpClient().PostAsync(command, content, cancellationToken);

            try
            {
                result = await response.Content.ReadAsStringAsync();

                if (DebugWriter != null)
                {
                    DebugWriter.WriteLine(
                        $@"{$" HTTP response {response.StatusCode} ".CenterString(60, '-')}
{result}
{"".CenterString(60, '-')}"

                    );
                }

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
        public void SendKeepAlive()
        {
            Post("keepalive", "{}");
        }

        internal HttpClient GetHttpClient()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Session", "This session has already been disposed!");
            }
            if (_httpClient == null)
            {
                HttpClientHandler handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression && Options.Compression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

#if NETSTANDARD2_0
                if (!Options.CertificateValidation)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                }
#elif NET45
                if (!Options.CertificateValidation)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                }
                else
                {
                    ServicePointManager.ServerCertificateValidationCallback = null;
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
            DebugWriter?.WriteLine(message);
        }

        private string UIDToJson(string uid)
        {
            JObject jObject = new JObject();
            if (uid != null) { jObject.Add("uid", uid); }
            string jsonData = JsonConvert.SerializeObject(jObject, JsonFormatting); ;
            return jsonData;
        }

        #endregion Session Methods

        #region Object Methods

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
        /// Finds all address ranges.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of AddressRanges</returns>
        public NetworkObjectsPagingResults<AddressRange> FindAllAddressRanges
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<AddressRange>
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
        /// Finds all address ranges that match filter.
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
        public NetworkObjectsPagingResults<AddressRange> FindAllAddressRanges
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Groups</returns>
        public NetworkObjectsPagingResults<Group> FindAllGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Group>
                (
                    Session: this,
                    Command: "show-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Groups</returns>
        public NetworkObjectsPagingResults<Group> FindAllGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all groups with exclusion.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of GroupWithExclusions</returns>
        public NetworkObjectsPagingResults<GroupWithExclusion> FindAllGroupsWithExclusion
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<GroupWithExclusion>
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
        /// Finds all groups with exclusion that match filter.
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
        public NetworkObjectsPagingResults<GroupWithExclusion> FindAllGroupsWithExclusion
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
        /// Finds all hosts.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Hosts</returns>
        public NetworkObjectsPagingResults<Host> FindAllHosts
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Host>
                (
                    Session: this,
                    Command: "show-hosts",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Hosts</returns>
        public NetworkObjectsPagingResults<Host> FindAllHosts
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all multicast address ranges.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of MulticastAddressRanges</returns>
        public NetworkObjectsPagingResults<MulticastAddressRange> FindAllMulticastAddressRanges
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<MulticastAddressRange>
                (
                    Session: this,
                    Command: "show-multicast-address-ranges",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of MulticastAddressRanges</returns>
        public NetworkObjectsPagingResults<MulticastAddressRange> FindAllMulticastAddressRanges
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all networks.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Networks</returns>
        public NetworkObjectsPagingResults<Network> FindAllNetworks
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Network>
                (
                    Session: this,
                    Command: "show-networks",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Networks</returns>
        public NetworkObjectsPagingResults<Network> FindAllNetworks
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all simple gateways.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SimpleGateways</returns>
        public NetworkObjectsPagingResults<SimpleGateway> FindAllSimpleGateways
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateways",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SimpleGateways</returns>
        public NetworkObjectsPagingResults<SimpleGateway> FindAllSimpleGateways
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all security zones.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SecurityZones</returns>
        public NetworkObjectsPagingResults<SecurityZone> FindAllSecurityZones
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<SecurityZone>
                (
                    Session: this,
                    Command: "show-security-zones",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of SecurityZones</returns>
        public NetworkObjectsPagingResults<SecurityZone> FindAllSecurityZones
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all tags.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Tags</returns>
        public NetworkObjectsPagingResults<Tag> FindAllTags
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Tag>
                (
                    Session: this,
                    Command: "show-tags",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Tags</returns>
        public NetworkObjectsPagingResults<Tag> FindAllTags
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all time objects.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Times</returns>
        public NetworkObjectsPagingResults<Time> FindAllTimes
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<Time>
                (
                    Session: this,
                    Command: "show-times",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of Times</returns>
        public NetworkObjectsPagingResults<Time> FindAllTimes
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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
        /// Finds all time groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of TimeGroups</returns>
        public NetworkObjectsPagingResults<TimeGroup> FindAllTimeGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
                IOrder order = FindAll.Defaults.Order
            )
        {
            return FindAll.Invoke<TimeGroup>
                (
                    Session: this,
                    Command: "show-time-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order
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
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <returns>NetworkObjectsPagingResults of TimeGroups</returns>
        public NetworkObjectsPagingResults<TimeGroup> FindAllTimeGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                int offset = FindAll.Defaults.Offset,
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
                    Offset: offset,
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

        #endregion TimeGroup Methods

        #endregion Object Methods

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
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "task-id", taskID },
                { "details-level", DetailLevels.Full }
            };

            string jsonData = JsonConvert.SerializeObject(data, this.JsonFormatting);

            string result = this.Post("show-task", jsonData);

            JObject results = JsonConvert.DeserializeObject<JObject>(result);
            JArray array = (JArray)results.GetValue("tasks");

            Task[] tasks = JsonConvert.DeserializeObject<Task[]>(array.ToString(), new JsonSerializerSettings() { Converters = { new SessionConstructorConverter(this) } });

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
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "script-name", scriptName },
                { "script", script },
                { "args", args },
                { "targets", targets },
                { "comments", comments }
            };

            string jsonData = JsonConvert.SerializeObject(data, this.JsonFormatting);

            string result = this.Post("run-script", jsonData);

            JObject results = JsonConvert.DeserializeObject<JObject>(result);
            JArray array = (JArray)results.GetValue("tasks");

            Dictionary<string, string> dicResults = new Dictionary<string, string>();

            foreach (var r in array)
            {
                JObject j = r as JObject;
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
            IReadOnlyDictionary<string, string> results = RunScript(
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
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy },
                { "targets", targets },
                { "access", access },
                { "threat-prevention", threatPrevention },
                { "install-on-all-cluster-members-or-fail", installOnAllClusterMembersOrFail },
                { "prepare-only", prepareOnly },
                { "revision", revision }
            };

            string jsonData = JsonConvert.SerializeObject(data, this.JsonFormatting);

            string result = this.Post("install-policy", jsonData);

            JObject taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        /// <summary>
        /// Verifies the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns>Task ID</returns>
        public string VerifyPolicy(string policy)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy }
            };

            string jsonData = JsonConvert.SerializeObject(data, this.JsonFormatting);

            string result = this.Post("verify-policy", jsonData);

            JObject taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        #endregion Policy Methods

        #endregion Misc. Methods
    }
}