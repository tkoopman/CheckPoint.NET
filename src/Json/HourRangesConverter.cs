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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Converts HourRanges.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class HourRangesConverter : JsonConverter
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(HourRanges) == objectType;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HourRanges result = new HourRanges();
            result.OnDeserializingMethod(serializer.Context);

            if (reader.TokenType != JsonToken.StartArray) throw new JsonSerializationException($"Cannot convert non array token value to {objectType}.");

            JArray array = serializer.Deserialize<JArray>(reader);
            foreach (JObject obj in array)
            {
                if (obj != null)
                {
                    int index = (int)obj.GetValue("index");
                    bool enabled = (bool)obj.GetValue("enabled");
                    TimeRange timeRange = null;
                    if (enabled)
                    {
                        TimeOfDay start = new TimeOfDay(obj.GetValue("from").ToString());
                        TimeOfDay end = new TimeOfDay(obj.GetValue("to").ToString());
                        timeRange = new TimeRange(start, end);
                    }

                    result[index - 1] = timeRange;
                }
            }

            result.OnDeserializedMethod(serializer.Context);

            return result;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            HourRanges v = (HourRanges)value;

            writer.WriteStartArray();
            for (int i = 0; i < HourRanges.Size; i++)
            {
                TimeRange tr = v[i];
                bool enabled = tr != null;
                writer.WriteStartObject();
                writer.WritePropertyName("index");
                writer.WriteValue(i + 1);
                writer.WritePropertyName("enabled");
                writer.WriteValue(enabled);
                writer.WritePropertyName("from");
                writer.WriteValue((enabled) ? tr.Start.ToString() : "00:00");
                writer.WritePropertyName("to");
                writer.WriteValue((enabled) ? tr.End.ToString() : "00:00");

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        #endregion Methods
    }
}