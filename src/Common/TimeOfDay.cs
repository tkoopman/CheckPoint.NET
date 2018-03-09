using System;
using System.ComponentModel;
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
            Regex re = new Regex(@"^(\d{1,2}):(\d{1,2})$");
            MatchCollection matches = re.Matches(time);
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
        public byte Hour
        {
            get => (byte)(Minutes / 60);
        }

        /// <summary>
        /// Gets the minute.
        /// </summary>
        public byte Minute
        {
            get => (byte)(Minutes % 60);
        }

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
        public static implicit operator short(TimeOfDay v)
        {
            return v.Minutes;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32" /> to <see cref="TimeOfDay" />.
        /// </summary>
        /// <param name="v">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TimeOfDay(int v)
        {
            return new TimeOfDay(v);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to subtract.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator -(TimeOfDay c1, int mins)
        {
            return new TimeOfDay(c1.Minutes - mins);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to subtract.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator -(TimeOfDay c1, short mins)
        {
            return new TimeOfDay(c1.Minutes - mins);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay to subtract.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator -(TimeOfDay c1, TimeOfDay c2)
        {
            return new TimeOfDay(c1.Minutes - c2.Minutes);
        }

        /// <summary>
        /// Implements the operator --.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator --(TimeOfDay c1)
        {
            return new TimeOfDay(c1.Minutes - 1);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes != c2.Minutes;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to add.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator +(TimeOfDay c1, int mins)
        {
            return new TimeOfDay(c1.Minutes + mins);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="mins">The number of minutes to add.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator +(TimeOfDay c1, short mins)
        {
            return new TimeOfDay(c1.Minutes + mins);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator +(TimeOfDay c1, TimeOfDay c2)
        {
            return new TimeOfDay(c1.Minutes + c2.Minutes);
        }

        /// <summary>
        /// Implements the operator ++.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static TimeOfDay operator ++(TimeOfDay c1)
        {
            return new TimeOfDay(c1.Minutes + 1);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes < c2.Minutes;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes <= c2.Minutes;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes == c2.Minutes;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes > c2.Minutes;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="c1">The TimeOfDay.</param>
        /// <param name="c2">The TimeOfDay.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes >= c2.Minutes;
        }

        /// <summary>
        /// Compares to another TimeOfDay.
        /// </summary>
        /// <param name="other">The other TimeOfDay.</param>
        /// <returns></returns>
        public int CompareTo(TimeOfDay other)
        {
            return Minutes.CompareTo(other.Minutes);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref>
        /// parameter; otherwise, false.
        /// </returns>
        public bool Equals(TimeOfDay other)
        {
            return Minutes == other.Minutes;
        }

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
            if (typeof(int).IsAssignableFrom(obj.GetType()))
            {
                return (int)obj == Minutes;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Minutes.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in format HH:mm.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0:D2}:{1:D2}", Hour, Minute);
        }

        #endregion Methods
    }
}