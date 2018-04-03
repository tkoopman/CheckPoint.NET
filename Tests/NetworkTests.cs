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
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.FastUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Tests
{
    [TestClass]
    public class NetworkTests : StandardTestsBase
    {
        #region Fields

        private static readonly string v4Filter = "172.16.0.0/16";
        private static readonly int v4MaskLen = 24;
        private static readonly string v4Name = "CP_default_Office_Mode_addresses_pool";
        private static readonly IPAddress v4Subnet = IPAddress.Parse("172.16.10.0");
        private static readonly int v6MaskLen = 64;
        private static readonly string v6Name = "IPv6_Link_Local_Hosts";
        private static readonly IPAddress v6Subnet = IPAddress.Parse("fe80::");

        #endregion Fields

        #region Methods

        [TestMethod]
        public void Delete()
        {
            Session.DeleteNetwork(v4Name);
        }

        [TestMethod]
        public void FastUpdate()
        {
            var a = Session.UpdateNetwork(v4Name);
            a.Comments = "Blah";
            a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);
        }

        [TestMethod]
        public void Find()
        {
            var a = Session.FindNetwork(v4Name);
            Assert.IsNotNull(a);
            Assert.AreEqual(v4Subnet, a.Subnet4);
            Assert.AreEqual(v4MaskLen, a.MaskLength4);
            Assert.IsTrue((bool)a.BroadcastInclusion);
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            Assert.AreEqual(v4Name, a.Name);
            Assert.AreEqual(Domain.Default, a.Domain);
            Assert.IsFalse(a.IsChanged);
            Assert.IsNotNull(a.NATSettings);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindNetworks(limit: 5, order: Network.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindNetworks(filter: v4Filter, ipOnly: true, limit: 5, order: Network.Order.NameDesc);
            Assert.IsNotNull(a);
            Network b = a[0].Reload();
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllUIDs()
        {
            var a = Session.FindNetworks(limit: 5, order: Network.Order.NameDesc, detailLevel: DetailLevels.UID);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void FindNotFound()
        {
            Session.FindNetwork("I Don't Exist!");
        }

        [TestMethod]
        public void FindUID()
        {
            var a = Session.FindNetwork("HQ LAN", DetailLevels.UID);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void FindV6()
        {
            var a = Session.FindNetwork(v6Name);
            Assert.IsNotNull(a);
            Assert.AreEqual(v6Subnet, a.Subnet6);
            Assert.AreEqual(v6MaskLen, a.MaskLength6);
            Assert.IsNull(a.Subnet4);
            Assert.AreEqual(-1, a.MaskLength4);
            Assert.IsNull(a.SubnetMask);
        }

        [TestMethod]
        public void New()
        {
            string name = $"New {v4Name}";

            var a = new Network(Session)
            {
                Name = name,
                Subnet4 = IPAddress.Parse("10.0.0.0"),
                SubnetMask = IPAddress.Parse("255.0.0.0"),
                BroadcastInclusion = false
            };

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public void Set()
        {
            var a = Session.FindNetwork(v4Name);
            a.Comments = "Blah";
            a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);
        }

        #endregion Methods
    }
}