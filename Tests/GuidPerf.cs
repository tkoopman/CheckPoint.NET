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
using System;
using System.Diagnostics;

namespace Tests
{
    public class GuidPerf
    {
        #region Fields

        private const string compareStr = "f15063f0-abd4-4f41-b0cd-94d3989321bf";
        private const string guid = "f15063f0-abd4-4f41-a0cd-94d3989321bf";
        private const int runs = 1000000;
        private static readonly Guid compareGuid = new Guid(compareStr);

        #endregion Fields

        #region Methods

        [TestMethod]
        public void New()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int x = 0; x < runs; x++)
            {
                var g = new Guid(guid);
                string s = g.ToString();
                bool b = g.Equals(compareGuid);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestMethod]
        public void Parse()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int x = 0; x < runs; x++)
            {
                var g = Guid.Parse(guid);
                string s = g.ToString();
                bool b = g.Equals(compareGuid);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestMethod]
        public void Strings()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int x = 0; x < runs; x++)
            {
                bool g = IsUID(guid);
                bool b = guid.Equals(compareStr);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestMethod]
        public void TryParse()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int x = 0; x < runs; x++)
            {
                Guid.TryParse(guid, out var g);
                string s = g.ToString();
                bool b = g.Equals(compareGuid);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        /// <summary>
        /// Returns true if string is in the format of a Check Point UID
        /// </summary>
        /// <param name="str">String to test.</param>
        /// <returns>True if valid UID format</returns>
        private static bool IsUID(string str) => Guid.TryParse(str, out _);

        #endregion Methods
    }
}