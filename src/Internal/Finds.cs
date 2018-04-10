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

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Standard method called to find objects of certain type
    /// </summary>
    internal static class Finds
    {
        #region Methods

        /// <summary>
        /// Invokes the Finds command. This is the API commands like show-hosts
        /// </summary>
        /// <typeparam name="T">Object type that should be returned</typeparam>
        /// <param name="Session">The session.</param>
        /// <param name="Command">The FindAll command.</param>
        /// <param name="DetailLevel">The detail level to be returned.</param>
        /// <param name="Limit">The number of objects to be returned.</param>
        /// <param name="Offset">The offset.</param>
        /// <param name="Order">The sort order.</param>
        internal static NetworkObjectsPagingResults<T> Invoke<T>(Session Session, string Command, DetailLevels DetailLevel, int Limit, int Offset, IOrder Order)
        {
            var objectConverter = new ObjectConverter(Session, DetailLevel, DetailLevel);

            var data = new Dictionary<string, dynamic>
            {
                { "details-level", DetailLevel.ToString() },
                { "limit", Limit },
                { "offset", Offset },
                { "order", (Order == null)? null:new IOrder[] { Order } }
            };

            string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

            string result = Session.Post(Command, jsonData);

            var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<T>>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            if (results != null)
            {
                objectConverter.PostDeserilization(results);
                results.Next = delegate ()
                {
                    if (results.To == results.Total) { return null; }
                    return Finds.Invoke<T>(Session, Command, DetailLevel, Limit, results.To, Order);
                };
            }

            return results;
        }

        /// <summary>
        /// Invokes the Finds using the show-objects API command.
        /// </summary>
        /// <typeparam name="T">Object type that should be returned</typeparam>
        /// <param name="Session">The session.</param>
        /// <param name="Type">The type of objects to return.</param>
        /// <param name="Filter">The filter to be applied to search.</param>
        /// <param name="IPOnly">if set to <c>true</c> ip only option will be sent.</param>
        /// <param name="DetailLevel">The detail level to return.</param>
        /// <param name="Limit">The number of objects to be returned.</param>
        /// <param name="Offset">The offset.</param>
        /// <param name="Order">The sort order.</param>
        /// <returns></returns>
        internal static NetworkObjectsPagingResults<T> Invoke<T>(Session Session, string Type, string Filter, bool IPOnly, DetailLevels DetailLevel, int Limit, int Offset, IOrder Order)
        {
            var objectConverter = new ObjectConverter(Session, DetailLevel, DetailLevel);

            var data = new Dictionary<string, dynamic>
            {
                { "filter", Filter },
                { "ip-only", IPOnly },
                { "type", Type },
                { "details-level", DetailLevel.ToString() },
                { "limit", Limit },
                { "offset", Offset },
                { "order", (Order == null)? null:new IOrder[] { Order } }
            };

            string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

            string result = Session.Post("show-objects", jsonData);

            var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<T>>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            if (results != null)
            {
                objectConverter.PostDeserilization(results);
                results.Next = delegate ()
                {
                    if (results.To == results.Total) { return null; }
                    return Finds.Invoke<T>(Session, Type, Filter, IPOnly, DetailLevel, Limit, results.To, Order);
                };
            }

            return results;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Default values that should be used where ever FindAll class is used.
        /// </summary>
        internal static class Defaults
        {
            #region Fields

            internal const DetailLevels DetailLevel = DetailLevels.Standard;
            internal const bool IPOnly = false;
            internal const int Limit = 50;
            internal const int Offset = 0;
            internal const IOrder Order = null;

            #endregion Fields
        }

        #endregion Classes
    }
}