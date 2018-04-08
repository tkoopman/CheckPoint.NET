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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Tests
{
    [TestClass]
    public class SimpleGatewayTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Filter = "gw";
        private static readonly IPAddress IP = IPAddress.Parse("10.0.0.138");
        private static readonly IPAddress IPv6 = IPAddress.Parse("fe80::1");
        private static readonly string Name = "HQgw";

        #endregion Fields

        #region Methods

        [TestMethod]
        [ExpectedException(typeof(ValidationFailedException))]
        public void Delete()
        {
            Session.DeleteSimpleGateway(Name);
        }

        [TestMethod]
        public void FastUpdate()
        {
            string set = $"Not_{Name}";

            var a = Session.UpdateSimpleGateway(Name);
            a.Name = set;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
        }

        [TestMethod]
        public void Find()
        {
            var a = Session.FindSimpleGateway(Name);
            Assert.IsNotNull(a);
            Assert.AreEqual(DetailLevels.Full, a.DetailLevel);
            Assert.AreEqual(Name, a.Name);
            Assert.AreEqual(Domain.Default, a.Domain);
            Assert.IsFalse(a.IsChanged);
            Assert.IsTrue(a.Interfaces.Count > 0);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindSimpleGateways(limit: 5, order: SimpleGateway.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
        }

        [TestMethod]
        public void FindAllFiltered()
        {
            var a = Session.FindSimpleGateways(filter: Filter, ipOnly: true, limit: 5, order: SimpleGateway.Order.NameDesc);
            Assert.IsNotNull(a);
            a = a.NextPage();
            Assert.IsNotNull(a);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void FindNotFound()
        {
            Session.FindSimpleGateway("I Don't Exist!");
        }

        [TestMethod]
        public void New()
        {
            string name = $"New_{Name}";

            var a = new SimpleGateway(Session)
            {
                Name = name,
                Firewall = true,
                FirewallSettings = new Koopman.CheckPoint.SimpleGatewaySettings.Firewall()
                {
                    AutoCalculateConnectionsHashTableSizeAndMemoryPool = true,
                    AutoMaximumLimitForConcurrentConnections = false,
                    MaximumLimitForConcurrentConnections = 50000
                },
                IPv4Address = IP,
                IPv6Address = IPv6,
                LogsSettings = new Koopman.CheckPoint.SimpleGatewaySettings.Logs()
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
                VPNSettings = new Koopman.CheckPoint.SimpleGatewaySettings.VPN()
                {
                    MaximumConcurrentIKENegotiations = 5,
                    MaximumConcurrentTunnels = 1000
                }
            };

            a.SendAlertsToServer.Add("mgmt");
            a.SendLogsToServer.Add("mgmt");
            a.Interfaces.Add(new Koopman.CheckPoint.SimpleGatewaySettings.Interface()
            {
                Name = "eth0",
                IPv4Address = IP,
                IPv4MaskLength = 24,
                AntiSpoofing = true,
                AntiSpoofingSettings = new Koopman.CheckPoint.SimpleGatewaySettings.AntiSpoofing()
                {
                    Action = Koopman.CheckPoint.SimpleGatewaySettings.AntiSpoofingAction.Detect
                },
                SecurityZoneSettings = new Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.SpecificZone() { Name = "WirelessZone" },
                Topology = Koopman.CheckPoint.SimpleGatewaySettings.Topology.Internal,
                TopologySettings = new Koopman.CheckPoint.SimpleGatewaySettings.TopologySettings()
                {
                    InterfaceLeadsToDMZ = true,
                    IPAddressBehindThisInterface = Koopman.CheckPoint.SimpleGatewaySettings.TopologyBehind.NetworkDefinedByTheInterfaceIPAndNetMask
                }
            });
            a.Interfaces.Add(new Koopman.CheckPoint.SimpleGatewaySettings.Interface()
            {
                Name = "eth1",
                IPv4Address = IP,
                IPv4MaskLength = 24,
                AntiSpoofing = true,
                AntiSpoofingSettings = new Koopman.CheckPoint.SimpleGatewaySettings.AntiSpoofing()
                {
                    Action = Koopman.CheckPoint.SimpleGatewaySettings.AntiSpoofingAction.Detect
                },
                SecurityZoneSettings = new Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.AutoCalculated()
            });

            Assert.IsTrue(a.IsNew);
            a.AcceptChanges(Ignore.Warnings);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(Metrics.Percent, a.LogsSettings.FreeDiskSpaceMetrics);
            Assert.AreEqual(1, a.SendAlertsToServer.Count);
            Assert.AreEqual(1, a.SendLogsToServer.Count);
            Assert.AreEqual(typeof(Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.SpecificZone), a.Interfaces["eth0"]?.SecurityZoneSettings?.GetType());
            Assert.AreEqual(typeof(Koopman.CheckPoint.SimpleGatewaySettings.SecurityZone.AutoCalculated), a.Interfaces["eth1"]?.SecurityZoneSettings?.GetType());
        }

        [TestMethod]
        public void Set()
        {
            string set = $"Not_{Name}";

            var a = Session.FindSimpleGateway(Name);
            a.Name = set;
            a.LogsSettings.AlertWhenFreeDiskSpaceBelowType = AlertType.UserDefinedAlertNo1;
            a.Interfaces[0].IPv4MaskLength = 20;
            Assert.IsTrue(a.IsChanged);
            a.AcceptChanges();
            Assert.IsFalse(a.IsChanged);
            Assert.AreEqual(set, a.Name);
            Assert.AreEqual(AlertType.UserDefinedAlertNo1, a.LogsSettings.AlertWhenFreeDiskSpaceBelowType);
        }

        #endregion Methods
    }
}