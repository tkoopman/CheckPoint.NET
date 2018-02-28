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

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    /// <summary>
    /// Simple Gateway Interface Security Zone Settings
    /// </summary>
    public static class SecurityZone
    {
        #region Classes

        /// <summary>
        /// Used when Security Zone is calculated according to where the interface leads to.
        /// </summary>
        /// <seealso cref="Koopman.CheckPoint.Common.ISecurityZoneSettings" />
        public class AutoCalculated : ISecurityZoneSettings
        {
            #region Properties

            [JsonProperty(PropertyName = "auto-calculated")]
            private bool Value { get => true; }

            #endregion Properties
        }

        /// <summary>
        /// Used when Security Zone specified manually
        /// </summary>
        /// <seealso cref="Koopman.CheckPoint.Common.ISecurityZoneSettings" />
        public class SpecificZone : ISecurityZoneSettings
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name of the Security Zone.
            /// </summary>
            /// <value>The Security Zone name.</value>
            [JsonProperty(PropertyName = "specific-zone")]
            public string Name { get; set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Name;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}