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
    public class TagTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "gw";
        private static readonly string Host = "DNS Server";
        private static readonly string Name = "Europe_gw";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void Find()
        {
            Tag a = Session.FindTag(Name);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllTags(limit: 5, order: Tag.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindAllTags(filter: Filter, limit: 5, order: Tag.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void New()
        {
            string name = $"New {Name}";

            Tag a = new Tag(Session)
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
            var a = Session.FindTag(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public void SetTags()
        {
            var a = Session.FindHost(Host);
            a.Tags.Clear();
            a.Tags.Add(Name);
            a.AcceptChanges();
            Assert.AreEqual(DetailLevels.Standard, a.Tags[0].DetailLevel);
            a.Tags[0].Reload();
            Assert.AreEqual(DetailLevels.Full, a.Tags[0].DetailLevel);
        }

        #endregion Methods
    }
}