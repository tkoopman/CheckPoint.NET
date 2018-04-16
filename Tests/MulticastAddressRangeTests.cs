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
using Koopman.CheckPoint.FastUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Tests
{
    [TestClass]
    public class MulticastAddressRangeTests : StandardTestsBase
    {
        #region Fields

        private static readonly string v6Filter = "ff05::1:3";
        private static readonly IPAddress v6First = IPAddress.Parse(v6Filter);
        private static readonly IPAddress v6Last = IPAddress.Parse(v6Filter);
        private static readonly string v6Name = "All_DHCPv6_Servers";

        #endregion Fields

        #region Methods

        [TestMethod]
        [ExpectedException(typeof(GenericException))]
        public void Delete() => Session.DeleteMulticastAddressRange(v6Name);

        [TestMethod]
        [ExpectedException(typeof(ObjectLockedException))]
        public void FastUpdate()
        {
            var a = Session.UpdateMulticastAddressRange(v6Name);
            a.Comments = "Blah";
            a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);
        }

        [TestMethod]
        public void Find()
        {
            var a = Session.FindMulticastAddressRange(v6Name);
            Assert.IsNotNull(a);
            Assert.AreEqual(v6First, a.IPv6AddressFirst);
            Assert.AreEqual(v6Last, a.IPv6AddressLast);
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            Assert.AreEqual(v6Name, a.Name);
            Assert.IsFalse(a.IsChanged);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindMulticastAddressRanges(limit: 5, order: MulticastAddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindMulticastAddressRanges(filter: v6Filter, ipOnly: true, limit: 5, order: MulticastAddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void FindNotFound() => Session.FindMulticastAddressRange("I Don't Exist!");

        [TestMethod]
        public void New()
        {
            string name = $"New {v6Name}";

            var a = new AddressRange(Session)
            {
                Name = name,
                IPv6AddressFirst = v6First,
                IPv6AddressLast = v6Last
            };

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectLockedException))]
        public void Set()
        {
            var a = Session.FindMulticastAddressRange(v6Name);
            a.Comments = "Blah";
            a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);
        }

        #endregion Methods
    }
}