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
    public class ApplicationGroupTests : StandardTestsBase
    {
        #region Fields

        private const string Name = "TestAppGroup.NET";

        #endregion Fields

        #region Methods

        [TestMethod]
        public async Task AppGroupTest()
        {
            // Create
            var a = new ApplicationGroup(Session)
            {
                Name = Name,
                Color = Colors.Red
            };
            a.Members.Add("Alcohol");
            await a.AcceptChanges();
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);

            // Set
            a.Members.Add("Email");
            a.Members.Remove("Alcohol");
            await a.AcceptChanges();
            Assert.AreEqual("Email", a.Members[0].Name);

            // Fast Update
            a = Session.UpdateApplicationGroup(Name);
            a.Comments = "Test comments";
            await a.AcceptChanges();

            // Find
            a = await Session.FindApplicationGroup(Name);
            Assert.AreEqual("Test comments", a.Comments);

            // Delete
            await a.Delete();
        }

        [TestMethod]
        public async Task FindAll()
        {
            var a = await Session.FindApplicationGroups(limit: 5);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        #endregion Methods
    }
}