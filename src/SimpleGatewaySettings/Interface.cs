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

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    public class Interface : SimpleChangeTracking
    {
        #region Fields

        private bool _antiSpoofing = true;
        private AntiSpoofing _antiSpoofingSettings;
        private IPAddress _ipv4Address;
        private int _ipv4MaskLength;
        private IPAddress _ipv6Address;
        private int _ipv6MaskLength;
        private string _name;
        private ISecurityZoneSettings _securityZoneSettings;
        private Topology _topology;
        private TopologySettings _topologySettings;

        #endregion Fields

        #region Properties

        [JsonProperty(PropertyName = "anti-spoofing")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AntiSpoofing
        {
            get => _antiSpoofing;

            set
            {
                _antiSpoofing = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "anti-spoofing-settings")]
        public AntiSpoofing AntiSpoofingSettings
        {
            get => _antiSpoofingSettings;
            set
            {
                _antiSpoofingSettings = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "ipv4-address", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4Address
        {
            get => _ipv4Address;

            set
            {
                _ipv4Address = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "ipv4-mask-length", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int IPv4MaskLength
        {
            get => _ipv4MaskLength;

            set
            {
                _ipv4MaskLength = value;
                IsChanged = true;
            }
        }

        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4NetworkMask
        {
            get => Internal.SubnetMask.MaskLengthToSubnetMask(IPv4MaskLength);

            set { IPv4MaskLength = Internal.SubnetMask.SubnetMaskToMaskLength(value); }
        }

        [JsonProperty(PropertyName = "ipv6-address", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv6Address
        {
            get => _ipv6Address;

            set
            {
                _ipv6Address = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "ipv6-mask-length", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int IPv6MaskLength
        {
            get => _ipv6MaskLength;

            set
            {
                _ipv6MaskLength = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "security-zone")]
        public bool SecurityZone
        {
            get => _securityZoneSettings != null;
        }

        [JsonProperty(PropertyName = "security-zone-settings", NullValueHandling = NullValueHandling.Ignore)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISecurityZoneSettings SecurityZoneSettings
        {
            get => _securityZoneSettings;

            set
            {
                _securityZoneSettings = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "topology")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Topology Topology
        {
            get => _topology;

            set
            {
                _topology = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "topology-settings")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public TopologySettings TopologySettings
        {
            get => _topologySettings;

            set
            {
                _topologySettings = value;
                IsChanged = true;
            }
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}