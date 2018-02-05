// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;

namespace Koopman.CheckPoint
{
    public class Session : IDisposable
    {
        #region Fields

        private HttpClient _httpClient = null;
        private bool _isDisposed = false;

        #endregion Fields

        #region Constructors

        public Session(CheckPointSessionOptions options, TextWriter debugWriter = null)
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

            string jsonData = JsonConvert.SerializeObject(data);

            string result = this.Post("login", jsonData);

            JsonConvert.PopulateObject(result, this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// <para type="description">API Server version.</para>
        /// </summary>
        [JsonProperty(PropertyName = "api-server-version")]
        public string APIServerVersion { get; private set; }

        public TextWriter DebugWriter { get; set; }

        /// <summary>
        /// <para type="description">Information about the available disk space on the management server.</para>
        /// </summary>
        [JsonProperty(PropertyName = "disk-space-message")]
        public string DiskSpaceMessage { get; private set; }

        /// <summary>
        /// <para type="description">True if this session is read only.</para>
        /// </summary>
        [JsonProperty(PropertyName = "read-only")]
        public bool ReadOnly { get; private set; }

        //TODO login-message
        /// <summary>
        /// <para type="description">Session expiration timeout in seconds.</para>
        /// </summary>
        [JsonProperty(PropertyName = "session-timeout")]
        public int SessionTimeout { get; private set; }

        /// <summary>
        /// <para type="description">Session unique identifier.</para>
        /// </summary>
        [JsonProperty(PropertyName = "sid")]
        public string SID { get; private set; }

        /// <summary>
        /// <para type="description">Timestamp when administrator last accessed the management server.</para>
        /// </summary>
        //[JsonProperty(PropertyName = "last-login-was-at")]
        // TODO public CheckPointTime LastLoginWasAt { get; private set; }
        /// <summary>
        /// <para type="description">True if this management server is in the standby mode.</para>
        /// </summary>
        [JsonProperty(PropertyName = "standby")]
        public bool Standby { get; private set; }

        /// <summary>
        /// <para type="description">Session object unique identifier. This identifier may be used in the discard API to discard changes that were made in this session, when administrator is working from another session, or in the 'switch-session' API.</para>
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; private set; }

        /// <summary>
        /// <para type="description">URL that was used to reach the API server.</para>
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string URL { get; private set; }

        internal CheckPointSessionOptions Options { get; private set; }

        #endregion Properties

        #region Session Methods

        public void ContinueSessionInSmartconsole()
        {
            Post("continue-session-in-smartconsole", "{}");
            Dispose();
        }

        public void Discard()
        {
            Post("discard", "{}");
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                ((IDisposable)_httpClient).Dispose();
                _httpClient = null;
            }

            _isDisposed = true;
        }

        public void Logout()
        {
            Post("logout", "{}");
            Dispose();
        }

        public void Publish()
        {
            Post("publish", "{}");
        }

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

        internal string Post(string command, string json)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Session", "This session has already been disposed!");
            }
            string result = null;

            if (DebugWriter != null)
            {
                DebugWriter.WriteLine($" Posting Command: {command} ".CenterString(60, '-'));
                DebugWriter.WriteLine(json);
            }

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            if (command != "login")
            {
                content.Headers.Add("X-chkp-sid", SID);
            }

            using (HttpResponseMessage response = GetHttpClient().PostAsync(command, content).Result)
            {
                result = response.Content.ReadAsStringAsync().Result;

                if (DebugWriter != null)
                {
                    DebugWriter.WriteLine($" HTTP response {response.StatusCode} ".CenterString(60, '-'));
                    DebugWriter.WriteLine(result);
                    DebugWriter.WriteLine("".CenterString(60, '-'));
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw CheckPointError.CreateException(result, response.StatusCode);
                }
            }

            return result;
        }

        internal void WriteDebug(string message)
        {
            DebugWriter?.WriteLine(message);
        }

        #endregion Session Methods

        #region Object Methods

        #region AddressRange Methods

        /// <summary>
        /// Deletes a address-range.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
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
        /// Finds all address-ranges.
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
        /// Finds all address-ranges that match filter.
        /// </summary>
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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

        /// <summary>
        /// Finds a address-range.
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

        #endregion AddressRange Methods

        #region Group Methods

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="session">The active session to management server.</param>
        /// <param name="value">The name or UID to delete.</param>
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
        /// <param name="session">The active session to management server.</param>
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
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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
        /// <param name="session">The active session to management server.</param>
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
        /// Deletes a group-with-exclusion.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
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
        /// Finds all group-with-exclusions.
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
        /// Finds all group-with-exclusions that match filter.
        /// </summary>
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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
        /// Finds a group-with-exclusion.
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
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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
        /// Deletes a multicast-address-range.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
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
        /// Finds all multicast-address-ranges.
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
        /// Finds all multicast-address-ranges that match filter.
        /// </summary>
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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
        /// Finds a multicast-address-range.
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
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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

        #region Tag Methods

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
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
        /// <param name="session">The active session to management server.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">if set to <c>true</c> will search objects by their IP address only, without involving the textual search.</param>
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

        #endregion Object Methods
    }
}