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

using Koopman.CheckPoint.AccessRules;
using Koopman.CheckPoint.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class StaticObjectsTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public async Task GenericObjects()
        {
            foreach (var a in GenericObjectSummary.InBuilt)
            {
                var b = await Session.FindObject(a.UID);
                Assert.AreEqual(a.Name, b.Name);
                Assert.AreEqual(a.Type, b.Type);
                Assert.AreEqual(a.Domain.Name, b.Domain.Name);
                Assert.AreEqual(a.Domain.UID, b.Domain.UID);
            }
        }

        [TestMethod]
        public async Task RulebaseActions()
        {
            foreach (var a in RulebaseAction.Actions)
            {
                var b = await Session.FindObject(a.UID);
                Assert.AreEqual(a.Name, b.Name);
                Assert.AreEqual(a.Type, b.Type);
                Assert.AreEqual(a.Domain.Name, b.Domain.Name);
                Assert.AreEqual(a.Domain.UID, b.Domain.UID);
            }
        }

        [TestMethod]
        public async Task TrackTypes()
        {
            foreach (var a in TrackType.Types)
            {
                var b = await Session.FindObject(a.UID);
                Assert.AreEqual(a.Name, b.Name);
                Assert.AreEqual(a.Type, b.Type);
                Assert.AreEqual(a.Domain.Name, b.Domain.Name);
                Assert.AreEqual(a.Domain.UID, b.Domain.UID);
            }
        }

        #endregion Methods
    }
}