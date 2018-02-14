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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    /// <summary>
    /// Summary description for SubnetMaskTest
    /// </summary>
    [TestClass]
    public class TimeOfDayTests
    {
        #region Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        [TestMethod]
        public void TimeTest1()
        {
            TimeOfDay t = 0;
            for (byte h = 0; h < 24; h++)
            {
                for (byte m = 0; m < 60; m++)
                {
                    Console.Out.WriteLine($"{t.ToString()}");

                    int mins = h * 60 + m;
                    var t1 = new TimeOfDay(h, m);
                    var t2 = new TimeOfDay(mins);
                    var t3 = new TimeOfDay($"{h}:{m}");

                    Assert.IsTrue(t == t1);
                    Assert.IsTrue(t == mins);

                    Assert.AreEqual(mins, t1.Minutes);
                    Assert.AreEqual(h, t1.Hour);
                    Assert.AreEqual(m, t1.Minute);

                    Assert.AreEqual(mins, t2.Minutes);
                    Assert.AreEqual(h, t2.Hour);
                    Assert.AreEqual(m, t2.Minute);

                    Assert.AreEqual(mins, t3.Minutes);
                    Assert.AreEqual(h, t3.Hour);
                    Assert.AreEqual(m, t3.Minute);

                    if (t.Minutes < 1439) t++;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TimeTest2()
        {
            TimeOfDay t2 = -1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TimeTest3()
        {
            var t2 = new TimeOfDay(24, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TimeTest4()
        {
            var t2 = new TimeOfDay(0);
            t2 -= 1;
        }

        #endregion Methods
    }
}