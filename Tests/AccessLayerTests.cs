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
    public class AccessLayerTests : StandardTestsBase
    {
        #region Fields

        private static readonly Colors color = Colors.Red;
        private static readonly string name = $"TestLayer.NET";

        #endregion Fields

        #region Methods

        public static async Task<AccessLayer> CreateTestAccessLayer(Session session)
        {
            var a = new AccessLayer(session, true)
            {
                Name = name,
                Color = color,
                Firewall = true,
                ApplicationsAndUrlFiltering = true
            };
            await a.AcceptChanges();

            return a;
        }

        [TestMethod]
        public async Task AccessLayerTest()
        {
            // Create Layer
            var a = await CreateTestAccessLayer(Session);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(name, a.Name);
            Assert.AreEqual(color, a.Color);

            // Find Layer by name
            a = await Session.FindAccessLayer(name);
            Assert.IsNotNull(a);
            Assert.AreEqual(name, a.Name);
            Assert.AreEqual(color, a.Color);

            // Find Layer by UID
            a = await Session.FindAccessLayer(a.UID);
            Assert.IsNotNull(a);
            Assert.AreEqual(name, a.Name);
            Assert.AreEqual(color, a.Color);

            // Set Layer
            a.Comments = name;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(name, a.Comments);

            // Fast Update
            Assert.IsFalse((bool)a.ContentAwareness);
            a = Session.UpdateAccessLayer(name);
            a.ContentAwareness = true;
            await a.AcceptChanges();
            Assert.IsTrue((bool)a.ContentAwareness);

            // Delete
            await Session.DeleteAccessLayer(name);
        }

        [TestMethod]
        public async Task FindAllLayers()
        {
            var a = await Session.FindAllAccessLayers(limit: 5, detailLevel: DetailLevels.Standard);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public async Task FindFilteredLayers()
        {
            var a = await Session.FindAccessLayers("Layer", limit: 5, order: AccessLayer.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        #endregion Methods
    }
}