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
using System;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Converts Check Point date and time objects to DateTime properties.
    /// </summary>
    internal class CheckPointDateTimeConverter : JsonConverter
    {
        #region Constructors

        public CheckPointDateTimeConverter() : this(false)
        {
        }

        public CheckPointDateTimeConverter(bool ignoreTimeZone)
        {
            IgnoreTimeZone = ignoreTimeZone;
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public bool IgnoreTimeZone { get; }

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);

            if (IgnoreTimeZone)
            {
                DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Unspecified);
                return result.AddMilliseconds((long)obj["posix"]);
            }
            else
            {
                DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                return result.AddMilliseconds((long)obj["posix"]).ToLocalTime();
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dt = (DateTime)value;

            string format = (IgnoreTimeZone) ? "yyyy-MM-ddTHH:mm:ss" : "yyyy-MM-ddTHH:mm:sszzz";

            writer.WriteStartObject();
            writer.WritePropertyName("iso-8601");
            writer.WriteValue(dt.ToString(format));
            writer.WriteEndObject();
        }

        /* Was going to use posix but found CheckPoint changed the values sometimes
                public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                    DateTime dt = (DateTime)value;

                    if (!IgnoreTimeZone)
                    {
                        dt = dt.ToUniversalTime();
                    }

                    TimeSpan ts = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, dt.Kind);

                    writer.WriteStartObject();
                    writer.WritePropertyName("posix");
                    writer.WriteValue((long)ts.TotalMilliseconds);
                    writer.WriteEndObject();
                }
        */

        #endregion Methods
    }
}