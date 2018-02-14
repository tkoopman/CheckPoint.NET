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

using Koopman.CheckPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TimeTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "Day";
        private static readonly string Name = "Off_Work";
        private static readonly string SetName = "Temp Access";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void Find()
        {
            var a = Session.FindTime(Name);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllTimes(limit: 5, order: Time.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindAllTimes(filter: Filter, limit: 5, order: Time.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void New()
        {
            string name = $"NoTime";

            var a = new Time(Session)
            {
                Name = name,
                Color = Colors.Red
            };

            a.HourRanges[0] = new Koopman.CheckPoint.Common.TimeRange(new Koopman.CheckPoint.Common.TimeOfDay("03:00"), new Koopman.CheckPoint.Common.TimeOfDay("04:00"));
            a.StartNow = true;
            a.Start = new System.DateTime(2019, 01, 01, 00, 00, 00);
            a.EndNever = false;
            a.End = new System.DateTime(2018, 01, 01, 23, 50, 00);

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public void Set()
        {
            string set = $"NoTime";
            var a = Session.FindTime(SetName);
            a.HourRanges[0] = new Koopman.CheckPoint.Common.TimeRange(new Koopman.CheckPoint.Common.TimeOfDay("03:00"), new Koopman.CheckPoint.Common.TimeOfDay("04:00"));
            a.AcceptChanges();
            a.Name = set;
            a.Start = new System.DateTime(2018, 01, 01, 00, 00, 00);
            a.End = new System.DateTime(2018, 01, 01, 23, 50, 00);
            a.Recurrence.Pattern = Time.RecurrencePattern.Weekly;
            a.Recurrence.Weekdays = Days.Mon | Days.Wed | Days.Fri;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
            Assert.AreEqual(Days.Mon | Days.Wed | Days.Fri, a.Recurrence.Weekdays);
        }

        #endregion Methods
    }
}