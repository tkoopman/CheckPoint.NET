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
using Newtonsoft.Json;
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Other Service
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.IServiceGroupMember" />
    public class ServiceOther : ObjectBase<ServiceOther>, IServiceGroupMember
    {
        #region Fields

        private bool? _acceptReplies;
        private string _action;
        private AggressiveAging _aggressiveAging;
        private MemberMembershipChangeTracking<ServiceGroup> _groups;
        private int? _ipProtocol;
        private bool? _keepConnectionsOpenAfterPolicyInstallation;
        private string _match;
        private bool? _matchForAny;
        private bool? _overrideDefaultSettings;
        private int? _sessionTimeout;
        private bool? _syncConnectionsOnCluster;
        private bool? _useDefaultSessionTimeout;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create new <see cref="ServiceOther" />.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public ServiceOther(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOther" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal ServiceOther(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<ServiceGroup>(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the accept replies indicator.
        /// </summary>
        /// <value><c>true</c> if replies should be accepted.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "accept-replies")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? AcceptReplies
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _acceptReplies : null;

            set
            {
                _acceptReplies = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains an INSPECT expression that defines the action to take if a rule containing this
        /// service is matched. Example: set r_mhandler &amp;open_ssl_handler sets a handler on the connection.
        /// </summary>
        /// <value>The action INSPECT expression.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "action")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Action
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _action : null;

            set
            {
                _action = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the short (aggressive) timeouts for idle connections.
        /// </summary>
        /// <value>The aggressive aging settings.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "aggressive-aging")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public AggressiveAging AggressiveAging
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _aggressiveAging : null;

            set
            {
                _aggressiveAging = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<ServiceGroup> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Gets or sets the IP protocol number.
        /// </summary>
        /// <value>The IP protocol number.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "ip-protocol")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int? IPProtocol
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _ipProtocol : null;

            set
            {
                _ipProtocol = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the keep connections open after policy installation.
        /// </summary>
        /// <value>The keep connections open after policy installation.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "keep-connections-open-after-policy-installation")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? KeepConnectionsOpenAfterPolicyInstallation
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _keepConnectionsOpenAfterPolicyInstallation : null;

            set
            {
                _keepConnectionsOpenAfterPolicyInstallation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains an INSPECT expression that defines the matching criteria. The connection is
        /// examined against the expression during the first packet. Example: tcp, dport = 21,
        /// direction = 0 matches incoming FTP control connections.
        /// </summary>
        /// <value>The match INSPECT expression.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "match")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Match
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _match : null;

            set
            {
                _match = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates whether this service is used when 'Any' is set as the rule's service and there
        /// are several service objects with the same source port and protocol.
        /// </summary>
        /// <value>The match for any value.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "match-for-any")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? MatchForAny
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _matchForAny : null;

            set
            {
                _matchForAny = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public override ObjectType ObjectType => ObjectType.ServiceOther;

        /// <summary>
        /// Indicates whether this service is a Data Domain service which has been overridden.
        /// </summary>
        /// <value>The override default settings.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "override-default-settings")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? OverrideDefaultSettings
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _overrideDefaultSettings : null;

            set
            {
                _overrideDefaultSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the session timeout (in seconds).
        /// </summary>
        /// <value>The session timeout (in seconds).</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "session-timeout")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int? SessionTimeout
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _sessionTimeout : null;

            set
            {
                _sessionTimeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Enables state-synchronized High Availability or Load Sharing on a ClusterXL or
        /// OPSEC-certified cluster.
        /// </summary>
        /// <value>The synchronize connections on cluster.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "sync-connections-on-cluster")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? SyncConnectionsOnCluster
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _syncConnectionsOnCluster : null;

            set
            {
                _syncConnectionsOnCluster = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public override string Type => "service-other";

        /// <summary>
        /// Gets or sets the use default session timeout.
        /// </summary>
        /// <value>The use default session timeout.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "use-default-session-timeout")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? UseDefaultSessionTimeout
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _useDefaultSessionTimeout : null;

            set
            {
                _useDefaultSessionTimeout = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        internal override void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            base.UpdateGenericMembers(objectConverter);
            Groups.UpdateGenericMembers(objectConverter);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Valid sort orders for UDP services
        /// </summary>
        public static class Order
        {
            #region Fields

            /// <summary>
            /// Sort by name in ascending order
            /// </summary>
            public readonly static IOrder NameAsc = new OrderAscending("name");

            /// <summary>
            /// Sort by name in descending order
            /// </summary>
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        #endregion Classes
    }
}