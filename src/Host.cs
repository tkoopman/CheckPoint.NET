﻿// MIT License
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
    public class Host : ObjectBase
    {
        #region Fields

        private ObjectMembershipChangeTracking<Group> _groups;
        private IPAddress _ipv4Address;
        private IPAddress _ipv6Address;
        private NATSettings _natSettings;

        #endregion Fields

        #region Constructors

        public Host(Session session) : this(session, DetailLevels.Full)
        {
        }

        protected internal Host(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new ObjectMembershipChangeTracking<Group>(this);
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        [JsonProperty(PropertyName = "ipv4-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4Address
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _ipv4Address : null;

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
            get => (TestDetailLevel(DetailLevels.Standard)) ? _ipv6Address : null;

            set
            {
                _ipv6Address = value;
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