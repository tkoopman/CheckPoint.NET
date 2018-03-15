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

namespace Tests
{
    [TestClass]
    public class SessionInfoTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public void Find()
        {
            var a = Session.FindSession();
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void FindAll()
        {
            var a = Session.FindAllSessions(limit: 5);
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void SetInfo()
        {
            string name = "Session Name";
            string description = "Session Description";
            Colors color = Colors.DarkBlue;
            string[] tags = new string[] { "ATag" };

            var a = Session.SetSessionInfo(
                name: name,
                description: description,
                color: color,
                tags: tags
                );
            Assert.IsNotNull(a);
            Assert.AreEqual(name, a.Name);
            Assert.AreEqual(description, a.Description);
            Assert.AreEqual(color, a.Color);
            Assert.AreEqual(tags.Length, a.Tags.Length);

            // Make sure sending all nulls doesn't change values
            a = Session.SetSessionInfo();
            Assert.IsNotNull(a.Name);
            Assert.AreEqual(name, a.Name);
            Assert.AreEqual(description, a.Description);
            Assert.AreEqual(color, a.Color);
            Assert.AreEqual(tags.Length, a.Tags.Length);
        }

        #endregion Methods
    }
}