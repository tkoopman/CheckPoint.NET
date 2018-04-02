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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used for reading Check Point objects in and creating them using the Class that matches the
    /// specified type.
    /// </summary>
    internal class ObjectConverter : JsonConverter
    {
        #region Fields

        private List<ObjectSummary> cache = new List<ObjectSummary>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConverter" /> class.
        /// </summary>
        /// <param name="session">The current active session to management server.</param>
        /// <param name="parentDetailLevel">Detail level returned for top level objects.</param>
        /// <param name="childDetailLevel">Detail level returned for all child objects.</param>
        public ObjectConverter(Session session, DetailLevels parentDetailLevel, DetailLevels childDetailLevel)
        {
            Session = session;
            ParentDetailLevel = parentDetailLevel;
            ChildDetailLevel = childDetailLevel;
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead => true;

        public override bool CanWrite => false;
        private DetailLevels ChildDetailLevel { get; }
        private DetailLevels ParentDetailLevel { get; }
        private Session Session { get; }

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectSummary).GetTypeInfo().IsAssignableFrom(objectType) || typeof(IMember).GetTypeInfo().IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Gets object from cache or if not in cache will return the defaultObject.
        /// </summary>
        /// <param name="uid">The uid to find in cache.</param>
        /// <param name="defaultObject">The default object if not found in cache.</param>
        /// <returns></returns>
        public ObjectSummary GetFromCache(string uid, ObjectSummary defaultObject = null)
        {
            foreach (var obj in cache)
                if (obj.UID.Equals(uid)) return obj;

            return defaultObject;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject obj = serializer.Deserialize<JObject>(reader);
                string uid = obj.GetValue("uid").ToString();

                ObjectSummary result = null;

                if (existingValue != null)
                {
                    ((ObjectSummary)existingValue).DetailLevel = GetDetailLevel(reader);
                }
                else
                {
                    result = GetFromCache(uid);
                    if (result != null && result.DetailLevel >= GetDetailLevel(reader))
                        return result;
                }

                // Create object using correct class that matches the type, or if existingValue
                // provided make sure it can be cast to the correct type
                if (result == null)
                {
                    switch (obj.GetValue("type").ToString())
                    {
                        case "address-range":
                            result = (existingValue == null) ? new AddressRange(Session, GetDetailLevel(reader)) : (AddressRange)existingValue;
                            break;

                        case "group":
                            result = (existingValue == null) ? new Group(Session, GetDetailLevel(reader)) : (Group)existingValue;
                            break;

                        case "group-with-exclusion":
                            result = (existingValue == null) ? new GroupWithExclusion(Session, GetDetailLevel(reader)) : (GroupWithExclusion)existingValue;
                            break;

                        case "host":
                            result = (existingValue == null) ? new Host(Session, GetDetailLevel(reader)) : (Host)existingValue;
                            break;

                        case "multicast-address-range":
                            result = (existingValue == null) ? new MulticastAddressRange(Session, GetDetailLevel(reader)) : (MulticastAddressRange)existingValue;
                            break;

                        case "network":
                            result = (existingValue == null) ? new Network(Session, GetDetailLevel(reader)) : (Network)existingValue;
                            break;

                        case "service-icmp":
                            result = (existingValue == null) ? new ServiceICMP(Session, GetDetailLevel(reader)) : (ServiceICMP)existingValue;
                            break;

                        case "service-icmp6":
                            result = (existingValue == null) ? new ServiceICMP6(Session, GetDetailLevel(reader)) : (ServiceICMP6)existingValue;
                            break;

                        case "service-tcp":
                            result = (existingValue == null) ? new ServiceTCP(Session, GetDetailLevel(reader)) : (ServiceTCP)existingValue;
                            break;

                        case "service-udp":
                            result = (existingValue == null) ? new ServiceUDP(Session, GetDetailLevel(reader)) : (ServiceUDP)existingValue;
                            break;

                        case "service-group":
                            result = (existingValue == null) ? new ServiceGroup(Session, GetDetailLevel(reader)) : (ServiceGroup)existingValue;
                            break;

                        case "simple-gateway":
                            result = (existingValue == null) ? new SimpleGateway(Session, GetDetailLevel(reader)) : (SimpleGateway)existingValue;
                            break;

                        case "security-zone":
                            result = (existingValue == null) ? new SecurityZone(Session, GetDetailLevel(reader)) : (SecurityZone)existingValue;
                            break;

                        case "tag":
                            result = (existingValue == null) ? new Tag(Session, GetDetailLevel(reader)) : (Tag)existingValue;
                            break;

                        case "time":
                            result = (existingValue == null) ? new Time(Session, GetDetailLevel(reader)) : (Time)existingValue;
                            break;

                        case "time-group":
                            result = (existingValue == null) ? new TimeGroup(Session, GetDetailLevel(reader)) : (TimeGroup)existingValue;
                            break;

                        case "":
                            // Not sure what to do with these. For now return null for known ones.
                            if (
                                obj.GetValue("uid").ToString().Equals(ObjectSummary.TrustAllAction.UID) ||
                                obj.GetValue("uid").ToString().Equals(ObjectSummary.RestrictCommonProtocolsAction.UID)
                                ) return null;
                            throw new NotImplementedException("Empty type objects not implemented");

                        case "CpmiAnyObject":
                            return ObjectSummary.Any;

                        default:
                            result = (existingValue == null) ? new ObjectSummary(Session, GetDetailLevel(reader)) : (ObjectSummary)existingValue;
                            break;
                    }

                    if (result.UID == null)
                        result.UID = uid;
                    cache.Add(result);
                }

                serializer.Populate(obj.CreateReader(), result);
                return result;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                string uid = serializer.Deserialize<string>(reader);
                var cached = GetFromCache(uid);
                if (cached != null) return cached;

                if (typeof(ObjectSummary).GetTypeInfo().IsAssignableFrom(objectType))
                {
                    ConstructorInfo ci = objectType.GetTypeInfo().DeclaredConstructors.Single(c => c.GetParameters().Length == 2 && c.GetParameters().First().ParameterType == typeof(Session) && c.GetParameters().Last().ParameterType == typeof(DetailLevels));
                    if (ci == null) { throw new Exception("Unable to find constructor that accepts Session, DetailLevels parameters"); }
                    cached = (ObjectSummary)ci.Invoke(new object[] { Session, DetailLevels.UID });
                    cached.UID = uid;
                    cache.Add(cached);

                    return cached;
                }

                throw CreateJsonReaderException(reader, $"Should not hit this. Type: {reader.TokenType}");
            }
            else throw CreateJsonReaderException(reader, $"Invalid token type found. Type: {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private JsonReaderException CreateJsonReaderException(JsonReader reader, string message)
        {
            if (reader is JsonTextReader)
            {
                var textReader = (JsonTextReader)reader;
                return new JsonReaderException(message, reader.Path, textReader.LineNumber, textReader.LinePosition, null);
            }
            return new JsonReaderException(message);
        }

        private DetailLevels GetDetailLevel(JsonReader reader)
        {
            return (reader.Depth == 0) ? ParentDetailLevel : ChildDetailLevel;
        }

        #endregion Methods
    }
}