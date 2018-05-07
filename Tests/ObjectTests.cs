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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ObjectTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public async Task ExportWhereUsed()
        {
            var wu = await Session.FindWhereUsed("domain-udp");
            var export = new JsonExport(Session, excludeByName: new string[] { "domain-tcp" });
            await export.AddAsync("domain-udp", wu);
            Console.Out.WriteLine(await export.Export(true));
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindAllObjects(filter: "domain", detailLevel: DetailLevels.UID);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Length > 0);
            var b = await Session.FindObject(a[0].UID);
            Assert.IsNotNull(b);
            Assert.AreEqual(a[0].UID, b.UID);
        }

        [TestMethod]
        public async Task Finds()
        {
            var a = await Session.FindObjects(limit: 5);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Total > 0);
            var b = await Session.FindObject(a[0].UID);
            Assert.IsNotNull(b);
            Assert.AreEqual(a[0].UID, b.UID);
        }

        [TestMethod]
        public async Task Unused()
        {
            var a = await Session.FindUnusedObjects(limit: 5, detailLevel: DetailLevels.UID);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Total > 0);
        }

        [TestMethod]
        public async Task WhereUsed()
        {
            var a = await Session.FindWhereUsed("domain-udp", DetailLevels.UID);
            Assert.IsNotNull(a);
        }

        #endregion Methods
    }
}