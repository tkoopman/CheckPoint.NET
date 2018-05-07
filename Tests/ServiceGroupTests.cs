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
    public class ServiceGroupTests : StandardTestsBase
    {
        #region Fields

        private static readonly string[] Members = { "http", "domain-udp", "echo-request" };
        private static readonly string Name = "TestServiceGroup.NET";

        #endregion Fields

        #region Methods

        public static async Task<ServiceGroup> CreateTestGroup(Session session)
        {
            var a = new ServiceGroup(session)
            {
                Name = Name,
                Color = Colors.Red
            };
            foreach (string m in Members)
                a.Members.Add(m);
            await a.AcceptChanges();

            return a;
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindServiceGroups(limit: 5, order: ServiceGroup.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindServiceGroups(filter: "MS", limit: 5, order: ServiceGroup.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task ServiceGroupTest()
        {
            var a = await CreateTestGroup(Session);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(Members.Length, a.Members.Count);

            a.Members.Clear();
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);

            a.Members.Add("domain-udp");
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(1, a.Members.Count);

            // Fast Update
            a = Session.UpdateServiceGroup(Name);
            a.Members.Remove("domain-udp");
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Members.Count);

            // Find
            a = await Session.FindServiceGroup(Name);

            // Delete
            await a.Delete();
        }

        #endregion Methods
    }
}