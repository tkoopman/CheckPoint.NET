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
using Koopman.CheckPoint.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public abstract class StandardTestsBase
    {
        #region Properties

        public Session Session { get; private set; }
        public TestContext TestContext { get; set; }
        private TextWriter DebugWriter { get; } = new StringWriter();

        #endregion Properties

        #region Methods

        [TestCleanup]
        public void CleanupTest()
        {
            Session.DebugWriter = null;
            DebugWriter.WriteLine($" Completed test {TestContext.TestName} ".CenterString(60, '#'));
            Session.Discard();
            Session.Logout();
            TestContext.WriteLine(DebugWriter.ToString());
            DebugWriter.Close();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            string ManagementServer = TestContext.Properties["ManagementServer"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtServer");
            string User = TestContext.Properties["User"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtUser");
            string Password = TestContext.Properties["Password"]?.ToString() ?? Environment.GetEnvironmentVariable("TestMgmtPassword");

            Session = new Session(
                         managementServer: ManagementServer,
                         userName: User,
                         password: Password,
                         certificateValidation: false,
                         indentJson: true
                     );

            Session.DebugWriter = DebugWriter;
            DebugWriter.WriteLine($" Starting test {TestContext.TestName} ".CenterString(60, '#'));
        }

        #endregion Methods
    }
}