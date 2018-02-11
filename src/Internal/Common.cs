// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Koopman.CheckPoint.Internal
{
    internal static class Common
    {
        #region Methods

        /// <summary>
        /// Converts a string that is in CamelCase to a string array of separate words.
        /// </summary>
        /// <param name="source">The source string to convert.</param>
        /// <returns>String array to words</returns>
        /// <example>
        /// The following code:
        /// <code>
        /// "ThisIsInCamelCase".CamelCaseToArray();
        ///
        /// Result:
        /// ["This", "Is", "In", "Camel", "Case"]
        /// </code>
        /// </example>
        internal static string[] CamelCaseToArray(this string source)
        {
            return Regex.Matches(source, @"(^[a-z\d]+|[A-Z]+(?![a-z\d])|[A-Z][a-z\d]+)")
                .OfType<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        /// <summary>
        /// Converts a string that is in CamelCase to a regular string.
        /// </summary>
        /// <param name="source">The source string to convert.</param>
        /// <param name="separator">The separator to use between words.</param>
        /// <example>
        /// The following code:
        /// <code>
        /// "ThisIsInCamelCase".CamelCaseToRegular();
        ///
        /// Result:
        /// "This Is In Camel Case"
        /// </code></example>
        internal static string CamelCaseToRegular(this string source, string separator = " ")
        {
            string[] s = CamelCaseToArray(source);
            return String.Join(separator, s);
        }

        internal static string CenterString(this string stringToCenter, int totalLength)
        {
            return stringToCenter.PadLeft(
                ((totalLength - stringToCenter.Length) / 2)
                  + stringToCenter.Length).PadRight(totalLength);
        }

        internal static string CenterString(this string stringToCenter,
                                               int totalLength,
                                               char paddingCharacter)
        {
            return stringToCenter.PadLeft(
                ((totalLength - stringToCenter.Length) / 2) + stringToCenter.Length,
                  paddingCharacter).PadRight(totalLength, paddingCharacter);
        }

        #endregion Methods
    }
}