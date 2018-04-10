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

using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Static internal Check Point related methods
    /// </summary>
    internal static class CheckPoint
    {
        #region Methods

        internal static void AddIfNotNull(this JObject jo, string name, string value)
        {
            if (value == null) return;
            jo.Add(name, value);
        }

        internal static void AddIfNotNull(this JObject jo, string name, bool? value)
        {
            if (value == null) return;
            jo.Add(name, value);
        }

        internal static void AddIfNotNull(this JObject jo, string name, int? value)
        {
            if (value == null) return;
            jo.Add(name, value);
        }

        /// <summary>
        /// Adds the ignore values to a request.
        /// </summary>
        /// <param name="jo">The request.</param>
        /// <param name="ignore">The ignore setting.</param>
        internal static void AddIgnore(this JObject jo, Ignore ignore)
        {
            switch (ignore)
            {
                case Ignore.Warnings:
                    jo.Add("ignore-warnings", true);
                    break;

                case Ignore.Errors:
                    jo.Add("ignore-errors", true);
                    break;
            }
        }

        /// <summary>
        /// Returns true if string is in the format of a Check Point UID
        /// </summary>
        /// <param name="str">String to test.</param>
        /// <returns>True if valid UID format</returns>
        internal static bool IsUID(this string str) => Regex.IsMatch(str, @"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$");

        #endregion Methods
    }
}