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
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Address Range Class.
    /// </summary>
    /// <example>
    /// Add new address range using <see cref="AddressRange.AddressRange(Session, bool)" />
    /// <code>
    /// var ar = new AddressRange(Session) {
    /// Name = "MyAddressRange",
    /// IPv4AddressFirst = IPAddress.Parse("10.1.1.1"),
    /// IPv4AddressLast = IPAddress.Parse("10.1.1.10")
    /// };
    /// ar.AcceptChanges();
    /// </code>
    /// Find address range using <see cref="Session.FindAddressRange(string, DetailLevels, CancellationToken)" />
    /// <code>
    /// var ar = Session.FindAddressRange("MyAddressRange");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.IGroupMember" />
    public class AddressRange : ObjectBase<AddressRange>, IGroupMember
    {
        #region Fields

        private MemberMembershipChangeTracking<Group> _groups;
        private IPAddress _ipv4AddressFirst;
        private IPAddress _ipv4AddressLast;
        private IPAddress _ipv6AddressFirst;
        private IPAddress _ipv6AddressLast;
        private NATSettings _natSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a new <see cref="AddressRange" />.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        /// <example>
        /// <code>
        /// var ar = new AddressRange(Session) {
        /// Name = "MyAddressRange",
        /// IPv4AddressFirst = IPAddress.Parse("10.1.1.1"),
        /// IPv4AddressLast = IPAddress.Parse("10.1.1.10")
        /// };
        /// ar.AcceptChanges();
        /// </code>
        /// </example>
        public AddressRange(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressRange" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal AddressRange(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<Group>(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Gets or sets the first IPv4 address in the range.
        /// </summary>
        /// <value>The first IPv4 address.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "ipv4-address-first")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4AddressFirst
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _ipv4AddressFirst : null;

            set
            {
                _ipv4AddressFirst = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the last IPv4 address in the range.
        /// </summary>
        /// <value>The last IPv4 address.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "ipv4-address-last")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4AddressLast
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _ipv4AddressLast : null;

            set
            {
                _ipv4AddressLast = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the first IPv6 address in the range.
        /// </summary>
        /// <value>The first IPv6 address.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "ipv6-address-first")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv6AddressFirst
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _ipv6AddressFirst : null;

            set
            {
                _ipv6AddressFirst = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the last IPv6 address in the range.
        /// </summary>
        /// <value>The last IPv6 address.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "ipv6-address-last")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv6AddressLast
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _ipv6AddressLast : null;

            set
            {
                _ipv6AddressLast = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the nat settings.
        /// </summary>
        /// <value>The nat settings.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "nat-settings")]
        public NATSettings NATSettings
        {
            get => (TestDetailLevel(DetailLevels.Full, DetailLevelActions.ReturnNull)) ? _natSettings : null;

            set
            {
                _natSettings = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public override ObjectType ObjectType => ObjectType.AddressRange;

        /// <inheritdoc />
        public override string Type => "address-range";

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
        /// Valid sort orders for Address Ranges
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