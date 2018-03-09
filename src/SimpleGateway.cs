﻿// MIT License
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
using Koopman.CheckPoint.Json;
using Koopman.CheckPoint.SimpleGatewaySettings;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Koopman.CheckPoint
{
    public class SimpleGateway : ObjectBase
    {
        #region Fields

        private bool _antiBot;
        private bool _antiVirus;
        private bool _applicationControl;
        private bool _contentAwareness;
        private bool _dynamicIP;
        private bool _firewall;
        private Firewall _firewallSettings;
        private ObjectMembershipChangeTracking<Group> _groups;
        private ListChangeTracking<Interface> _interfaces;
        private IPAddress _ipv4Address;
        private IPAddress _ipv6Address;
        private Logs _logsSettings;
        private string _osName;
        private bool _saveLogsLocally;
        private MembershipChangeTracking<string> _sendAlertsToServer;
        private MembershipChangeTracking<string> _sendLogsToBackupServer;
        private MembershipChangeTracking<string> _sendLogsToServer;
        private string _sicName;
        private string _sicState;
        private bool _threatEmulation;
        private bool _urlFiltering;
        private string _version;
        private bool _vpn;
        private VPN _vpnSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleGateway" /> class. This will create a
        /// new Simple Gateway once AcceptChanges called.
        /// </summary>
        /// <param name="session">The current session.</param>
        public SimpleGateway(Session session) : this(session, DetailLevels.Full)
        {
        }

        /// <summary>
        /// Used by <see cref="ObjectConverter" /> class when creating instance of existing Simple Gateway
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level.</param>
        protected internal SimpleGateway(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new ObjectMembershipChangeTracking<Group>(this);
            _sendAlertsToServer = new MembershipChangeTracking<string>(this);
            _sendLogsToBackupServer = new MembershipChangeTracking<string>(this);
            _sendLogsToServer = new MembershipChangeTracking<string>(this);
            _interfaces = new ListChangeTracking<Interface>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether Anti-Bot blade is enabled.
        /// </summary>
        /// <value><c>true</c> if anti-bot blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "anti-bot")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AntiBot
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _antiBot : false;

            set
            {
                _antiBot = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Anti-Virus blade is enabled.
        /// </summary>
        /// <value><c>true</c> if anti-virus blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "anti-virus")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AntiVirus
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _antiVirus : false;

            set
            {
                _antiVirus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Application Control blade is enabled.
        /// </summary>
        /// <value><c>true</c> if application control blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "application-control")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool ApplicationControl
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _applicationControl : false;

            set
            {
                _applicationControl = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Content Awareness blade is enabled.
        /// </summary>
        /// <value><c>true</c> if content awareness blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "content-awareness")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool ContentAwareness
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _contentAwareness : false;

            set
            {
                _contentAwareness = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dynamic ip used on this object.
        /// </summary>
        /// <value><c>true</c> if dynamic ip assigned; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "dynamic-ip")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool DynamicIP
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _dynamicIP : false;

            set
            {
                _dynamicIP = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Firewall blade is enabled.
        /// </summary>
        /// <value><c>true</c> if firewall blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "firewall")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool Firewall
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _firewall : false;

            set
            {
                _firewall = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the firewall settings.
        /// </summary>
        /// <value>The firewall settings.</value>
        [JsonProperty(PropertyName = "firewall-settings")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Firewall FirewallSettings
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _firewallSettings : null;

            set
            {
                _firewallSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the group membership.
        /// </summary>
        /// <value>The group membership.</value>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>The interfaces.</value>
        [JsonProperty(PropertyName = "interfaces")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ListChangeTracking<Interface> Interfaces
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _interfaces : null;

            private set => _interfaces = value;
        }

        /// <summary>
        /// Gets or sets the IPv4 address.
        /// </summary>
        /// <value>The IPv4 address.</value>
        [JsonProperty(PropertyName = "ipv4-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4Address
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _ipv4Address : null;

            set
            {
                _ipv4Address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv6 address.
        /// </summary>
        /// <value>The IPv6 address.</value>
        [JsonProperty(PropertyName = "ipv6-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv6Address
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _ipv6Address : null;

            set
            {
                _ipv6Address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the logs settings.
        /// </summary>
        /// <value>The logs settings.</value>
        [JsonProperty(PropertyName = "logs-settings")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Logs LogsSettings
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _logsSettings : null;

            set
            {
                _logsSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the gateway platform operating system.
        /// </summary>
        /// <value>The name of the os.</value>
        [JsonProperty(PropertyName = "os-name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string OSName
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _osName : null;

            set
            {
                _osName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether logs saved locally on the gateway.
        /// </summary>
        /// <value><c>true</c> if logs saved locally; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "save-logs-locally")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool SaveLogsLocally
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _saveLogsLocally : false;

            set
            {
                _saveLogsLocally = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the Server(s) to send alerts to.
        /// </summary>
        /// <value>The Server(s) to send alerts to.</value>
        [JsonProperty(PropertyName = "send-alerts-to-server")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> SendAlertsToServer
        {
            get => _sendAlertsToServer;
            internal set => _sendAlertsToServer = value;
        }

        /// <summary>
        /// Gets the Backup server(s) to send logs to.
        /// </summary>
        /// <value>The Backup server(s) to send logs to.</value>
        [JsonProperty(PropertyName = "send-logs-to-backup-server")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> SendLogsToBackupServer
        {
            get => _sendLogsToBackupServer;
            internal set => _sendLogsToBackupServer = value;
        }

        /// <summary>
        /// Gets the Servers(s) to send logs to.
        /// </summary>
        /// <value>The Servers(s) to send logs to.</value>
        [JsonProperty(PropertyName = "send-logs-to-server")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> SendLogsToServer
        {
            get => _sendLogsToServer;
            internal set => _sendLogsToServer = value;
        }

        /// <summary>
        /// Gets the Secure Internal Communication name.
        /// </summary>
        /// <value>The Secure Internal Communication name.</value>
        [JsonProperty(PropertyName = "sic-name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string SICName
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _sicName : null;

            private set => _sicName = value;
        }

        /// <summary>
        /// Gets the Secure Internal Communication state.
        /// </summary>
        /// <value>The Secure Internal Communication state.</value>
        [JsonProperty(PropertyName = "sic-state")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string SICState
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _sicState : null;

            private set => _sicState = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Threat Emulation blade is enabled.
        /// </summary>
        /// <value><c>true</c> if threat emulation blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "threat-emulation")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool ThreatEmulation
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _threatEmulation : false;

            set
            {
                _threatEmulation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether URL Filtering blade is enabled.
        /// </summary>
        /// <value><c>true</c> if URL filtering blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "url-filtering")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool URLFiltering
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _urlFiltering : false;

            set
            {
                _urlFiltering = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the gateway platform version.
        /// </summary>
        /// <value>The gateway platform version.</value>
        [JsonProperty(PropertyName = "version")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Version
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _version : null;

            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether VPN blade is enabled.
        /// </summary>
        /// <value><c>true</c> if VPN blade enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "vpn")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool VPN
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _vpn : false;

            set
            {
                _vpn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the VPN settings.
        /// </summary>
        /// <value>The VPN settings.</value>
        [JsonProperty(PropertyName = "vpn-settings")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public VPN VPNSettings
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _vpnSettings : null;

            set
            {
                _vpnSettings = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Classes

        public static class Order
        {
            #region Fields

            public readonly static IOrder NameAsc = new OrderAscending("name");
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        #endregion Classes
    }
}