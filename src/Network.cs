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
using System;
using System.Diagnostics;
using System.Net;

namespace Koopman.CheckPoint
{
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

        public Network(Session session) : this(session, DetailLevels.Full)
        {
        }

        protected internal Network(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new ObjectMembershipChangeTracking<Group>(this);
        }

        #endregion Constructors

        #region Properties

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

        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

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

        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress SubnetMask
        {
            get => (TestDetailLevel(DetailLevels.Standard) && _maskLength4 != -1) ? Internal.SubnetMask.MaskLengthToSubnetMask(_maskLength4) : null;

            set { MaskLength4 = Internal.SubnetMask.SubnetMaskToMaskLength(value); }
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