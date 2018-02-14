using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Koopman.CheckPoint.Common
{
    [ImmutableObject(true)]
    public class TimeOfDay : IEquatable<TimeOfDay>, IComparable<TimeOfDay>
    {
        #region Fields

        private const byte MaxHour = 23;
        private const byte MaxMinute = 59;
        private const short MaxMinutes = 1439;

        #endregion Fields

        #region Constructors

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

        public TimeOfDay(int minutes)
        {
            if (minutes < 0 || minutes > 1439) throw new ArgumentOutOfRangeException(nameof(minutes), $"Minutes valid range is 0 - {MaxMinutes}");
            Minutes = (short)minutes;
        }

        public TimeOfDay(int hour, int minute)
        {
            if (hour < 0 || hour > MaxHour) throw new ArgumentOutOfRangeException(nameof(hour), $"Hour valid range is 0 - {MaxHour}");
            if (minute < 0 || minute > MaxMinute) throw new ArgumentOutOfRangeException(nameof(minute), $"Minute valid range is 0 - {MaxMinute}");
            Minutes = (short)(hour * 60 + minute);
        }

        #endregion Constructors

        #region Properties

        public byte Hour
        {
            get => (byte)(Minutes / 60);
        }

        public byte Minute
        {
            get => (byte)(Minutes % 60);
        }

        public short Minutes { get; }

        #endregion Properties

        #region Methods

        public static implicit operator short(TimeOfDay v)
        {
            return v.Minutes;
        }

        public static implicit operator TimeOfDay(int v)
        {
            return new TimeOfDay(v);
        }

        public static TimeOfDay operator -(TimeOfDay c1, int mins)
        {
            return new TimeOfDay(c1.Minutes - mins);
        }

        public static TimeOfDay operator -(TimeOfDay c1, short mins)
        {
            return new TimeOfDay(c1.Minutes - mins);
        }

        public static TimeOfDay operator -(TimeOfDay c1, TimeOfDay c2)
        {
            return new TimeOfDay(c1.Minutes - c2.Minutes);
        }

        public static TimeOfDay operator --(TimeOfDay c1)
        {
            return new TimeOfDay(c1.Minutes - 1);
        }

        public static bool operator !=(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes != c2.Minutes;
        }

        public static TimeOfDay operator +(TimeOfDay c1, int mins)
        {
            return new TimeOfDay(c1.Minutes + mins);
        }

        public static TimeOfDay operator +(TimeOfDay c1, short mins)
        {
            return new TimeOfDay(c1.Minutes + mins);
        }

        public static TimeOfDay operator +(TimeOfDay c1, TimeOfDay c2)
        {
            return new TimeOfDay(c1.Minutes + c2.Minutes);
        }

        public static TimeOfDay operator ++(TimeOfDay c1)
        {
            return new TimeOfDay(c1.Minutes + 1);
        }

        public static bool operator <(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes < c2.Minutes;
        }

        public static bool operator <=(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes <= c2.Minutes;
        }

        public static bool operator ==(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes == c2.Minutes;
        }

        public static bool operator >(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes > c2.Minutes;
        }

        public static bool operator >=(TimeOfDay c1, TimeOfDay c2)
        {
            return c1.Minutes >= c2.Minutes;
        }

        public int CompareTo(TimeOfDay other)
        {
            return Minutes.CompareTo(other.Minutes);
        }

        public bool Equals(TimeOfDay other)
        {
            return Minutes == other.Minutes;
        }

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

        public override int GetHashCode()
        {
            return Minutes.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0:D2}:{1:D2}", Hour, Minute);
        }

        #endregion Methods
    }
}