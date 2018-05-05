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

        private static readonly string Add = "DNS Server";
        private static readonly string Name = "Corporate LANs";

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task FastUpdate()
        {
            string set = $"Not {Name}";

            var a = Session.UpdateGroup(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public async Task Find()
        {
            var a = await Session.FindGroup(Name);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Members.Count > 0);
            Assert.IsFalse(a.IsChanged);
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
        public async Task FindsFiltered()
        {
            string filter = Name.Substring(0, 3);

            var a = await Session.FindGroups(filter: filter, limit: 5, order: Group.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindUID()
        {
            var a = await Session.FindGroup(Name, DetailLevels.UID);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Members.Count > 0);
            Assert.IsFalse(a.IsChanged);
        }

        [TestMethod]
        public async Task New()
        {
            string name = $"New {Name}";

            var a = new Group(Session)
            {
                Name = name,
                Color = Colors.Red
            };

            Assert.IsTrue(a.IsNew);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public async Task Set()
        {
            string set = $"Not {Name}";

            var a = await Session.FindGroup(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public async Task SetMembers()
        {
            var a = await Session.FindGroup(Name);
            a.Members.Clear();
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);

            a.Members.Add(Add);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(1, a.Members.Count);

            a.Members.Remove(a.Members[0]);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);

            a.Members.Add(Add);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(1, a.Members.Count);

            a.Members.Clear();
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);
        }

        #endregion Methods
    }
}