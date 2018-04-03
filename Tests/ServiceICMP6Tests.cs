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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ServiceICMP6Tests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "echo";
        private static readonly string Name = "echo-request6";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void Find()
        {
            var a = Session.FindServiceICMP6(Name);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindServicesICMP6(limit: 5, order: ServiceICMP.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindServicesICMP6(filter: Filter, limit: 5, order: ServiceICMP.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void New()
        {
            string name = $"New{Name}";

            var a = new ServiceICMP6(Session)
            {
                Name = name,
                Color = Colors.Red,
                ICMPCode = 0,
                ICMPType = 8
            };

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        #endregion Methods
    }
}