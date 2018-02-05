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
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Koopman.CheckPoint
{
    public class AddressRange : ObjectBase
    {
        #region Fields

        private MembershipChangeTracking<Group> _groups = new MembershipChangeTracking<Group>();
        private IPAddress _ipv4AddressFirst;
        private IPAddress _ipv4AddressLast;
        private IPAddress _ipv6AddressFirst;
        private IPAddress _ipv6AddressLast;
        private NATSettings _natSettings;

        #endregion Fields

        #region Constructors

        public AddressRange(Session session) : base(session, DetailLevels.Full)
        {
        }

        protected internal AddressRange(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

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