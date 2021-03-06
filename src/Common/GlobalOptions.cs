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

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// These global options will affect ALL sessions.
    /// </summary>
    public static class GlobalOptions
    {
        #region Enums

        /// <summary>
        /// Which identifier should be used
        /// </summary>
        public enum Identifier
        {
            /// <summary>
            /// Use UID
            /// </summary>
            UID,

            /// <summary>
            /// Use name
            /// </summary>
            Name
        }

        /// <summary>
        /// Which time field should be used
        /// </summary>
        public enum TimeField
        {
            /// <summary>
            /// Use posix
            /// </summary>
            Posix,

            /// <summary>
            /// Use ISO8601
            /// </summary>
            ISO8601
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets or sets the identifier for set calls. When ever a post for a set method is sent to
        /// identify which object is being updated either name or UID need to be sent.
        /// </summary>
        /// <remarks>
        /// NOTE: Currently have noticed a bug in Check Point PI if using UID with some operations
        ///       like clearing group memberships.
        /// </remarks>
        /// <value>The identifier for set calls.</value>
        public static Identifier IdentifierForSetCalls { get; set; } = Identifier.Name;

        /// <summary>
        /// Gets or sets the field / format to send date time fields in when posting to the Check
        /// Point API.
        /// </summary>
        /// <remarks>NOTE: have noticed bug if you use Posix currently.</remarks>
        /// <value>The format to write in.</value>
        public static TimeField WriteTimeAs { get; set; } = TimeField.ISO8601;

        #endregion Properties
    }
}