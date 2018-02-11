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

using Koopman.CheckPoint.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;

namespace Tests
{
    /// <summary>
    /// Summary description for SubnetMaskTest
    /// </summary>
    [TestClass]
    public class SubnetMaskTest
    {
        #region Fields

        private Dictionary<int, string> TestSubnetMasks = new Dictionary<int, string>
        {
            { 0, "0.0.0.0" },
            { 1, "128.0.0.0" },
            { 8, "255.0.0.0" },
            { 16, "255.255.0.0" },
            { 24, "255.255.255.0" },
            { 27, "255.255.255.224" },
            { 32, "255.255.255.255" }
        };

        #endregion Fields

        #region Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        [TestMethod]
        public void MaskLenToSubnetMask()
        {
            foreach (int len in TestSubnetMasks.Keys)
            {
                Assert.AreEqual(IPAddress.Parse(TestSubnetMasks[len]), SubnetMask.MaskLengthToSubnetMask(len));
            }
        }

        [TestMethod]
        public void SubnetMaskToMaskLen()
        {
            foreach (int len in TestSubnetMasks.Keys)
            {
                Assert.AreEqual(len, SubnetMask.SubnetMaskToMaskLength(IPAddress.Parse(TestSubnetMasks[len])));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SubnetMaskToMaskLenFail1()
        {
            SubnetMask.SubnetMaskToMaskLength(IPAddress.Parse("255.255.255.1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SubnetMaskToMaskLenFail2()
        {
            SubnetMask.SubnetMaskToMaskLength(IPAddress.Parse("255.0.255.0"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SubnetMaskToMaskLenFail3()
        {
            SubnetMask.SubnetMaskToMaskLength(IPAddress.Parse("0.0.0.1"));
        }

        #endregion Methods
    }
}