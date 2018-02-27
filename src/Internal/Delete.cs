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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Standard method called to delete objects
    /// </summary>
    internal static class Delete
    {
        #region Methods

        /// <summary>
        /// Invokes the Delete.
        /// </summary>
        /// <param name="Session">The session.</param>
        /// <param name="Command">The delete command.</param>
        /// <param name="Value">The object name or UID.</param>
        /// <param name="Ignore">The ignore setting.</param>
        internal static void Invoke(Session Session, string Command, string Value, Ignore Ignore)
        {
            JObject jo = new JObject
            {
                { Value.isUID() ? "uid" : "name", Value }
            };

            jo.AddIgnore(Ignore);

            string jsonData = JsonConvert.SerializeObject(jo, Session.JsonFormatting);

            string result = Session.Post(Command, jsonData);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Default values that should be used whereever Delete class is used.
        /// </summary>
        internal static class Defaults
        {
            #region Fields

            internal const Ignore ignore = Ignore.No;

            #endregion Fields
        }

        #endregion Classes
    }
}