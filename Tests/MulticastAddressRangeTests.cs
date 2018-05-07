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
    public class MulticastAddressRangeTests : StandardTestsBase
    {
        #region Fields

        private static readonly string v6Filter = "ff05::1:3";
        private static readonly IPAddress v6First = IPAddress.Parse(v6Filter);
        private static readonly IPAddress v6Last = IPAddress.Parse(v6Filter);
        private static readonly string v6Name = "TestMulticastAddressRange.NET";

        #endregion Fields

        #region Methods

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task FindNotFound() => await Session.FindMulticastAddressRange("I Don't Exist!");

        [TestMethod]
        public async Task Finds()
        {
            var a = await Session.FindMulticastAddressRanges(limit: 5, order: MulticastAddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindsFiltered()
        {
            var a = await Session.FindMulticastAddressRanges(filter: v6Filter, ipOnly: true, limit: 5, order: MulticastAddressRange.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task MulticastAddressRangeTest()
        {
            // Create
            var a = new MulticastAddressRange(Session)
            {
                Name = v6Name,
                IPv6AddressFirst = v6First,
                IPv6AddressLast = v6Last
            };

            await a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Set
            a.Comments = "Blah";
            await a.AcceptChanges();
            Assert.AreEqual("Blah", a.Comments);

            // Fast Update
            a = Session.UpdateMulticastAddressRange(v6Name);
            a.Color = Colors.Purple;
            await a.AcceptChanges();

            // Find
            a = await Session.FindMulticastAddressRange(a.UID);
            Assert.AreEqual(Colors.Purple, a.Color);

            // Delete
            await a.Delete();
        }

        #endregion Methods
    }
}