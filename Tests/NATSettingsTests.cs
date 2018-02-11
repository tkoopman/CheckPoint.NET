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
using Koopman.CheckPoint.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Tests
{
    [TestClass]
    public class NATSettingsTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "DNS Server";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void HideGateway()
        {
            var a = Session.FindHost(Name);
            a.NATSettings = new NATSettings()
            {
                AutoRule = true,
                Method = NATSettings.NATMethods.Hide,
                HideBehind = NATSettings.HideBehindValues.Gateway
            };
            a.AcceptChanges();
        }

        [TestMethod]
        public void None()
        {
            var a = Session.FindHost(Name);
            a.NATSettings = new NATSettings();
            a.AcceptChanges();
        }

        [TestMethod]
        public void StaticIP()
        {
            var a = Session.FindHost(Name);
            a.NATSettings = new NATSettings()
            {
                AutoRule = true,
                Method = NATSettings.NATMethods.Static,
                IPv4Address = IPAddress.Parse("1.1.1.1")
            };
            a.AcceptChanges();

            Assert.AreEqual(IPAddress.Parse("1.1.1.1"), a.NATSettings.IPv4Address);
            Assert.IsNull(a.NATSettings.IPv6Address);
            Assert.IsFalse(a.IsChanged);
        }

        #endregion Methods
    }
}