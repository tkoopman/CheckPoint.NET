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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Examples
{
    [TestClass]
    public abstract class ExampleBase
    {
        #region Properties

        public string CertificateHash { get; private set; }
        public string ManagementServer { get; private set; }
        public string Password { get; private set; }
        public TestContext TestContext { get; set; }
        public string Username { get; private set; }

        #endregion Properties

        #region Methods

        [TestInitialize]
        public void InitializeTest()
        {
            ManagementServer = (TestContext.Properties.ContainsKey("ManagementServer")) ? TestContext.Properties["ManagementServer"].ToString() : Environment.GetEnvironmentVariable("TestMgmtServer");
            Username = (TestContext.Properties.ContainsKey("User")) ? TestContext.Properties["User"].ToString() : Environment.GetEnvironmentVariable("TestMgmtUser");
            Password = (TestContext.Properties.ContainsKey("Password")) ? TestContext.Properties["Password"].ToString() : Environment.GetEnvironmentVariable("TestMgmtPassword");
            CertificateHash = (TestContext.Properties.ContainsKey("CertificateHash")) ? TestContext.Properties["CertificateHash"].ToString() : Environment.GetEnvironmentVariable("TestCertificateHash");
        }

        #endregion Methods
    }
}