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
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AddressRangeTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "TestAddressRange.NET";
        private static readonly IPAddress v4First = IPAddress.Parse("0.0.0.0");
        private static readonly IPAddress v4Last = IPAddress.Parse("255.255.255.255");

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task AddressRangeTest()
        {
            var a = new AddressRange(Session)
            {
                Name = Name,
                IPv4AddressFirst = v4First,
                IPv4AddressLast = v4Last
            };
            await a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Fast Update
            a = Session.UpdateAddressRange(Name);
            a.Comments = "Blah";
            await a.AcceptChanges();

            // Find
            a = await Session.FindAddressRange(Name);
            Assert.AreEqual("Blah", a.Comments);
            Assert.AreEqual(v4First, a.IPv4AddressFirst);
            Assert.AreEqual(v4Last, a.IPv4AddressLast);
            Assert.IsNotNull(a.NATSettings);

            // Delete
            await Session.DeleteAddressRange(Name);
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindAddressRanges(limit: 5, order: AddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindAllFiltered()
        {
            var a = await Session.FindAddressRanges(filter: "0.0.0.0", ipOnly: true, limit: 5, order: AddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task FindNotFound() => await Session.FindAddressRange("I Don't Exist!");

        #endregion Methods
    }
}