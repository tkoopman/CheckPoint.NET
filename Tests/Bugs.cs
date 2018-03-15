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
using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    /// <summary>
    /// These tests are to keep track of bugs I have found in the API. They are configured to pass
    /// testing while the bug is still there. This is so the build process continues. They should
    /// fail once the bug has been fixed on Check Point's side.
    /// </summary>
    /// <seealso cref="Tests.StandardTestsBase" />
    [TestClass]
    public class Bugs : StandardTestsBase
    {
        #region Methods

        /// <summary>
        /// Bug that shows no way to clear an IP address once one is set.
        /// </summary>
        [TestMethod]
        public void ClearIPProperty()
        {
            var a = Session.FindHost("DNS Server");
            a.IPv6Address = System.Net.IPAddress.Parse("fe80::1");
            a.AcceptChanges();
            a.IPv6Address = null;
            a.AcceptChanges();

            // Should be null
            Assert.IsNotNull(a.IPv6Address);

            // Maybe by using quotes and not null
            Session.Post("set-host", "{\"ipv6-address\": \"\", \"name\": \"DNS Server\"}");
            a = Session.FindHost("DNS Server");
            // Should be null
            Assert.IsNotNull(a.IPv6Address);
        }

        /// <summary>
        /// This is a copy of <see cref="HostTests.SetGroups" /> but because we are using the UID and
        /// not name it will fail. Once this has been fixed in an updated API this test should start
        /// to fail proving it is fixed.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InternalErrorException))]
        public void HostSetGroupUsingUID()
        {
            GlobalOptions.IdentifierForSetCalls = GlobalOptions.Identifier.UID;

            string Name = "Web Server";

            try
            {
                var a = Session.FindHost(Name);
                a.Groups.Clear();
                Assert.IsTrue(a.IsChanged);
                a.AcceptChanges();
                Assert.IsFalse(a.IsChanged);
                Assert.AreEqual(0, a.Groups.Count);
            }
            finally
            {
                GlobalOptions.IdentifierForSetCalls = GlobalOptions.Identifier.Name;
            }
        }

        /// <summary>
        /// This shows the bug when using posix values to set a date and time value.
        /// </summary>
        [TestMethod]
        public void NewTimeUsingPosix()
        {
            GlobalOptions.WriteTimeAs = GlobalOptions.TimeField.Posix;

            try
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

                // While these should be equal for some reason the posix value changes by 40 seconds
                // when sent to API. Yet when using ISO8601 the posix value returned by the API does
                // match what we are sending
                Assert.AreNotEqual(new System.DateTime(2018, 01, 01, 23, 50, 00), a.End);
            }
            finally
            {
                GlobalOptions.WriteTimeAs = GlobalOptions.TimeField.ISO8601;
            }
        }

        #endregion Methods
    }
}