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

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;

namespace Koopman.CheckPoint
{
    public class Time : ObjectBase
    {
        #region Constructors

        public Time(Session session) : base(session, DetailLevels.Full)
        {
        }

        protected internal Time(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
        }

        #endregion Constructors

        #region Fields

        private DateTime _end;
        private bool _endNever;
        private RecurrenceClass _recurrence;
        private DateTime _start;
        private bool _startNow;

        #endregion Fields

        #region Properties

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

        [JsonProperty(PropertyName = "hours-ranges")]
        public HourRanges HourRanges { get; set; } = new HourRanges();

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

        #region Classes

        [JsonConverter(typeof(EnumConverter), StringCases.NoChange)]
        public enum RecurrencePattern
        {
            Daily,
            Weekly,
            Monthly
        }

        public static class Order
        {
            #region Fields

            public readonly static IOrder NameAsc = new OrderAscending("name");
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        public class RecurrenceClass : ChangeTracking
        {
            #region Fields

            private string _month;
            private RecurrencePattern _pattern;
            private Days _weekdays;

            #endregion Fields

            #region Properties

            public SimpleListChangeTracking<string> Days { get; } = new SimpleListChangeTracking<string>();

            [JsonProperty(PropertyName = "month")]
            public string Month
            {
                get => _month;
                set
                {
                    _month = value;
                    OnPropertyChanged();
                }
            }

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

            [JsonProperty(PropertyName = "weekdays")]
            [JsonConverter(typeof(FlagsConverter))]
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