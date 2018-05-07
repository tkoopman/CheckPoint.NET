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
using Koopman.CheckPoint.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class AccessRuleTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public async Task AccessRuleTest()
        {
            var layer = await AccessLayerTests.CreateTestAccessLayer(Session);
            var host = await HostTests.CreateTestHost(Session);
            var service = await ServiceGroupTests.CreateTestGroup(Session);

            // Create Rule
            var a = new AccessRule(Session, layer.Name, new Position(2))
            {
                Action = RulebaseAction.Accept,
                CustomFields = new CustomFields() { Field1 = "TestNew" }
            };
            a.ActionSettings.SetLimit("Upload_1Gbps");
            a.Track.Type = TrackType.Log;
            a.Source.Add(host);
            a.Service.Add(service);
            await a.AcceptChanges();
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);

            // Set Rule
            a.Source.Clear();
            a.Destination.Add(host);
            a.CustomFields.Field2 = "Test";
            a.SetPosition(new Position(Positions.Top));
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

            // Find by rule#
            a = await Session.FindAccessRule(layer.Name, 1, DetailLevels.Full);
            Assert.AreEqual("Test Rule", a.Name);

            // Find rule base
            var b = await Session.FindAccessRulebase(layer.Name, detailLevel: DetailLevels.Full);
            Assert.IsNotNull(b);
            Assert.AreEqual(2, b.Total);

            // Export Rule base
            var export = new JsonExport(Session);
            await export.AddAsync(b);
            string e = await export.Export();
            Assert.IsTrue(e.StartsWith("{"));
            Assert.IsTrue(e.EndsWith("}"));
            Assert.IsTrue(e.Contains(service.UID));
            foreach (var m in service.Members)
                Assert.IsTrue(e.Contains(m.UID));

            // Delete Rule
            await a.Delete();
        }

        #endregion Methods
    }
}