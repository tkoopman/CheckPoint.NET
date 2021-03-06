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

namespace Koopman.CheckPoint.Exceptions
{
    /// <summary>
    /// Thrown when current detail level of object is not high enough to complete current operation.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DetailLevelException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailLevelException" /> class.
        /// </summary>
        /// <param name="actual">The actual detail level.</param>
        /// <param name="required">The required detail level.</param>
        public DetailLevelException(DetailLevels actual, DetailLevels required) :
            base(message: $"Detail level of {actual.ToString()} does not meet requirement of {required.ToString()}")
        {
            Actual = actual;
            Required = required;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the current detail level of the object.
        /// </summary>
        /// <value>The actual detail level currently</value>
        public DetailLevels Actual { get; }

        /// <summary>
        /// Gets the required detail level for the call to be successful.
        /// </summary>
        /// <value>The required detail level.</value>
        public DetailLevels Required { get; }

        #endregion Properties
    }
}