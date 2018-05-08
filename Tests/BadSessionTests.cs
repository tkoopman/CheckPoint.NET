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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;

namespace Tests
{
    [TestClass]
    public class BadSessionTests
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
        [ExpectedException(typeof(System.Net.Http.HttpRequestException))]
        public void WrongCertificateHash()
        {
            string ManagementServer = TestContext.Properties["ManagementServer"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtServer");
            string User = TestContext.Properties["User"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtUser");
            string Password = TestContext.Properties["Password"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtPassword");
            string CertificateHash = TestContext.Properties["CertificateHash"]?.ToString() ?? Environment.GetEnvironmentVariable("TestCertificateHash");

            // Setting as tests running under .NET 4.5 which don't allow different pinning as
            // statically assigned.
            ServicePointManager.ServerCertificateValidationCallback = null;
            try
            {
                var session = Session.Login(
                             managementServer: ManagementServer,
                             userName: User,
                             password: Password,
                             certificateHash: "00000000",
                             indentJson: true,
                             sessionName: "CheckPoint.NET Test",
                             description: TestContext.TestName
                         ).GetAwaiter().GetResult();
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        /// <summary>
        /// While one would expect this to throw LoginFailedWrongUsernameOrPasswordException
        /// Currently API only returns LoginFailedException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(LoginFailedException))]
        public void WrongCredentials()
        {
            string ManagementServer = TestContext.Properties["ManagementServer"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtServer");
            string CertificateHash = TestContext.Properties["CertificateHash"]?.ToString() ?? Environment.GetEnvironmentVariable("TestCertificateHash");
            using (Session.Login(
                         managementServer: ManagementServer,
                         userName: "dummy",
                         password: "***",
                         certificateHash: CertificateHash,
                         debugWriter: DebugWriter
                     ).GetAwaiter().GetResult())
            {
            }
        }

        #endregion Methods
    }
}