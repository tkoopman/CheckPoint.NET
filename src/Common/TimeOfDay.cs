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
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Hours and Minutes of day. Not linked to any date or time-zone.
    /// </summary>
    /// <seealso cref="System.IEquatable{T}" />
    /// <seealso cref="System.IComparable{T}" />
    [ImmutableObject(true)]
    public class TimeOfDay : IEquatable<TimeOfDay>, IComparable<TimeOfDay>
    {
        #region Fields

        private const byte MaxHour = 23;
        private const byte MaxMinute = 59;
        private const short MaxMinutes = 1439;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeOfDay" /> class.
        /// </summary>
        /// <param name="time">The time in HH:mm format.</param>
        /// <exception cref="InvalidCastException">TimeOfDay</exception>
        /// <exception cref="ArgumentOutOfRangeException">hour or minute</exception>
        public TimeOfDay(string time)
        {
            var re = new Regex(@"^(\d{1,2}):(\d{1,2})$");
            var matches = re.Matches(time);
            if (matches.Count != 1) throw new InvalidCastException($"Cannot convert to {nameof(TimeOfDay)}");

            short hour = short.Parse(matches[0].Groups[1].Value);
            short minute = short.Parse(matches[0].Groups[2].Value);

            if (hour < 0 || hour > MaxHour) throw new ArgumentOutOfRangeException(nameof(hour), $"Hour valid range is 0 - {MaxHour}");
            if (minute < 0 || minute > MaxMinute) throw new ArgumentOutOfRangeException(nameof(minute), $"Minute valid range is 0 - {MaxMinute}");
            Minutes = (short)(hour * 60 + minute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeOfDay" /> class.
        /// </summary>
        /// <param name="minutes">The number of minutes past midnight.</param>
        /// <exception cref="ArgumentOutOfRangeException">minutes</exception>
        public TimeOfDay(int minutes)
        {
            if (minutes < 0 || minutes > 1439) throw new ArgumentOutOfRangeException(nameof(minutes), $"Minutes valid range is 0 - {MaxMinutes}");
            Minutes = (short)minutes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeOfDay" /> class.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <exception cref="ArgumentOutOfRangeException">hour or minute</exception>
        public TimeOfDay(int hour, int minute)
        {
            if (hour < 0 || hour > MaxHour) throw new ArgumentOutOfRangeException(nameof(hour), $"Hour valid range is 0 - {MaxHour}");
            if (minute < 0 || minute > MaxMinute) throw new ArgumentOutOfRangeException(nameof(minute), $"Minute valid range is 0 - {MaxMinute}");
            Minutes = (short)(hour * 60 + minute);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the hour.
        /// </summary>
        public byte Hour => (byte)(Minutes / 60);

        /// <summary>
        /// Gets the minute.
        /// </summary>
        public byte Minute => (byte)(Minutes % 60);

        /// <summary>
        /// Gets the number of minutes past midnight.
        /// </summary>
        public short Minutes { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs an implicit conversion from <see cref="TimeOfDay" /> to <see cref="System.Int16" />.
        /// </summary>
        /// <param name="v">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator short(TimeOfDay v) => v.Minutes;

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32" /> to <see cref="TimeOfDay" />.
        /// </summary>
        /// <param name="v">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TimeOfDay(int v) => new TimeOfDay(v);

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to subtract.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator -(TimeOfDay c1, int mins) => new TimeOfDay(c1.Minutes - mins);

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to subtract.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator -(TimeOfDay c1, short mins) => new TimeOfDay(c1.Minutes - mins);

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay to subtract.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator -(TimeOfDay c1, TimeOfDay c2) => new TimeOfDay(c1.Minutes - c2.Minutes);

        /// <summary>
        /// Implements the operator --.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator --(TimeOfDay c1) => new TimeOfDay(c1.Minutes - 1);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TimeOfDay c1, TimeOfDay c2) => c1.Minutes != c2.Minutes;

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to add.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator +(TimeOfDay c1, int mins) => new TimeOfDay(c1.Minutes + mins);

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to add.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator +(TimeOfDay c1, short mins) => new TimeOfDay(c1.Minutes + mins);

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator +(TimeOfDay c1, TimeOfDay c2) => new TimeOfDay(c1.Minutes + c2.Minutes);

        /// <summary>
        /// Implements the operator ++.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator ++(TimeOfDay c1) => new TimeOfDay(c1.Minutes + 1);

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(TimeOfDay c1, TimeOfDay c2) => c1.Minutes < c2.Minutes;

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(TimeOfDay c1, TimeOfDay c2) => c1.Minutes <= c2.Minutes;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TimeOfDay c1, TimeOfDay c2) => c1.Minutes == c2.Minutes;

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(TimeOfDay c1, TimeOfDay c2) => c1.Minutes > c2.Minutes;

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(TimeOfDay c1, TimeOfDay c2) => c1.Minutes >= c2.Minutes;

        /// <summary>
        /// Compares to another TimeOfDay.
        /// </summary>
        /// <param name="other">The other TimeOfDay.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(TimeOfDay other) => Minutes.CompareTo(other.Minutes);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref>
        /// parameter; otherwise, false.
        /// </returns>
        public bool Equals(TimeOfDay other) => Minutes == other.Minutes;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (typeof(int).GetTypeInfo().IsAssignableFrom(obj.GetType()))
                return (int)obj == Minutes;
            else
                return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode() => Minutes.GetHashCode();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in format HH:mm.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => string.Format("{0:D2}:{1:D2}", Hour, Minute);

        #endregion Methods
    }
}