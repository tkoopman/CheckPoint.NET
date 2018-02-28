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
using Newtonsoft.Json;
using System;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    /// <summary>
    /// Simple Gateway VPN Settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class VPN : ChangeTracking
    {
        #region Fields

        private int _maximumConcurrentIKENegotiations;
        private int _maximumConcurrentTunnels;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the maximum concurrent ike negotiations.
        /// </summary>
        /// <value>The maximum concurrent ike negotiations.</value>
        [JsonProperty(PropertyName = "maximum-concurrent-ike-negotiations")]
        public int MaximumConcurrentIKENegotiations
        {
            get => _maximumConcurrentIKENegotiations;
            set
            {
                _maximumConcurrentIKENegotiations = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the maximum concurrent tunnels.
        /// </summary>
        /// <value>The maximum concurrent tunnels.</value>
        [JsonProperty(PropertyName = "maximum-concurrent-tunnels")]
        public int MaximumConcurrentTunnels
        {
            get => _maximumConcurrentTunnels;
            set
            {
                _maximumConcurrentTunnels = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        /// <exception cref="NotImplementedException">Use AcceptChanges from Parent Object.</exception>
        public override void AcceptChanges()
        {
            throw new NotImplementedException("Use AcceptChanges from Parent Object.");
        }

        #endregion Methods
    }
}