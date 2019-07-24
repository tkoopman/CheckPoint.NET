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
using Koopman.CheckPoint.FastUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class HostTests : StandardTestsBase
    {
        #region Fields

        private static readonly IPAddress IPv4 = IPAddress.Parse("4.3.2.1");
        private static readonly IPAddress IPv6 = IPAddress.Parse("fe80::1");
        private static readonly string Name = "TestHost.NET";

        #endregion Fields

        #region Methods

        public static async Task<Host> CreateTestHost(Session session)
        {
            var a = new Host(session, true)
            {
                Name = Name,
                IPv4Address = IPv4,
                IPv6Address = IPv6
            };

            a.Tags.Add("CheckPoint.NET");

            await a.AcceptChanges(Ignore.Warnings);

            return a;
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindHosts(limit: 5, order: Host.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindHosts(filter: "10.0.0.0/8", ipOnly: true, limit: 5, order: Host.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllMemberships()
        {
            // Create Group
            var group = await GroupTests.CreateTestGroup(Session);

            // Add member
            var host = await CreateTestHost(Session);
            group.Members.Add(host);
            await group.AcceptChanges();

            var a = await Session.FindHosts(limit: 5, order: Host.Order.NameDesc, detailLevel: DetailLevels.Full, showMembership: true);

            int count = 0;
            foreach (var h in a)
            {
                if (h.Groups.Count > 0 && count == 0)
                {
                    /// v1.1 Returns standard detail level on memberships, v1.3 Returns just UIDs
                    var member = h.Groups[0];
                    if (member.DetailLevel == DetailLevels.UID)
                        member = await member.Reload();
                    Assert.AreNotEqual(DetailLevels.UID, member.DetailLevel);
                }
                count += h.Groups.Count;
            }

            Assert.AreNotEqual(0, count);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task FindNotFound() => await Session.FindHost("I Don't Exist!");

        [TestMethod]
        public async Task HostTest()
        {
            // Create Host
            var a = await CreateTestHost(Session);
            await a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(Name, a.Name);

            // Find host by name
            a = await Session.FindHost(Name);
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            Assert.AreEqual(Domain.Default, a.Domain);
            Assert.IsFalse(a.IsChanged);
            Assert.IsNotNull(a.NATSettings);

            // Group Membership
            var g = await GroupTests.CreateTestGroup(Session);

            a.Groups.Add(g);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(1, a.Groups.Count);
            Assert.AreEqual(DetailLevels.Standard, a.Groups[0].DetailLevel);
            await a.Groups[0].Reload(OnlyIfPartial: true);
            Assert.AreEqual(DetailLevels.Full, a.Groups[0].DetailLevel);

            a.Groups.Clear();
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Groups.Count);

            a = Session.UpdateHost(Name); // Test Fast Update
            a.Groups.Add(g);
            await a.AcceptChanges();
            a.Groups.Remove(a.Groups[0]);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Groups.Count);

            // Delete
            await Session.DeleteHost(Name);
        }

        #endregion Methods
    }
}