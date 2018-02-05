// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used for reading Check Point objects in and creating them using the Class that matches the specified type.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class ObjectConverter : JsonConverter
    {
        #region Fields

        private Session _Session;
        private DetailLevels ChildDetailLevel;
        private DetailLevels ParentDetailLevel;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConverter"/> class.
        /// </summary>
        /// <param name="Session">The current active session to management server.</param>
        /// <param name="parentDetailLevel">Detail level returned for top level objects.</param>
        /// <param name="childDetailLevel">Detail level returned for all child objects.</param>
        public ObjectConverter(Session Session, DetailLevels parentDetailLevel, DetailLevels childDetailLevel)
        {
            _Session = Session;
            ParentDetailLevel = parentDetailLevel;
            ChildDetailLevel = childDetailLevel;
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead => true;

        public override bool CanWrite => false;

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectSummary).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);

            if (existingValue != null)
            {
                ((ObjectSummary)existingValue).DetailLevel = GetDetailLevel(reader);
            }

            ObjectSummary result;

            // Create object using correct class that matches the type, or
            // if existingValue provided make sure it can be cast to the correct type
            switch (obj.GetValue("type").ToString())
            {
                case "address-range":
                    result = (existingValue == null) ? new AddressRange(_Session, GetDetailLevel(reader)) : (AddressRange)existingValue;
                    break;

                case "group":
                    result = (existingValue == null) ? new Group(_Session, GetDetailLevel(reader)) : (Group)existingValue;
                    break;

                case "group-with-exclusion":
                    result = (existingValue == null) ? new GroupWithExclusion(_Session, GetDetailLevel(reader)) : (GroupWithExclusion)existingValue;
                    break;

                case "host":
                    result = (existingValue == null) ? new Host(_Session, GetDetailLevel(reader)) : (Host)existingValue;
                    break;

                case "multicast-address-range":
                    result = (existingValue == null) ? new MulticastAddressRange(_Session, GetDetailLevel(reader)) : (MulticastAddressRange)existingValue;
                    break;

                case "network":
                    result = (existingValue == null) ? new Network(_Session, GetDetailLevel(reader)) : (Network)existingValue;
                    break;

                case "tag":
                    result = (existingValue == null) ? new Tag(_Session, GetDetailLevel(reader)) : (Tag)existingValue;
                    break;

                case "":
                    // Not sure what to do with these like 'Trust_all_action' group returned with AddressRange 'LocalMachine_Loopback'
                    return null;

                default:
                    result = (existingValue == null) ? new ObjectSummary(_Session, GetDetailLevel(reader)) : (ObjectSummary)existingValue;
                    break;
            }

            serializer.Populate(obj.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private DetailLevels GetDetailLevel(JsonReader reader)
        {
            return (reader.Depth == 0) ? ParentDetailLevel : ChildDetailLevel;
        }

        #endregion Methods
    }
}