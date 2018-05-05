﻿// MIT License
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
    public class ApplicationSiteTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "mycompany.com";
        private static readonly string Name = "mycompany.com";

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task FastUpdate()
        {
            string set = $"Not_{Name}";
            var a = Session.UpdateApplicationSite(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public async Task Find()
        {
            var a = await Session.FindApplicationSite(Name);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindApplicationSites(limit: 5);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindApplicationSites(filter: Filter, limit: 5);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Total > 0);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task New()
        {
            string name = $"New_{Name}";

            var a = new ApplicationSite(Session)
            {
                Name = name,
                Color = Colors.Red,
                PrimaryCategory = "Custom_Application_Site"
            };

            a.UrlList.Add("www.purple.com");

            Assert.IsTrue(a.IsNew);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public async Task Set()
        {
            string set = $"Not_{Name}";
            var a = await Session.FindApplicationSite(Name);
            a.Name = set;
            a.UrlList.Add(Name);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        #endregion Methods
    }
}