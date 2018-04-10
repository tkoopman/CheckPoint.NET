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
using System.ComponentModel;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Time range between two TimeOfDay objects. Doesn't allow for Start being after End.
    /// </summary>
    [ImmutableObject(true)]
    public class TimeRange
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRange" /> class.
        /// </summary>
        /// <param name="start">The start TimeOfDay.</param>
        /// <param name="end">The end TimeOfDay.</param>
        /// <exception cref="ArgumentOutOfRangeException">end - End must be greater than start</exception>
        public TimeRange(TimeOfDay start, TimeOfDay end)
        {
            if (start > end) throw new ArgumentOutOfRangeException(nameof(end), "End must be greater than start");
            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRange" /> class.
        /// </summary>
        /// <param name="start">The start TimeOfDay.</param>
        /// <param name="end">The end TimeOfDay.</param>
        /// <exception cref="ArgumentOutOfRangeException">end - End must be greater than start</exception>
        public TimeRange(string start, string end)
        {
            var s = new TimeOfDay(start);
            var e = new TimeOfDay(end);
            if (s > e) throw new ArgumentOutOfRangeException(nameof(end), "End must be greater than start");
            Start = s;
            End = e;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public short Duration => (short)((short)End - (short)Start);

        /// <summary>
        /// Gets the end TimeOfDay.
        /// </summary>
        public TimeOfDay End { get; }

        /// <summary>
        /// Gets the start TimeOfDay.
        /// </summary>
        public TimeOfDay Start { get; }

        #endregion Properties
    }
}