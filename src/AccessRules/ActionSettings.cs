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
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Special;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Extra action settings for rule
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class ActionSettings : ChangeTracking
    {
        #region Fields

        private bool _enableIdentityCaptivePortal;
        private CpmiAppfwLimit _limit;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether identity captive portal is enabled for this rule.
        /// </summary>
        /// <value><c>true</c> if enable identity captive portal; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "enable-identity-captive-portal")]
        public bool EnableIdentityCaptivePortal
        {
            get => _enableIdentityCaptivePortal;
            set
            {
                _enableIdentityCaptivePortal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the limit object assigned to this rule.
        /// </summary>
        /// <value>The limit.</value>
        [JsonProperty(PropertyName = "limit")]
        public CpmiAppfwLimit Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the limit by name or UID.
        /// </summary>
        /// <param name="identifier">The name or UID of Limit object.</param>
        public void SetLimit(string identifier)
        {
            var l = new CpmiAppfwLimit(null, DetailLevels.Full);
            if (identifier.IsUID())
                l.UID = identifier;
            else
                l.Name = identifier;

            Limit = l;
        }

        #endregion Methods
    }
}