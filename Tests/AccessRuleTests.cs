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
using Koopman.CheckPoint.AccessRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AccessRuleTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public async Task Find()
        {
            var a = await Session.FindAccessRule("TestLayer", 1, DetailLevels.Full);
            Assert.IsNotNull(a);
            Assert.IsNotNull(a.Action);
            Assert.IsNotNull(a.Layer);
            Assert.IsTrue(a.Source.Count > 0);
            Assert.IsTrue(a.Destination.Count > 0);
            Assert.IsTrue(a.Service.Count > 0);
            Assert.IsTrue(a.Content.Count > 0);
        }

        [TestMethod]
        public async Task FindRulebase()
        {
            var a = await Session.FindAccessRulebase("TestLayer", detailLevel: DetailLevels.Full);
            Assert.IsNotNull(a);
            a = await Session.FindAccessRulebase(a.UID, detailLevel: DetailLevels.Standard);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public async Task New()
        {
            var a = new AccessRule(Session, "TestLayer", new Position(1))
            {
                Action = RulebaseAction.Accept,
                CustomFields = new CustomFields() { Field1 = "TestNew" }
            };
            a.ActionSettings.SetLimit("Upload_1Gbps");
            a.Track.Type = TrackType.Log;
            a.Source.Add("DNS Server");
            a.VPN.Add("All_GwToGw");
            await a.AcceptChanges();
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            a.Source.Clear();
            a.Destination.Add("DNS Server");
            a.CustomFields.Field1 = "Test";
            a.SetPosition(new Position(Positions.Bottom));
            a.Track.Type = TrackType.ExtendedLog;
            a.Track.PerConnection = false;
            a.Track.PerSession = true;
            a.Track.Alert = AlertType.SNMP;
            a.Name = "Test Rule";
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(TrackType.ExtendedLog, a.Track.Type);
            Assert.AreEqual(1, a.Destination.Count);
            Assert.IsFalse(a.Track.PerConnection);
            Assert.IsTrue(a.Track.PerSession);
        }

        #endregion Methods
    }
}