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
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.IA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class IATests
    {
        #region Properties

        public TestContext TestContext { get; set; }
        private TextWriter DebugWriter { get; } = new StringWriter();

        #endregion Properties

        #region Methods

        [TestCleanup]
        public void CleanupTest()
        {
            TestContext.WriteLine(DebugWriter.ToString());
            DebugWriter.Close();
        }

        [TestMethod]
        public async Task IATest()
        {
            string Gateway = TestContext.Properties["IAGateway"]?.ToString() ?? Environment.GetEnvironmentVariable("TestIAGateway");
            string Secret = TestContext.Properties["IASecret"]?.ToString() ?? Environment.GetEnvironmentVariable("TestIASecret");
            string CertificateHash = TestContext.Properties["IACertificateHash"]?.ToString() ?? Environment.GetEnvironmentVariable("TestIACertificateHash");

            // Setting as tests running under .NET 4.5 which don't allow different pinning as
            // statically assigned.
            ServicePointManager.ServerCertificateValidationCallback = null;
            try
            {
                var session = new IASession(Gateway, Secret, CertificateHash, debugWriter: DebugWriter);

                var result = await session.AddIdentity("10.1.1.1", machine: "IATestMachine", fetchMachineGroups: false, calculateRoles: false, roles: new string[] { "TestIARole" });
                Assert.AreEqual("10.1.1.1", result.IPv4Address);
                result = await session.AddIdentity("10.1.1.2", machine: "IATestMachine2", fetchMachineGroups: false, calculateRoles: false, roles: new string[] { "TestIARole" });
                Assert.AreEqual("10.1.1.2", result.IPv4Address);

                session.StartAddBatch((r) => Console.WriteLine($"{r.IPv4Address}: {r.Message}"), 50);
                List<Task> tasks = new List<Task>(255);
                for(int x = 1; x < 255; x++)
                    tasks.Add(session.AddIdentity($"10.1.2.{x}", machine: "IATestMachine", fetchMachineGroups: false, calculateRoles: false, roles: new string[] { "TestIARole" }));
                tasks.Add(session.Flush(true));
                await Task.WhenAll(tasks);

                var showResult = await session.ShowIdentity("10.1.1.1");
                Assert.IsNotNull(showResult);
                session.StartShowBatch((r) => Console.WriteLine($"{r.IPv4Address}: {r.Message}"), 50);
                tasks = new List<Task>(255);
                for (int x = 1; x < 255; x++)
                    tasks.Add(session.ShowIdentity($"10.1.2.{x}"));
                tasks.Add(session.Flush(true));
                await Task.WhenAll(tasks);

                var delResult = await session.DeleteIdentity("10.1.1.1");
                Assert.IsTrue(delResult.Count > 0);
                session.StartDeleteBatch((r) => Console.WriteLine($"{r.IPv4Address}:{r.Count}: {r.Message}"), 50);
                tasks = new List<Task>(255);
                for (int x = 1; x < 255; x++)
                    tasks.Add(session.DeleteIdentity($"10.1.2.{x}"));
                tasks.Add(session.Flush(true));
                await Task.WhenAll(tasks);
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        #endregion Methods
    }
}