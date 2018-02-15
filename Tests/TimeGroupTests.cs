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
    public class TimeGroupTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Add = "Weekend";
        private static readonly string Name = "TimeGroup";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void Find()
        {
            var a = Session.FindTimeGroup(Name);
            Assert.IsNotNull(a);
            Assert.IsFalse(a.IsChanged);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllTimeGroups(limit: 5);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            string filter = Name.Substring(0, 3);

            var a = Session.FindAllTimeGroups(filter: filter, limit: 5);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void New()
        {
            string name = $"{Name}2";

            var a = new TimeGroup(Session)
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
            string set = $"{Name}2";

            var a = Session.FindTimeGroup(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public void SetMembers()
        {
            var a = Session.FindTimeGroup(Name);
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