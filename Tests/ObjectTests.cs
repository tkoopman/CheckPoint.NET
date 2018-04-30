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
using System;

namespace Tests
{
    [TestClass]
    public class ObjectTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public void ExportWhereUsed()
        {
            string a = Session.ExportWhereUsed(new string[] { "domain-udp" });
            Assert.IsNotNull(a);
            Console.Out.WriteLine(a);
        }

        [TestMethod]
        public void ExportRulebase()
        {
            string a = Session.ExportRulebase("TestLayer");
            Assert.IsNotNull(a);
            Console.Out.WriteLine(a);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllObjects(filter: "domain", detailLevel: DetailLevels.Full);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Length > 0);
            var b = Session.FindObject(a[0].UID);
            Assert.IsNotNull(b);
            Assert.AreEqual(a[0].UID, b.UID);
        }

        [TestMethod]
        public void Finds()
        {
            var a = Session.FindObjects(limit: 5);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Total > 0);
            var b = Session.FindObject(a[0].UID);
            Assert.IsNotNull(b);
            Assert.AreEqual(a[0].UID, b.UID);
        }

        [TestMethod]
        public void Unused()
        {
            var a = Session.FindUnusedObjects(limit: 5, detailLevel: DetailLevels.Full);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Total > 0);
        }

        [TestMethod]
        public void WhereUsed()
        {
            var a = Session.FindWhereUsed("domain-udp", DetailLevels.Full);
            Assert.IsNotNull(a);
        }

        #endregion Methods
    }
}