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
using System.Net;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    /// <summary>
    /// Simple Gateway Interface Settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.SimpleChangeTracking" />
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

        /// <summary>
        /// Gets or sets a value indicating whether anti spoofing is enabled.
        /// </summary>
        /// <value><c>true</c> if anti spoofing enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "anti-spoofing")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AntiSpoofing
        {
            get => _antiSpoofing;

            set
            {
                _antiSpoofing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Anti spoofing settings.
        /// </summary>
        [JsonProperty(PropertyName = "anti-spoofing-settings")]
        public AntiSpoofing AntiSpoofingSettings
        {
            get => _antiSpoofingSettings;
            set
            {
                _antiSpoofingSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv4 address.
        /// </summary>
        /// <value>The IPv4 address.</value>
        [JsonProperty(PropertyName = "ipv4-address", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4Address
        {
            get => _ipv4Address;

            set
            {
                _ipv4Address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv4 Mask Length.
        /// </summary>
        /// <value>The IPV4 mask length.</value>
        [JsonProperty(PropertyName = "ipv4-mask-length", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int IPv4MaskLength
        {
            get => _ipv4MaskLength;

            set
            {
                _ipv4MaskLength = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv4 network mask.
        /// </summary>
        /// <value>The IPv4 network mask.</value>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv4NetworkMask
        {
            get => Internal.SubnetMask.MaskLengthToSubnetMask(IPv4MaskLength);
            set => IPv4MaskLength = Internal.SubnetMask.SubnetMaskToMaskLength(value);
        }

        /// <summary>
        /// Gets or sets the IPv6 address.
        /// </summary>
        /// <value>The IPv6 address.</value>
        [JsonProperty(PropertyName = "ipv6-address", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressConverter))]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress IPv6Address
        {
            get => _ipv6Address;

            set
            {
                _ipv6Address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv6 network mask.
        /// </summary>
        /// <value>The IPv6 network mask.</value>
        [JsonProperty(PropertyName = "ipv6-mask-length", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int IPv6MaskLength
        {
            get => _ipv6MaskLength;

            set
            {
                _ipv6MaskLength = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        /// <value>The interface name.</value>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether interface is assigned to a security zone.
        /// </summary>
        /// <value><c>true</c> if security zone assigned; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "security-zone")]
        public bool SecurityZone => _securityZoneSettings != null;

        /// <summary>
        /// Gets or sets the security zone settings.
        /// </summary>
        /// <value>The security zone settings.</value>
        [JsonProperty(PropertyName = "security-zone-settings", NullValueHandling = NullValueHandling.Ignore)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISecurityZoneSettings SecurityZoneSettings
        {
            get => _securityZoneSettings;

            set
            {
                _securityZoneSettings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the topology.
        /// </summary>
        /// <value>The topology.</value>
        [JsonProperty(PropertyName = "topology")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Topology Topology
        {
            get => _topology;

            set
            {
                _topology = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the topology settings.
        /// </summary>
        /// <value>The topology settings.</value>
        [JsonProperty(PropertyName = "topology-settings")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public TopologySettings TopologySettings
        {
            get => _topologySettings;

            set
            {
                _topologySettings = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => Name;

        #endregion Methods
    }
}