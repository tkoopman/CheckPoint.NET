﻿// MIT License
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
using System.Collections.Generic;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Standard method called to find objects
    /// </summary>
    internal static class Find
    {
        #region Methods

        /// <summary>
        /// Invokes the Find command.
        /// </summary>
        /// <typeparam name="T">Object type that should be returned</typeparam>
        /// <param name="Session">The session.</param>
        /// <param name="Command">The find command.</param>
        /// <param name="Value">The name or UID of the object to find.</param>
        /// <param name="DetailLevel">The detail level to be returned.</param>
        internal static T Invoke<T>(Session Session, string Command, string Value, DetailLevels DetailLevel)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { Value.isUID() ? "uid" : "name", Value },
                { "details-level", DetailLevel.ToString() }
            };

            string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

            string result = Session.Post(Command, jsonData);

            return JsonConvert.DeserializeObject<T>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevels.Full, DetailLevel) } });
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Default values that should be used whereever Find class is used.
        /// </summary>
        internal static class Defaults
        {
            #region Fields

            internal const DetailLevels DetailLevel = DetailLevels.Standard;

            #endregion Fields
        }

        #endregion Classes
    }
}