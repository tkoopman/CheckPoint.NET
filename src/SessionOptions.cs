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

namespace Koopman.CheckPoint
{
    public class SessionOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether SSL certificate should be valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if certificate validation enabled otherwise, <c>false</c>.
        /// </value>
        public bool CertificateValidation { get; set; } = true;

        /// <summary>
        /// Gets or sets the action to be taken when current detail level is too low.
        /// </summary>
        /// <value>
        /// The detail level action to take.
        /// </value>
        public DetailLevelActions DetailLevelAction { get; set; } = DetailLevelActions.ThrowException;

        public bool IndentJson { get; set; } = false;

        /// <summary>
        /// Gets or sets the management server to connect to.
        /// </summary>
        /// <value>
        /// The management server IP or host name.
        /// </value>
        public string ManagementServer { get; set; }

        //TODO SecureString: But only if implemented correctly when reading to send in Login request
        /// <summary>
        /// Gets or sets the administrator's password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the port to connect to.
        /// </summary>
        /// <value>
        /// The port number.
        /// </value>
        public int Port { get; set; } = 443;

        /// <summary>
        /// Gets or sets a value indicating whether [read only] connection should be made.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read only otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Gets or sets the administrator's user name.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public string User { get; set; }

        // Doesn't look like Check Point support HTTP compression with API
        // So for now force disable
        internal bool Compression { get; set; } = false;

        #endregion Properties

        //TODO public int SessionTimeout { get; set; } = 600;
    }
}