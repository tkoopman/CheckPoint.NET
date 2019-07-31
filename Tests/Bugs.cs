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
using System.Threading.Tasks;

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
        public async Task ClearIPProperty()
        {
            var a = await HostTests.CreateTestHost(Session);
            a.IPv6Address = null;
            await a.AcceptChanges();

            // Should be null
            Assert.IsNotNull(a.IPv6Address);

            // Maybe by using quotes and not null
            await Session.PostAsync("set-host", $"{{\"ipv6-address\": \"\", \"name\": \"{a.Name}\"}}", default);
            a = await Session.FindHost(a.Name);
            // Should be null
            Assert.IsNotNull(a.IPv6Address);
        }

        /// <summary>
        /// Bug that shows unable to clear all tags by sending empty array. Had also tried sending ""
        /// but that fails with errors. Also tried null but that did same as empty array.
        /// </summary>
        [TestMethod]
        public async Task ClearTags()
        {
            var a = await HostTests.CreateTestHost(Session);
            Assert.AreEqual(1, a.Tags.Count);

            a.Tags.Clear();
            await a.AcceptChanges();
            // Should equal 0
            Assert.AreEqual(1, a.Tags.Count);
        }

        /// <summary>
        /// In API version 1.1 we expect this to fail but should work from version 1.3 onwards.
        /// </summary>
        [TestMethod]
        public async Task FindAppByID()
        {
            ApplicationSite a = null;
            try
            {
                a = await Session.FindApplicationSite("Check Point User Center");
            }
            catch (ObjectNotFoundException)
            {
                Assert.Fail("Failed to find application.");
            }

            // ObjectNotFound error returned when it should be found
            try
            {
                a = await Session.FindApplicationSite((int)a.ApplicationID);
            }
            catch (ObjectNotFoundException)
            {
                if (Session.APIVersion > 1.1F)
                    Assert.Fail("Failed to find application by ID.");
            }
            Assert.IsNotNull(a);
        }

        /// <summary>
        /// This is a copy of <see cref="HostTests.SetGroups" /> but because we are using the UID and
        /// not name it will fail. Once this has been fixed in an updated API this test should start
        /// to fail proving it is fixed.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InternalErrorException))]
        public async Task HostSetGroupUsingUID()
        {
            // Fist do it the working way
            var a = await HostTests.CreateTestHost(Session);
            a.Groups.Clear();
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Groups.Count);

            GlobalOptions.IdentifierForSetCalls = GlobalOptions.Identifier.UID;

            try
            {
                // Now this way will fail
                a.Groups.Clear();
                Assert.IsTrue(a.IsChanged);
                await a.AcceptChanges();
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
        public async Task NewTimeUsingPosix()
        {
            //First working way.
            var a = new Time(Session)
            {
                Name = "ISOTest"
            };

            a.HourRanges[0] = new TimeRange(new TimeOfDay("03:00"), new TimeOfDay("04:00"));
            a.StartNow = true;
            a.Start = new System.DateTime(2019, 01, 01, 00, 00, 00);
            a.EndNever = false;
            a.End = new System.DateTime(2018, 01, 01, 23, 50, 00);

            Assert.IsTrue(a.IsNew);
            await a.AcceptChanges();

            Assert.AreEqual(new System.DateTime(2018, 01, 01, 23, 50, 00), a.End);

            // Now not working way
            GlobalOptions.WriteTimeAs = GlobalOptions.TimeField.Posix;

            try
            {
                a = new Time(Session)
                {
                    Name = "PosixTest",
                    Color = Colors.Red
                };

                a.HourRanges[0] = new TimeRange(new TimeOfDay("03:00"), new TimeOfDay("04:00"));
                a.StartNow = true;
                a.Start = new System.DateTime(2019, 01, 01, 00, 00, 00);
                a.EndNever = false;
                a.End = new System.DateTime(2018, 01, 01, 23, 50, 00);

                Assert.IsTrue(a.IsNew);
                await a.AcceptChanges();

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