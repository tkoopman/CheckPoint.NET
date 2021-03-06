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

using Koopman.CheckPoint.AccessRules;
using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Special;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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

        private List<IObjectSummary> cache = new List<IObjectSummary>();
        private List<GenericMember> cacheGeneric = new List<GenericMember>();

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

        public override bool CanConvert(Type objectType) => typeof(IObjectSummary).GetTypeInfo().IsAssignableFrom(objectType);

        /// <summary>
        /// Gets object from cache or if not in cache will return the defaultObject.
        /// </summary>
        /// <param name="uid">The uid to find in cache.</param>
        /// <returns></returns>
        public IObjectSummary GetFromCache(string uid)
        {
            foreach (var obj in cache)
                if (obj.UID.Equals(uid)) return obj;

            return null;
        }

        /// <summary>
        /// This will call UpdateGenericMembers(ObjectConverter) on each returned object if
        /// HasUpdatedGenericMembers equals false. Each object can then do what ever it needs to
        /// update things like groups with the cached version if one exists. This is used in cases
        /// where the UID was used before the object matching the UID was details in the JSON response.
        /// </summary>
        /// <remarks>
        /// This needs to be called manually wherever ObjectConverter is used before returning results.
        /// </remarks>
        public void PostDeserilization(object o)
        {
            // Nothing to update if either cache is empty so lets not waste time looking
            if (o == null || cacheGeneric.Count == 0 || cache.Count == 0) return;

            if (o is IEnumerable collection)
            {
                foreach (object obj in collection)
                    PostDeserilization(obj);
            }
            else
            {
                var type = o.GetType().GetTypeInfo();
                var property = type.GetAllProperties().FirstOrDefault(
                            p => p.Name.Equals("HasUpdatedGenericMembers"));

                if (property == null || property.GetValue(o).Equals(true)) return;

                var method = type.GetAllMethods().FirstOrDefault(
                            c => c.Name.Equals("UpdateGenericMembers") &&
                            c.GetParameters().Length == 1 &&
                            c.GetParameters().First().ParameterType == typeof(ObjectConverter));

                if (method == null) return;

                method.Invoke(o, new object[] { this });
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IObjectSummary result;
            if (reader.TokenType == JsonToken.StartObject)
            {
                var obj = serializer.Deserialize<JObject>(reader);
                string uid = obj.GetValue("uid").ToString();

                if (IsSpecialObject(uid, objectType, out result))
                    return result;

                if (existingValue != null)
                    ((IObjectSummary)existingValue).SetProperty("DetailLevel", GetDetailLevel(reader));
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
                    string type = obj.GetValue("type").ToString();
                    switch (type)
                    {
                        case "access-layer":
                            result = (existingValue == null) ? new AccessLayer(Session, GetDetailLevel(reader)) : (AccessLayer)existingValue;
                            break;

                        case "access-rule":
                            result = (existingValue == null) ? new AccessRule(Session, GetDetailLevel(reader)) : (AccessRule)existingValue;
                            break;

                        case "access-section":
                            result = (existingValue == null) ? new AccessSection(Session) : (AccessSection)existingValue;
                            break;

                        case "address-range":
                            result = (existingValue == null) ? new AddressRange(Session, GetDetailLevel(reader)) : (AddressRange)existingValue;
                            break;

                        case "application-site-category":
                            result = (existingValue == null) ? new ApplicationCategory(Session, GetDetailLevel(reader)) : (ApplicationCategory)existingValue;
                            break;

                        case "application-site-group":
                            result = (existingValue == null) ? new ApplicationGroup(Session, GetDetailLevel(reader)) : (ApplicationGroup)existingValue;
                            break;

                        case "application-site":
                            result = (existingValue == null) ? new ApplicationSite(Session, GetDetailLevel(reader)) : (ApplicationSite)existingValue;
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

                        case "service-dce-rpc":
                            result = (existingValue == null) ? new ServiceDceRpc(Session, GetDetailLevel(reader)) : (ServiceDceRpc)existingValue;
                            break;

                        case "service-icmp":
                            result = (existingValue == null) ? new ServiceICMP(Session, GetDetailLevel(reader)) : (ServiceICMP)existingValue;
                            break;

                        case "service-icmp6":
                            result = (existingValue == null) ? new ServiceICMP6(Session, GetDetailLevel(reader)) : (ServiceICMP6)existingValue;
                            break;

                        case "service-other":
                            result = (existingValue == null) ? new ServiceOther(Session, GetDetailLevel(reader)) : (ServiceOther)existingValue;
                            break;

                        case "service-sctp":
                            result = (existingValue == null) ? new ServiceSCTP(Session, GetDetailLevel(reader)) : (ServiceSCTP)existingValue;
                            break;

                        case "service-tcp":
                            result = (existingValue == null) ? new ServiceTCP(Session, GetDetailLevel(reader)) : (ServiceTCP)existingValue;
                            break;

                        case "service-rpc":
                            result = (existingValue == null) ? new ServiceRPC(Session, GetDetailLevel(reader)) : (ServiceRPC)existingValue;
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

                        case "CpmiAppfwLimit":
                            result = (existingValue == null) ? new CpmiAppfwLimit(Session, GetDetailLevel(reader)) : (CpmiAppfwLimit)existingValue;
                            break;

                        case "CpmiGatewayCluster":
                            result = (existingValue == null) ? new CpmiGatewayCluster(Session, GetDetailLevel(reader)) : (CpmiGatewayCluster)existingValue;
                            break;

                        case "CpmiClusterMember":
                            result = (existingValue == null) ? new CpmiClusterMember(Session, GetDetailLevel(reader)) : (CpmiClusterMember)existingValue;
                            break;

                        case "CpmiVsxClusterNetobj":
                            result = (existingValue == null) ? new CpmiVsxClusterNetobj(Session, GetDetailLevel(reader)) : (CpmiVsxClusterNetobj)existingValue;
                            break;

                        case "CpmiVsClusterMember":
                            result = (existingValue == null) ? new CpmiVsClusterMember(Session, GetDetailLevel(reader)) : (CpmiVsClusterMember)existingValue;
                            break;

                        case "CpmiVsClusterNetobj":
                            result = (existingValue == null) ? new CpmiVsClusterNetobj(Session, GetDetailLevel(reader)) : (CpmiVsClusterNetobj)existingValue;
                            break;

                        case "CpmiVsxClusterMember":
                            result = (existingValue == null) ? new CpmiVsxClusterMember(Session, GetDetailLevel(reader)) : (CpmiVsxClusterMember)existingValue;
                            break;

                        case "CpmiHostCkp":
                            result = (existingValue == null) ? new CpmiHostCkp(Session, GetDetailLevel(reader)) : (CpmiHostCkp)existingValue;
                            break;

                        case "CpmiGatewayPlain":
                            result = (existingValue == null) ? new CpmiGatewayPlain(Session, GetDetailLevel(reader)) : (CpmiGatewayPlain)existingValue;
                            break;

                        case "":
                            return null;

                        default:
                            result = (existingValue == null) ? new GenericObjectSummary(Session, GetDetailLevel(reader), type) : (GenericObjectSummary)existingValue;
                            break;
                    }

                    if (result.UID == null)
                        result.SetProperty(nameof(IObjectSummary.UID), uid);
                    cache.Add(result);
                }
                else
                {
                    result.SetProperty(nameof(IObjectSummary.DetailLevel), GetDetailLevel(reader));
                }

                serializer.Populate(obj.CreateReader(), result);
                return result;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                string uid = serializer.Deserialize<string>(reader);

                if (IsSpecialObject(uid, objectType, out result))
                    return result;

                result = GetFromCache(uid);
                if (result != null) return result;

                foreach (var obj in cacheGeneric)
                    if (
                        obj.UID.Equals(uid) &&
                        objectType.GetTypeInfo().IsAssignableFrom(obj.GetType())
                       ) return obj;

                if (objectType.GetTypeInfo().IsClass)
                {
                    var ci = objectType.GetTypeInfo().DeclaredConstructors.Single(
                        c => c.GetParameters().Length == 2 &&
                        c.GetParameters().First().ParameterType == typeof(Session) &&
                        c.GetParameters().Last().ParameterType == typeof(DetailLevels));

                    if (ci == null) { throw new Exception("Unable to find constructor that accepts Session, DetailLevels parameters"); }
                    result = (IObjectSummary)ci.Invoke(new object[] { Session, DetailLevels.UID });
                    result.SetProperty("UID", uid);
                    if (result is SimpleChangeTracking sct)
                    {
                        sct.OnDeserializingMethod(default);
                        sct.OnDeserializedMethod(default);
                    }
                    cache.Add(result);

                    return result;
                }

                var generic = new GenericMember(Session, uid);
                cacheGeneric.Add(generic);
                return generic;
            }
            else throw CreateJsonReaderException(reader, $"Invalid token type found. Type: {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

        private JsonReaderException CreateJsonReaderException(JsonReader reader, string message)
        {
            if (reader is JsonTextReader textReader)
                return new JsonReaderException(message, reader.Path, textReader.LineNumber, textReader.LinePosition, null);

            return new JsonReaderException(message);
        }

        private DetailLevels GetDetailLevel(JsonReader reader) => (reader.Depth == 0) ? ParentDetailLevel : ChildDetailLevel;

        private bool IsSpecialObject(string uid, Type objectType, out IObjectSummary obj)
        {
            if (objectType.Equals(typeof(RulebaseAction)))
            {
                foreach (var action in RulebaseAction.Actions)
                    if (action.UID.Equals(uid))
                    {
                        obj = action;
                        return true;
                    }
                throw new JsonException($"Invalid rulebase action uid: {uid}");
            }

            if (objectType.Equals(typeof(TrackType)))
            {
                foreach (var t in TrackType.Types)
                    if (t.UID.Equals(uid))
                    {
                        obj = t;
                        return true;
                    }
                throw new JsonException($"Invalid track type uid: {uid}");
            }

            foreach (var inbuilt in GenericObjectSummary.InBuilt)
                if (inbuilt.UID.Equals(uid))
                {
                    if (objectType.GetTypeInfo().IsAssignableFrom(inbuilt.GetType()))
                        obj = inbuilt;
                    else if (inbuilt.AllowNullResult)
                        obj = null;
                    else
                        throw new InvalidCastException($"Unable to cast {inbuilt.GetType()} to {objectType}.");
                    return true;
                }

            obj = null;
            return false;
        }

        #endregion Methods
    }
}