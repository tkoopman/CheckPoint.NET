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
        public void FastUpdate()
        {
            string set = $"Not {Name}";

            var a = Session.UpdateGroup(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public void Find()
        {
            var a = Session.FindGroup(Name);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Members.Count > 0);
            Assert.IsFalse(a.IsChanged);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllGroups(limit: 5, order: Group.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            string filter = Name.Substring(0, 3);

            var a = Session.FindAllGroups(filter: filter, limit: 5, order: Group.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void New()
        {
            string name = $"New {Name}";

            var a = new Group(Session)
            {
                Name = name,
                Color = Colors.Red
            };

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public void Set()
        {
            string set = $"Not {Name}";

            var a = Session.FindGroup(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public void SetMembers()
        {
            var a = Session.FindGroup(Name);
            a.Members.Clear();
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);

            a.Members.Add(Add);
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(1, a.Members.Count);

            a.Members.Remove(a.Members[0]);
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);
        }

        #endregion Methods
    }
}