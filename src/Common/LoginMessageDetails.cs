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

using Newtonsoft.Json;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Login Message Full Details
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.LoginMessage" />
    public class LoginMessageDetails : LoginMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginMessageDetails" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="showMessage">if set to <c>true</c> show message.</param>
        /// <param name="warning">if set to <c>true</c> show warning.</param>
        [JsonConstructor]
        protected LoginMessageDetails(string header, string message, bool showMessage, bool warning) : base(header, message)
        {
            ShowMessage = showMessage;
            Warning = warning;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether login message should be shown at login.
        /// </summary>
        /// <value><c>true</c> if message shown; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "show-message")]
        public bool ShowMessage { get; }

        /// <summary>
        /// Gets a value indicating whether the warning sign should be added.
        /// </summary>
        /// <value><c>true</c> if warning sign added; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "warning")]
        public bool Warning { get; }

        #endregion Properties
    }
}