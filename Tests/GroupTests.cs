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
using Koopman.CheckPoint.FastUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class GroupTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "TestGroup.NET";

        #endregion Fields

        #region Methods

        public static async Task<Group> CreateTestGroup(Session session)
        {
            var a = new Group(session)
            {
                Name = Name,
                Color = Colors.Red
            };

            await a.AcceptChanges();
            return a;
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindAllGroups(limit: 5, order: Group.Order.NameAsc);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public async Task Finds()
        {
            var a = await Session.FindGroups(limit: 5, order: Group.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task GroupTest()
        {
            // Create Group
            var a = await CreateTestGroup(Session);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(0, a.Members.Count);

            // Add member
            var host = await HostTests.CreateTestHost(Session);
            a.Members.Add(host);
            await a.AcceptChanges();
            Assert.AreEqual(1, a.Members.Count);

            // Remove Member Fast Update
            a = Session.UpdateGroup(Name);
            a.Members.Remove(host.Name);
            await a.AcceptChanges();
            Assert.AreEqual(0, a.Members.Count);

            // Find by name
            a = await Session.FindGroup(Name);
            a.Members.Add(host.UID);
            await a.AcceptChanges();

            host = await host.Reload(false, DetailLevels.UID);
            Assert.AreEqual(a.UID, host.Groups[0].UID);

            // Find by UID
            a = await Session.FindGroup(a.UID, DetailLevels.UID);
            Assert.AreEqual(Name, a.Name);
            Assert.AreEqual(1, a.Members.Count);
            a.Members.Clear();
            await a.AcceptChanges();
            Assert.AreEqual(0, a.Members.Count);

            // Delete group
            await a.Delete();
        }

        #endregion Methods
    }
}