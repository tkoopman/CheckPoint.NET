﻿// MIT License
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
    public class ServiceUDPTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "domain";
        private static readonly string Name = "domain-udp";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void Find()
        {
            var a = Session.FindServiceUDP(Name);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllServicesUDP(limit: 5, order: ServiceUDP.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindAllServicesUDP(filter: Filter, limit: 5, order: ServiceUDP.Order.NameAsc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void New()
        {
            string name = $"New{Name}";

            var a = new ServiceUDP(Session)
            {
                Name = name,
                Color = Colors.Red,
                Port = "53",
                AggressiveAging = new Koopman.CheckPoint.Common.AggressiveAging()
                {
                    Enable = true,
                    UseDefaultTimeout = false,
                    Timeout = 5
                }
            };

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        [TestMethod]
        public void Set()
        {
            string set = $"Not{Name}";
            var a = Session.FindServiceUDP(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        #endregion Methods
    }
}