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
using Koopman.CheckPoint.FastUpdate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class GroupWithExclusionTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "TestGroupWithExlusion.NET";

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindGroupsWithExclusion(limit: 5, order: GroupWithExclusion.Order.NameAsc);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task GroupWithExclusionTest()
        {
            var g = await GroupTests.CreateTestGroup(Session);

            // Create
            var a = new GroupWithExclusion(Session)
            {
                Name = Name,
                Color = Colors.Red,
                Include = ObjectSummary.Any,
                Except = g
            };
            await a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Set
            a.Comments = "Test";
            await a.AcceptChanges();

            // Fast Update
            a = Session.UpdateGroupWithExclusion(Name);
            a.Tags.Add("CheckPoint.NET");
            await a.AcceptChanges();

            // Find
            a = await Session.FindGroupWithExclusion(Name);
            Assert.AreEqual(1, a.Tags.Count);
            Assert.AreEqual("Test", a.Comments);

            // Delete
            await Session.DeleteGroupWithExclusion(Name);

            // Create as string
            a = new GroupWithExclusion(Session)
            {
                Name = Name,
                Color = Colors.Red
            };
            a.SetInclude("any");
            a.SetExcept(g.Name);

            await a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
        }

        #endregion Methods
    }
}