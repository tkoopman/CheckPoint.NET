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
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Network Object
    /// </summary>
    /// <example>
    /// Add new network using <see cref="Network.Network(Session)" />
    /// <code>
    /// var n = new Network(Session) {
    ///     Name = "MyNetwork",
    ///     Subnet4 = IPAddress.Parse("10.0.0.0"),
    ///     MaskLength4 = 24
    /// };
    /// n.AcceptChanges();
    /// </code>
    /// Find network using <see cref="Session.FindNetwork(string, DetailLevels)" />
    /// <code>
    /// var n = Session.FindHost("MyNetwork");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase" />
    public class Network : ObjectBase
    {
        #region Fields

        private bool _broadcast;
        private ObjectMembershipChangeTracking<Group> _groups;
        private int _maskLength4 = -1;
        private int _maskLength6 = -1;
        private NATSettings _natSettings;
        private IPAddress _subnet4;
        private IPAddress _subnet6;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create new <see cref="Network" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var n = new Network(Session) {
        ///     Name = "MyNetwork",
        ///     Subnet4 = IPAddress.Parse("10.0.0.0"),
        ///     MaskLength4 = 24
        /// };
        /// n.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        public Network(Session session) : this(session, DetailLevels.Full)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Network" /> class ready to be populated with
        /// current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal Network(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new ObjectMembershipChangeTracking<Group>(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets is broadcast address is included.
        /// </summary>
        /// <value>The broadcast inclusion.</value>
        /// <exception cref="ArgumentNullException">BroadcastInclusion</exception>
        [JsonProperty(PropertyName = "broadcast")]
        [JsonConverter(typeof(CustomBoolConverter), "allow", "disallow")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? BroadcastInclusion
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _broadcast : (bool?)null;

            set
            {
                _broadcast = value ?? throw new ArgumentNullException(nameof(BroadcastInclusion));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Gets or sets the mask length for IPv4.
        /// </summary>
        /// <value>The mask length for IPv4.</value>
        [JsonProperty(PropertyName = "mask-length4")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int MaskLength4
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _maskLength4 : -1;

            set
            {
                _maskLength4 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the mask length for IPv6.
        /// </summary>
        /// <value>The mask length for IPv6.</value>
        [JsonProperty(PropertyName = "mask-length6")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int MaskLength6
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _maskLength6 : -1;

            set
            {
                _maskLength6 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the NAT settings.
        /// </summary>
        [JsonProperty(PropertyName = "nat-settings")]
        public NATSettings NATSettings
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _natSettings : null;

            set
            {
                _natSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv4 network address.
        /// </summary>
        /// <value>The IPv4 network address.</value>
        [JsonProperty(PropertyName = "subnet4")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress Subnet4
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _subnet4 : null;

            set
            {
                _subnet4 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv6 network address.
        /// </summary>
        /// <value>The IPv6 network address.</value>
        [JsonProperty(PropertyName = "subnet6")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress Subnet6
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _subnet6 : null;

            set
            {
                _subnet6 = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv4 subnet mask.
        /// </summary>
        /// <value>The subnet mask.</value>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress SubnetMask
        {
            get => (TestDetailLevel(DetailLevels.Standard) && _maskLength4 != -1) ? Internal.SubnetMask.MaskLengthToSubnetMask(_maskLength4) : null;

            set { MaskLength4 = Internal.SubnetMask.SubnetMaskToMaskLength(value); }
        }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Valid sort orders for Networks
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