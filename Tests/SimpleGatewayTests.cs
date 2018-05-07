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
using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.FastUpdate;
using Koopman.CheckPoint.SimpleGatewaySettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class SimpleGatewayTests : StandardTestsBase
    {
        #region Fields

        private static readonly IPAddress IP = IPAddress.Parse("10.0.0.138");
        private static readonly IPAddress IPv6 = IPAddress.Parse("fe80::1");
        private static readonly string Name = "TestGateway.NET";

        #endregion Fields

        #region Methods

        public static async Task<SimpleGateway> CreateTestGateway(Session session, TestContext testContext)
        {
            string MgmtName = testContext.Properties["MgmtName"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtName");

            var a = new SimpleGateway(session)
            {
                Name = Name,
                Firewall = true,
                FirewallSettings = new Firewall()
                {
                    AutoCalculateConnectionsHashTableSizeAndMemoryPool = true,
                    AutoMaximumLimitForConcurrentConnections = false,
                    MaximumLimitForConcurrentConnections = 50000
                },
                IPv4Address = IP,
                IPv6Address = IPv6,
                LogsSettings = new Logs()
                {
                    AlertWhenFreeDiskSpaceBelow = true,
                    FreeDiskSpaceMetrics = Metrics.Percent,
                    AlertWhenFreeDiskSpaceBelowThreshold = 10,
                    AlertWhenFreeDiskSpaceBelowType = AlertType.PopupAlert,
                    BeforeDeleteKeepLogsFromTheLastDays = true,
                    BeforeDeleteKeepLogsFromTheLastDaysThreshold = 7,
                    BeforeDeleteRunScript = true,
                    BeforeDeleteRunScriptCommand = "ls",
                    DeleteIndexFilesOlderThanDays = true,
                    DeleteIndexFilesOlderThanDaysThreshold = 5,
                    DeleteIndexFilesWhenIndexSizeAbove = true,
                    DeleteIndexFilesWhenIndexSizeAboveThreshold = 1000,
                    DeleteWhenFreeDiskSpaceBelow = true,
                    DeleteWhenFreeDiskSpaceBelowThreshold = 15
                },
                VPN = true,
                VPNSettings = new VPN()
                {
                    MaximumConcurrentIKENegotiations = 5,
                    MaximumConcurrentTunnels = 1000
                }
            };

            a.SendAlertsToServer.Add(MgmtName);
            a.SendLogsToServer.Add(MgmtName);
            a.Interfaces.Add(new Interface()
            {
                Name = "eth0",
                IPv4Address = IP,
                IPv4MaskLength = 24,
                AntiSpoofing = true,
                AntiSpoofingSettings = new AntiSpoofing()
                {
                    Action = AntiSpoofingAction.Detect
                },
                SecurityZoneSettings = new Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.SpecificZone() { Name = "WirelessZone" },
                Topology = Topology.Internal,
                TopologySettings = new TopologySettings()
                {
                    InterfaceLeadsToDMZ = true,
                    IPAddressBehindThisInterface = TopologyBehind.NetworkDefinedByTheInterfaceIPAndNetMask
                }
            });
            a.Interfaces.Add(new Interface()
            {
                Name = "eth1",
                IPv4Address = IP,
                IPv4MaskLength = 24,
                AntiSpoofing = true,
                AntiSpoofingSettings = new AntiSpoofing()
                {
                    Action = AntiSpoofingAction.Detect
                },
                SecurityZoneSettings = new Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.AutoCalculated()
            });

            await a.AcceptChanges(Ignore.Warnings);
            return a;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public async Task FindNotFound() => await Session.FindSimpleGateway("I Don't Exist!");

        [TestMethod]
        public async Task Finds()
        {
            var a = await Session.FindSimpleGateways(limit: 5, order: SimpleGateway.Order.NameDesc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task GatewayTest()
        {
            // Create
            var a = await CreateTestGateway(Session, TestContext);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(Metrics.Percent, a.LogsSettings.FreeDiskSpaceMetrics);
            Assert.AreEqual(1, a.SendAlertsToServer.Count);
            Assert.AreEqual(1, a.SendLogsToServer.Count);
            Assert.AreEqual(typeof(Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.SpecificZone), a.Interfaces["eth0"]?.SecurityZoneSettings?.GetType());
            Assert.AreEqual(typeof(Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.AutoCalculated), a.Interfaces["eth1"]?.SecurityZoneSettings?.GetType());

            // Set
            a.LogsSettings.AlertWhenFreeDiskSpaceBelowType = AlertType.UserDefinedAlertNo1;
            a.Interfaces[0].IPv4MaskLength = 20;
            Assert.IsTrue(a.IsChanged);
            await a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(AlertType.UserDefinedAlertNo1, a.LogsSettings.AlertWhenFreeDiskSpaceBelowType);

            // Fast Update
            a = Session.UpdateSimpleGateway(Name);
            a.Comments = "Test Gateway";
            await a.AcceptChanges();

            // Find
            a = await Session.FindSimpleGateway(Name);
            Assert.AreEqual("Test Gateway", a.Comments);

            // Delete
            await a.Delete();
        }

        #endregion Methods
    }
}