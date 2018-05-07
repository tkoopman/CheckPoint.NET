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

using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Common internal static methods
    /// </summary>
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
            return Regex.Matches(source, @"(^[a-z\d]+|[A-Z\d]+(?![a-z])|[A-Z][a-z\d]+)")
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
        /// </code>
        /// </example>
        internal static string CamelCaseToRegular(this string source, string separator = " ")
        {
            string[] s = CamelCaseToArray(source);
            return string.Join(separator, s);
        }

        /// <summary>
        /// Centers the string with spaces as padding.
        /// </summary>
        /// <param name="stringToCenter">The string to center.</param>
        /// <param name="totalLength">The total length of result string.</param>
        internal static string CenterString(this string stringToCenter, int totalLength)
        {
            return stringToCenter.PadLeft(
                ((totalLength - stringToCenter.Length) / 2)
                  + stringToCenter.Length).PadRight(totalLength);
        }

        /// <summary>
        /// Centers the string.
        /// </summary>
        /// <param name="stringToCenter">The string to center.</param>
        /// <param name="totalLength">The total length of result string.</param>
        /// <param name="paddingCharacter">The padding character.</param>
        internal static string CenterString(this string stringToCenter,
                                               int totalLength,
                                               char paddingCharacter)
        {
            return stringToCenter.PadLeft(
                ((totalLength - stringToCenter.Length) / 2) + stringToCenter.Length,
                  paddingCharacter).PadRight(totalLength, paddingCharacter);
        }

        internal static object GetProperty(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (!TryGetProperty(obj, name, out object value, bindingFlags)) throw new ArgumentException("No readable property found.");

            return value;
        }

        internal static void SetProperty(this object obj, string name, object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (!TrySetProperty(obj, name, value, bindingFlags)) throw new ArgumentException("No writeable property found.");
        }

        internal static bool TryGetProperty(this object obj, string name, out object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (name == null) throw new ArgumentNullException(nameof(name));
            value = null;

            var prop = obj.GetType().GetTypeInfo().GetProperty(name, bindingFlags);
            if (prop != null && prop.CanRead)
                value = prop.GetValue(obj);
            else
                return false;

            return true;
        }

        internal static bool TrySetProperty(this object obj, string name, object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var prop = obj.GetType().GetTypeInfo().GetProperty(name, bindingFlags);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
            else
                return false;

            return true;
        }

        #endregion Methods
    }
}