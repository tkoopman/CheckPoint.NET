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
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class NetworkTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "TestNetwork.NET";
        private static readonly int v4MaskLen = 24;
        private static readonly IPAddress v4Subnet = IPAddress.Parse("172.16.10.0");
        private static readonly int v6MaskLen = 64;
        private static readonly IPAddress v6Subnet = IPAddress.Parse("fe80::");

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindAllNetworks(limit: 5, order: Network.Order.NameDesc, detailLevel: DetailLevels.UID);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task FindNotFound() => await Session.FindNetwork("I Don't Exist!");

        [TestMethod]
        public async Task FindsFull()
        {
            var a = await Session.FindNetworks(limit: 5, order: Network.Order.NameDesc, detailLevel: DetailLevels.Full);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindsUIDs()
        {
            var a = await Session.FindNetworks(limit: 5, order: Network.Order.NameDesc, detailLevel: DetailLevels.Standard);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task NetworkTest()
        {
            // Create
            var a = new Network(Session)
            {
                Name = Name,
                Subnet4 = v4Subnet,
                SubnetMask = SubnetMask.MaskLengthToSubnetMask(v4MaskLen),
                Subnet6 = v6Subnet,
                MaskLength6 = v6MaskLen,
                BroadcastInclusion = false
            };
            await a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Find
            a = await Session.FindNetwork(Name);

            // Set
            a.Comments = "Blah";
            await a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);
            Assert.AreEqual(v4MaskLen, a.MaskLength4);

            // Delete
            await a.Delete();
        }

        #endregion Methods
    }
}