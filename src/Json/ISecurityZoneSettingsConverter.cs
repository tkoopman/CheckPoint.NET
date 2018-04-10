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
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used for reading Check Point objects in and creating them using the Class that matches the
    /// specified type.
    /// </summary>
    internal class ISecurityZoneSettingsConverter : JsonConverter
    {
        #region Properties

        public override bool CanRead => true;

        public override bool CanWrite => false;

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType) => typeof(ISecurityZoneSettings).GetTypeInfo().IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<JObject>(reader);

            ISecurityZoneSettings result;

            if (obj.GetValue("specific-zone") == null)
            {
                result = new SimpleGatewaySettings.SecurityZone.AutoCalculated();
            }
            else
            {
                result = new SimpleGatewaySettings.SecurityZone.SpecificZone();
                serializer.Populate(obj.CreateReader(), result);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

        #endregion Methods
    }
}