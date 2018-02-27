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

using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Standard method called to reload objects
    /// </summary>
    internal static class Reload
    {
        #region Methods

        /// <summary>
        /// Invokes the reload.
        /// </summary>
        /// <param name="command">The show command to use to get refreshed data.</param>
        /// <param name="obj">The object to be reloaded.</param>
        /// <param name="OnlyIfPartial">
        /// if set to <c>true</c> if reload should only be run if current DetailLevel is not Full.
        /// </param>
        /// <param name="detailLevel">The detail level to be returned.</param>
        /// <exception cref="Exception">Cannot reload a new object.</exception>
        internal static void Invoke(string command, ObjectSummary obj, bool OnlyIfPartial, DetailLevels detailLevel)
        {
            if (obj.IsNew) { throw new Exception("Cannot reload a new object."); }
            if (OnlyIfPartial && obj.DetailLevel == DetailLevels.Full) { return; }

            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "uid", obj.UID },
                { "details-level", detailLevel.ToString() }
            };

            string jsonData = JsonConvert.SerializeObject(data, obj.Session.JsonFormatting);

            string result = obj.Session.Post(command, jsonData);

            obj.DetailLevel = DetailLevels.Full;

            JsonConvert.PopulateObject(result, obj, new JsonSerializerSettings() { Converters = { new ObjectConverter(obj.Session, DetailLevels.Full, detailLevel) } });
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Default values that should be used whereever Reload class is used.
        /// </summary>
        internal static class Defaults
        {
            #region Fields

            internal const DetailLevels DetailLevel = DetailLevels.Standard;
            internal const bool OnlyIfPartial = false;

            #endregion Fields
        }

        #endregion Classes
    }
}