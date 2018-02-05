// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Koopman.CheckPoint;
using Koopman.CheckPoint.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Tests
{
    [TestClass]
    public class AddressRangeTests : StandardTestsBase
    {
        private static readonly string v4Name = "All_Internet";
        private static readonly IPAddress v4First = IPAddress.Parse("0.0.0.0");
        private static readonly IPAddress v4Last = IPAddress.Parse("255.255.255.255");
        private static readonly string v4Filter = "0.0.0.0";

        #region Methods

        [TestMethod]
        [ExpectedException(typeof(ObjectDeletionException))]
        public void Delete()
        {
            Session.DeleteAddressRange(v4Name);
        }

        [TestMethod]
        public void Find()
        {
            var a = Session.FindAddressRange(v4Name);
            Assert.IsNotNull(a);
            Assert.AreEqual(v4First, a.IPv4AddressFirst);
            Assert.AreEqual(v4Last, a.IPv4AddressLast);
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            Assert.AreEqual(v4Name, a.Name);
            Assert.IsFalse(a.IsChanged);
            Assert.IsNotNull(a.NATSettings);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllAddressRanges(limit: 5, order: AddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindAllAddressRanges(filter: v4Filter, ipOnly: true, limit: 5, order: AddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void FindNotFound()
        {
            Session.FindAddressRange("I Don't Exist!");
        }

        [TestMethod]
        public void New()
        {
            string name = $"New {v4Name}";

            var a = new AddressRange(Session)
            {
                Name = name,
                IPv4AddressFirst = v4First,
                IPv4AddressLast = v4Last
            };

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public void Set()
        {
            var a = Session.FindAddressRange(v4Name);
            a.Comments = "Blah";
            a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);
        }

        #endregion Methods
    }
}