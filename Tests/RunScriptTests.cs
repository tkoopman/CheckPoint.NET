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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class RunScriptTests : StandardTestsBase
    {
        #region Properties

        public string RunScriptHostName => TestContext.Properties["RunScriptHostName"]?.ToString() ?? Environment.GetEnvironmentVariable("TestRunScriptHostName");

        #endregion Properties

        #region Methods

        [TestMethod]
        public async Task RunCancel()
        {
            string run = await Session.RunScript("Sleep", "sleep 4", null, RunScriptHostName);

            var task = await Session.FindTask(run);

            var cts = new CancellationTokenSource();
            var p = new Progress<int>(i => Console.Out.WriteLine($"Progress: {i}%"));
            bool caught = false;
            try
            {
                var t = task.WaitAsync(cancellationToken: cts.Token, progress: p);
                cts.CancelAfter(new TimeSpan(0, 0, 1));
                t.Wait();
            }
            catch (Exception)
            {
                caught = true;
            }

            // Wait for task to finish
            await task.WaitAsync();

            Assert.IsTrue(caught);
        }

        [TestMethod]
        public async Task RunLS()
        {
            string run = await Session.RunScript("LS", "ls -l /", null, RunScriptHostName);
            Assert.IsNotNull(run);

            var task = await Session.FindTask(run);
            Assert.IsNotNull(task);

            bool s = await task.WaitAsync();
            Assert.IsTrue(s);

            Assert.IsTrue(task.TaskDetails[0].ResponseMessage.Contains("etc"));
        }

        #endregion Methods
    }
}