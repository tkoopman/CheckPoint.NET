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
    public class ApplicationSiteTests : StandardTestsBase
    {
        #region Fields

        private static readonly string Name = "TestSite.NET";

        #endregion Fields

        #region Methods

        public static async Task<ApplicationSite> CreateTestAppSite(Session session)
        {
            var a = new ApplicationSite(session)
            {
                Name = Name,
                Color = Colors.Red,
                PrimaryCategory = "Custom_Application_Site"
            };

            a.UrlList.Add("www.purple.com");
            await a.AcceptChanges();

            return a;
        }

        [TestMethod]
        public async Task ApplicationSiteTest()
        {
            // Create
            var a = await CreateTestAppSite(Session);
            Assert.IsFalse(a.IsNew);
            Assert.IsNotNull(a.UID);
            Assert.AreEqual(1, a.UrlList.Count);

            // Set
            a.UrlList.Add("www.testsite.net");
            a.Comments = "Test";
            await a.AcceptChanges();
            Assert.AreEqual(2, a.UrlList.Count);
            Assert.AreEqual("Test", a.Comments);

            // Fast Update
            a = Session.UpdateApplicationSite(Name);
            a.UrlList.Remove("www.testsite.net");
            await a.AcceptChanges();
            Assert.AreEqual(1, a.UrlList.Count);

            // Find
            a = await Session.FindApplicationSite(Name);

            // Delete
            await a.Delete();
        }

        [TestMethod]
        public async Task Finds()
        {
            var a = await Session.FindApplicationSites(limit: 5);
            Assert.IsNotNull(a);
            a = await a.NextPage();
        }

        [TestMethod]
        public async Task FindsFiltered()
        {
            var a = await Session.FindApplicationSites(filter: "Check Point", limit: 5, detailLevel: DetailLevels.UID);
            Assert.IsNotNull(a);
            Assert.IsTrue(a.Total > 0);
            a = await a.NextPage();
        }

        #endregion Methods
    }
}