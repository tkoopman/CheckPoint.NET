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
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class HostTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "10.0.0.0/8";
        private static readonly string Group = "Corporate LANs";
        private static readonly IPAddress IP = IPAddress.Parse("4.3.2.1");
        private static readonly string Name = "InterfacesTestHost";

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task Delete() => await Session.DeleteHost(Name);

        [TestMethod]
        public async Task FastUpdate()
        {
            string set = $"Not {Name}";

            var a = Session.UpdateHost(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public async Task Find()
        {
            var a = await Session.FindHost(Name);
            Assert.IsNotNull(a);
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            Assert.AreEqual(Name, a.Name);
            Assert.AreEqual(Domain.Default, a.Domain);
            Assert.IsFalse(a.IsChanged);
            Assert.IsNotNull(a.NATSettings);
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindHosts(limit: 5, order: Host.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindHosts(filter: Filter, ipOnly: true, limit: 5, order: Host.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
            Assert.IsNotNull(a);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task FindNotFound() => await Session.FindHost("I Don't Exist!");

        [TestMethod]
        public async Task New()
        {
            string name = $"New {Name}";

            var a = new Host(Session, true)
            {
                Name = name,
                IPv4Address = IP
            };

            Assert.IsTrue(a.IsNew);
            await a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public async Task Set()
        {
            string set = $"Not {Name}";

            var a = await Session.FindHost(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public async Task SetGroups()
        {
            var a = await Session.FindHost(Name);
            a.Groups.Clear();
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Groups.Count);

            a.Groups.Add(Group);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(1, a.Groups.Count);
            Assert.AreEqual(DetailLevels.Standard, a.Groups[0].DetailLevel);
            await a.Groups[0].Reload(OnlyIfPartial: true);
            Assert.AreEqual(DetailLevels.Full, a.Groups[0].DetailLevel);

            a.Groups.Remove(a.Groups[0]);
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(0, a.Groups.Count);
        }

        #endregion Methods
    }
}