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
    public class ServiceTCPTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "TestTCP.NET";

        #endregion Fields

        #region Methods

        public static async Task<ServiceTCP> CreateTestTCP(Session session)
        {
            var a = new ServiceTCP(session)
            {
                Name = Name,
                Color = Colors.Red,
                Port = "80",
                AggressiveAging = new Koopman.CheckPoint.Common.AggressiveAging()
                {
                    Enable = true
                }
            };

            await a.AcceptChanges(Ignore.Warnings);
            return a;
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindServicesTCP(limit: 5, order: ServiceTCP.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindServicesTCP(filter: "http", limit: 5, order: ServiceTCP.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task ServiceTCPTest()
        {
            var g = await ServiceGroupTests.CreateTestGroup(Session);

            // Create
            var a = await CreateTestTCP(Session);
            await a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Fast Update
            a = Session.UpdateServiceTCP(Name);
            a.Comments = "Blah";
            a.Groups.Add(g);
            await a.AcceptChanges(Ignore.Warnings);

            // Find
            a = await Session.FindServiceTCP(Name, DetailLevels.UID);
            Assert.AreEqual(1, a.Groups.Count);
            a.Groups.Clear();
            await a.AcceptChanges(Ignore.Warnings);

            // Delete
            await a.Delete(Ignore.Warnings);
        }

        #endregion Methods
    }
}