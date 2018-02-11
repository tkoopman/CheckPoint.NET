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
using System;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Convert NAT Settings object to JSON.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class NATSettingsConverter : JsonConverter
    {
        #region Methods

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NATSettings);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            NATSettings nat = (NATSettings)value;

            writer.WriteStartObject();
            writer.WritePropertyName("auto-rule");
            writer.WriteValue(nat.AutoRule);

            if (nat.AutoRule)
            {
                writer.WritePropertyName("install-on");
                writer.WriteValue(nat.InstallOn);

                writer.WritePropertyName("method");
                serializer.Serialize(writer, nat.Method);

                if (nat.Method == NATSettings.NATMethods.Hide)
                {
                    writer.WritePropertyName("hide-behind");
                    serializer.Serialize(writer, nat.HideBehind);
                }

                if (nat.Method == NATSettings.NATMethods.Static || (nat.Method == NATSettings.NATMethods.Hide && nat.HideBehind == NATSettings.HideBehindValues.IPAddress))
                {
                    writer.WritePropertyName("ipv4-address");
                    writer.WriteValue(nat.IPv4Address?.ToString());

                    writer.WritePropertyName("ipv6-address");
                    writer.WriteValue(nat.IPv6Address?.ToString());
                }
            }

            writer.WriteEndObject();
        }

        #endregion Methods
    }
}