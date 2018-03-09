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

using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Net;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Network Object's NAT Settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    [JsonConverter(typeof(NATSettingsConverter))]
    public class NATSettings : ChangeTracking
    {
        #region Fields

        private bool _autoRule = false;
        private HideBehindValues _hideBehind;
        private string _installOn = "All";
        private IPAddress _iPv4Address;
        private IPAddress _iPv6Address;
        private NATMethods _method;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NATSettings" /> class.
        /// </summary>
        public NATSettings()
        {
        }

        #endregion Constructors

        #region Enums

        /// <summary>
        /// Hide Behind Gateway or IP
        /// </summary>
        [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, "-")]
        public enum HideBehindValues
        {
            /// <summary>
            /// Hide traffic behind the gateway's IP Address
            /// </summary>
            Gateway,

            /// <summary>
            /// Hide or Static NAT traffic behind specific IP Address
            /// </summary>
            IPAddress
        }

        /// <summary>
        /// NAT Method to use
        /// </summary>
        [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase)]
        public enum NATMethods
        {
            /// <summary>
            /// Hide NAT
            /// </summary>
            Hide,

            /// <summary>
            /// Static NAT
            /// </summary>
            Static
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets or sets whether to add automatic address translation rules
        /// </summary>
        [JsonProperty(PropertyName = "auto-rule")]
        public bool AutoRule
        {
            get => _autoRule;
            set
            {
                _autoRule = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the hide behind method.
        /// </summary>
        [JsonProperty(PropertyName = "hide-behind")]
        public HideBehindValues HideBehind
        {
            get => _hideBehind;
            set
            {
                _hideBehind = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets which gateway should apply the NAT translation.
        /// </summary>
        [JsonProperty(PropertyName = "install-on")]
        public string InstallOn
        {
            get => _installOn;
            set
            {
                _installOn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPv4 address for static NATs.
        /// </summary>
        [JsonProperty(PropertyName = "ipv4-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPv4Address
        {
            get => _iPv4Address;
            set
            {
                _iPv4Address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IPV6 address for static NATs.
        /// </summary>
        [JsonProperty(PropertyName = "ipv6-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPv6Address
        {
            get => _iPv6Address;
            set
            {
                _iPv6Address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the NAT translation method.
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        public NATMethods Method
        {
            get => _method;
            set
            {
                _method = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}