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

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Time Object class
    /// </summary>
    /// <example>
    /// Add new time object using <see cref="Time.Time(Session, bool)" />
    /// <code>
    /// var t = new Time(Session)
    /// {
    /// Name = "MyTime",
    /// Color = Colors.Red
    /// };
    /// t.HourRanges[0] = new Koopman.CheckPoint.Common.TimeRange("03:00", "04:00");
    /// t.HourRanges[1] = new Koopman.CheckPoint.Common.TimeRange("15:00", "16:00");
    /// t.Recurrence = new Time.RecurrenceClass() {
    /// Pattern = Time.RecurrencePattern.Daily,
    /// Weekdays = Days.Saturday | Days.Sunday
    /// };
    /// t.AcceptChanges();
    /// </code>
    /// Find time object using <see cref="Session.FindTime(string, DetailLevels)" />
    /// <code>
    /// var t = Session.FindTime("MyTime");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.ITimeGroupMember" />
    public class Time : ObjectBase<Time>, ITimeGroupMember
    {
        #region Constructors

        /// <summary>
        /// Create new <see cref="Time" /> object.
        /// </summary>
        /// <example>
        /// <code>
        /// var t = new Time(Session)
        /// {
        ///     Name = "MyTime",
        ///     Color = Colors.Red
        /// };
        ///
        /// t.HourRanges[0] = new Koopman.CheckPoint.Common.TimeRange("03:00", "04:00");
        /// t.HourRanges[1] = new Koopman.CheckPoint.Common.TimeRange("15:00", "16:00");
        ///
        /// t.Recurrence = new Time.RecurrenceClass() {
        ///     Pattern = Time.RecurrencePattern.Daily,
        ///     Weekdays = Days.Weekend
        /// };
        ///
        /// t.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public Time(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time" /> class ready to be populated with
        /// current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal Time(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<TimeGroup>(this);
        }

        #endregion Constructors

        #region Fields

        private DateTime _end;
        private bool _endNever;
        private MemberMembershipChangeTracking<TimeGroup> _groups;
        private RecurrenceClass _recurrence;
        private DateTime _start;
        private bool _startNow;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the End time. Note: Each gateway may interpret this time differently
        /// according to its time zone.
        /// </summary>
        /// <value>The end time.</value>
        [JsonProperty(PropertyName = "end")]
        [JsonConverter(typeof(CheckPointDateTimeConverter), true)]
        public DateTime End
        {
            get => _end;
            set
            {
                _end = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this Time object should never end.
        /// </summary>
        /// <value><c>true</c> if end never; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "end-never")]
        public bool EndNever
        {
            get => _endNever;
            set
            {
                _endNever = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the time groups.
        /// </summary>
        /// <value>The time groups.</value>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<TimeGroup> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Gets or sets the Hours recurrence. Note: Each gateway may interpret this time differently
        /// according to its time zone.
        /// </summary>
        /// <value>The hour ranges.</value>
        [JsonProperty(PropertyName = "hours-ranges")]
        public HourRanges HourRanges { get; set; } = new HourRanges();

        /// <summary>
        /// Gets or sets the days recurrence.
        /// </summary>
        /// <value>The days recurrence.</value>
        [JsonProperty(PropertyName = "recurrence")]
        public RecurrenceClass Recurrence
        {
            get => _recurrence;
            set
            {
                _recurrence = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Starting time. Note: Each gateway may interpret this time differently
        /// according to its time zone.
        /// </summary>
        /// <value>The starting time.</value>
        [JsonProperty(PropertyName = "start")]
        [JsonConverter(typeof(CheckPointDateTimeConverter), true)]
        public DateTime Start
        {
            get => _start;
            set
            {
                _start = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to start now.
        /// </summary>
        /// <value><c>true</c> if start now; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "start-now")]
        public bool StartNow
        {
            get => _startNow;
            set
            {
                _startNow = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        internal override void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            base.UpdateGenericMembers(objectConverter);
            Groups.UpdateGenericMembers(objectConverter);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Days Recurrence Options
        /// </summary>
        [JsonConverter(typeof(EnumConverter))]
        public enum RecurrencePattern
        {
            /// <summary>
            /// Repeat daily
            /// </summary>
            Daily,

            /// <summary>
            /// Repeat weekly
            /// </summary>
            Weekly,

            /// <summary>
            /// Repeat monthly
            /// </summary>
            Monthly
        }

        /// <summary>
        /// Valid sort orders for Hosts
        /// </summary>
        public static class Order
        {
            #region Fields

            /// <summary>
            /// Sort by name in ascending order
            /// </summary>
            public readonly static IOrder NameAsc = new OrderAscending("name");

            /// <summary>
            /// Sort by name in descending order
            /// </summary>
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        /// <summary>
        /// Days recurrence details
        /// </summary>
        /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
        public class RecurrenceClass : ChangeTracking
        {
            #region Fields

            private Months _month;
            private RecurrencePattern _pattern;
            private Days _weekdays;

            #endregion Fields

            #region Properties

            /// <summary>
            /// Gets the specific days it recurres on.
            /// </summary>
            /// <value>Single days, or range. "1", "3", "9-20"</value>
            [JsonProperty(PropertyName = "days")]
            public SimpleListChangeTracking<string> Days { get; } = new SimpleListChangeTracking<string>();

            /// <summary>
            /// Gets or sets the valid month.
            /// </summary>
            /// <value>The valid month.</value>
            [JsonProperty(PropertyName = "month")]
            [JsonConverter(typeof(EnumConverter))]
            public Months Month
            {
                get => _month;
                set
                {
                    _month = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the recurrence pattern.
            /// </summary>
            /// <value>The recurrence pattern.</value>
            [JsonProperty(PropertyName = "pattern")]
            public RecurrencePattern Pattern
            {
                get => _pattern;
                set
                {
                    _pattern = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the valid weekdays.
            /// </summary>
            /// <value>The valid weekdays.</value>
            [JsonProperty(PropertyName = "weekdays")]
            public Days Weekdays
            {
                get => _weekdays;
                set
                {
                    _weekdays = value;
                    OnPropertyChanged();
                }
            }

            #endregion Properties
        }

        #endregion Classes
    }
}