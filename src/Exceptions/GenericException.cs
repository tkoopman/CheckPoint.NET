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

using System;
using System.Net;

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
            Warnings = warnings;
            Errors = errors;
            BlockingErrors = blockingErrors;
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
        /// Returns the Message property when converted to a string.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Message;
        }

        #endregion Methods
    }
}