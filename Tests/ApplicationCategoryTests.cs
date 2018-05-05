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
    public class ApplicationCategoryTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "Facebook";
        private static readonly string Name = "Custom_Application_Site";

        #endregion Fields

        #region Methods

        [TestMethod]
        [ExpectedException(typeof(Koopman.CheckPoint.Exceptions.ObjectLockedException))]
        public async Task FastUpdate()
        {
            string set = $"Not_{Name}";
            var a = Session.UpdateApplicationCategory(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
        }

        [TestMethod]
        public async Task Find()
        {
            var a = await Session.FindApplicationCategory(Name);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindApplicationCategories(limit: 5);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindApplicationCategories(filter: Filter, limit: 5);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task New()
        {
            string name = $"New_{Name}";

            var a = new ApplicationCategory(Session)
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
        [ExpectedException(typeof(Koopman.CheckPoint.Exceptions.ObjectLockedException))]
        public async Task Set()
        {
            string set = $"Not_{Name}";
            var a = await Session.FindApplicationCategory(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
        }

        #endregion Methods
    }
}