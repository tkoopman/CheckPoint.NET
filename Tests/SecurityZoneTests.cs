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
    public class SecurityZoneTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "DMZ";
        private static readonly string Name = "TestZone.NET";

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task Finds()
        {
            var a = await Session.FindSecurityZones(limit: 5, order: Tag.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindsFiltered()
        {
            var a = await Session.FindSecurityZones(filter: Filter, limit: 5, order: Tag.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task ZoneTest()
        {
            // Create
            var a = new SecurityZone(Session)
            {
                Name = Name,
                Color = Colors.Red
            };

            await a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Fast Update
            a = Session.UpdateSecurityZone(Name);
            a.Comments = "Blah";
            await a.AcceptChanges();

            // Find
            a = await Session.FindSecurityZone(Name);
            Assert.AreEqual("Blah", a.Comments);

            // Delete
            await a.Delete();
        }

        #endregion Methods
    }
}