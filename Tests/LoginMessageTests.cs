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

namespace Tests
{
    /// <summary>
    /// Summary description for LoginMessageTests
    /// </summary>
    [TestClass]
    public class LoginMessageTests : StandardTestsBase
    {
        #region Methods

        [TestMethod]
        public void SetLoginMessage()
        {
            string header = "Session header";
            string message = "Should you be here?";
            bool show = true;
            bool warn = false;

            var a = Session.SetLoginMessage(
                    header: header,
                    message: message,
                    showMessage: show,
                    warning: warn
                );

            Assert.IsNotNull(a);
            Assert.AreEqual(header, a.Header);
            Assert.AreEqual(message, a.Message);
            Assert.AreEqual(show, a.ShowMessage);
            Assert.AreEqual(warn, a.Warning);

            // Confirm sending nulls doesn't change anything
            a = Session.SetLoginMessage();
            Assert.IsNotNull(a);
            Assert.AreEqual(header, a.Header);
            Assert.AreEqual(message, a.Message);
            Assert.AreEqual(show, a.ShowMessage);
            Assert.AreEqual(warn, a.Warning);
        }

        [TestMethod]
        public void ShowLoginMessage()
        {
            var a = Session.GetLoginMessage();
            Assert.IsNotNull(a);
        }

        #endregion Methods
    }
}