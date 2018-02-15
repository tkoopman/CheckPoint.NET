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
    internal static class FindAll
    {
        #region Methods

        internal static NetworkObjectsPagingResults<T> Invoke<T>(Session Session, string Command, DetailLevels DetailLevel, int Limit, int Offset, IOrder Order)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
            {
                { "details-level", DetailLevel.ToString() },
                { "limit", Limit },
                { "offset", Offset },
                { "order", (Order == null)? null:new IOrder[] { Order } }
            };

            string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

            string result = Session.Post(Command, jsonData);

            NetworkObjectsPagingResults<T> results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<T>>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevel, DetailLevel) } });

            if (results != null)
            {
                results.Next = delegate ()
                {
                    if (results.To == results.Total) { return null; }
                    return FindAll.Invoke<T>(Session, Command, DetailLevel, Limit, results.To, Order);
                };
            }

            return results;
        }

        internal static NetworkObjectsPagingResults<T> Invoke<T>(Session Session, string Type, string Filter, bool IPOnly, DetailLevels DetailLevel, int Limit, int Offset, IOrder Order)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>
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

            NetworkObjectsPagingResults<T> results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<T>>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevel, DetailLevel) } });

            if (results != null)
            {
                results.Next = delegate ()
                {
                    if (results.To == results.Total) { return null; }
                    return FindAll.Invoke<T>(Session, Type, Filter, IPOnly, DetailLevel, Limit, results.To, Order);
                };
            }

            return results;
        }

        #endregion Methods

        #region Classes

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