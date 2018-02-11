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

        public SimpleGateway(Session session) : this(session, DetailLevels.Full)
        {
        }

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

        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        [JsonProperty(PropertyName = "interfaces")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ListChangeTracking<Interface> Interfaces
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _interfaces : null;

            private set => _interfaces = value;
        }

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

        [JsonProperty(PropertyName = "send-alerts-to-server")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> SendAlertsToServer
        {
            get => _sendAlertsToServer;
            internal set => _sendAlertsToServer = value;
        }

        [JsonProperty(PropertyName = "send-logs-to-backup-server")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> SendLogsToBackupServer
        {
            get => _sendLogsToBackupServer;
            internal set => _sendLogsToBackupServer = value;
        }

        [JsonProperty(PropertyName = "send-logs-to-server")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> SendLogsToServer
        {
            get => _sendLogsToServer;
            internal set => _sendLogsToServer = value;
        }

        [JsonProperty(PropertyName = "sic-name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string SICName
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _sicName : null;

            set
            {
                _sicName = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "sic-state")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string SICState
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _sicState : null;

            set
            {
                _sicState = value;
                OnPropertyChanged();
            }
        }

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