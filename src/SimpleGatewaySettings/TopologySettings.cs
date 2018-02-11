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
using Newtonsoft.Json;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    public class TopologySettings : SimpleChangeTracking
    {
        #region Fields

        private bool _interfaceLeadsToDMZ;
        private TopologyBehind _ipAddressBehindThisInterface;
        private string _specificNetwork;

        #endregion Fields

        #region Properties

        [JsonProperty(PropertyName = "interface-leads-to-dmz")]
        public bool InterfaceLeadsToDMZ
        {
            get => _interfaceLeadsToDMZ;
            set
            {
                _interfaceLeadsToDMZ = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "ip-address-behind-this-interface")]
        public TopologyBehind IPAddressBehindThisInterface
        {
            get => _ipAddressBehindThisInterface;
            set
            {
                _ipAddressBehindThisInterface = value;
                IsChanged = true;
            }
        }

        [JsonProperty(PropertyName = "specific-network")]
        public string SpecificNetwork
        {
            get => _specificNetwork;
            set
            {
                _specificNetwork = value;
                IsChanged = true;
            }
        }

        #endregion Properties
    }
}