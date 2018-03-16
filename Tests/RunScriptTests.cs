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

namespace Tests
{
    [TestClass]
    public class RunScriptTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public void RunCancel()
        {
            string run = Session.RunScript("Sleep", "sleep 4", null, "mgmt");

            Task task = Session.FindTask(run);

            CancellationTokenSource cts = new CancellationTokenSource();
            Progress<int> p = new Progress<int>(i => Console.Out.WriteLine($"Progress: {i}%"));
            bool caught = false;
            try
            {
                System.Threading.Tasks.Task<bool> t = task.WaitAsync(cancellationToken: cts.Token, progress: p);
                cts.CancelAfter(new TimeSpan(0, 0, 1));
                t.Wait();
            }
            catch (Exception)
            {
                caught = true;
            }

            // Wait for task to finish
            task.WaitAsync().Wait();

            Assert.IsTrue(caught);
        }

        [TestMethod]
        public void RunLS()
        {
            string run = Session.RunScript("LS", "ls -l /", null, "mgmt");
            Assert.IsNotNull(run);

            Task task = Session.FindTask(run);
            Assert.IsNotNull(task);

            bool s = task.WaitAsync().Result;
            Assert.IsTrue(s);

            Console.Out.WriteLine(task.TaskDetails[0].ResponseMessage);
        }

        #endregion Methods
    }
}