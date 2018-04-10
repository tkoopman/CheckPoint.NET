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

using System;
using System.Net;
using System.Text;

namespace Koopman.CheckPoint.Exceptions
{
    /// <summary>
    /// Generic exception when an unknown error code or invalid JSON data is returned
    /// </summary>
    /// <remarks>Should not be thrown normally.</remarks>
    /// <seealso cref="System.Exception" />
    public class GenericException : Exception
    {
        #region Constructors

        internal GenericException(string message, HttpStatusCode httpStatusCode, CheckPointErrorCodes code, CheckPointErrorDetails[] warnings, CheckPointErrorDetails[] errors, CheckPointErrorDetails[] blockingErrors) : base(message)
        {
            HTTPStatusCode = httpStatusCode;
            Code = code;
            Warnings = warnings ?? new CheckPointErrorDetails[] { };
            Errors = errors ?? new CheckPointErrorDetails[] { };
            BlockingErrors = blockingErrors ?? new CheckPointErrorDetails[] { };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// <para type="description">Validation blocking-errors.</para>
        /// </summary>
        public CheckPointErrorDetails[] BlockingErrors { get; }

        /// <summary>
        /// <para type="description">Validation errors.</para>
        /// </summary>
        public CheckPointErrorDetails[] Errors { get; }

        /// <summary>
        /// Gets the HTTP status code returned by the management server.
        /// </summary>
        /// <value>The HTTP status code.</value>
        public HttpStatusCode HTTPStatusCode { get; }

        /// <summary>
        /// <para type="description">Validation warnings.</para>
        /// </summary>
        public CheckPointErrorDetails[] Warnings { get; }

        /// <summary>
        /// <para type="description">Error code.</para>
        /// </summary>
        internal CheckPointErrorCodes Code { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns the Message property and any Check Point error or warning details.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => ToString(true);

        /// <summary>
        /// Returns the Message property and optionally any Check Point error or warning details.
        /// </summary>
        /// <param name="includeDetails">if set to <c>true</c> includes error and warning details.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(bool includeDetails)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Message);
            if (includeDetails)
            {
                foreach (var be in BlockingErrors) sb.AppendLine($"Blocking Error: {be}");
                foreach (var e in Errors) sb.AppendLine($"Error: {e}");
                foreach (var w in Warnings) sb.AppendLine($"Warning: {w}");
            }

            return sb.ToString().TrimEnd('\r', '\n');
        }

        #endregion Methods
    }
}