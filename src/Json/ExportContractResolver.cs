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
using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used to make sure Include and Except properties just output the name or UID of the object.
    /// </summary>
    internal class ExportContractResolver : DefaultContractResolver
    {
        #region Fields

        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly ExportContractResolver Instance = new ExportContractResolver();

        #endregion Fields

        #region Methods

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) =>
            base.CreateProperties(type, memberSerialization).OrderBy(p => p.Order).ThenBy(p => p.PropertyName).ToList();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (typeof(IObjectSummary).GetTypeInfo().IsAssignableFrom(member.DeclaringType))
            {
                property.Order = 100;
                switch (member.Name)
                {
                    case nameof(IObjectSummary.DetailLevel):
                    case nameof(IObjectSummary.IsNew):
                    case nameof(ObjectBase<IObjectSummary>.Icon):
                    case nameof(ObjectSummary<IObjectSummary>.OldName):
                    case nameof(AccessRule.Position):
                    case "SetIfExists":
                    case "AddDefaultRule":
                        property.Ignored = true;
                        break;

                    case nameof(IObjectSummary.UID):
                        property.Order = 1;
                        break;

                    case nameof(IObjectSummary.Name):
                        property.Order = 2;
                        break;

                    case nameof(IObjectSummary.Type):
                        property.Order = 3;
                        break;

                    case nameof(AccessRule.Layer):
                    case nameof(AccessRule.InlineLayer):
                    case nameof(AccessSection.Rulebase):
                    case nameof(AccessSection.Objects):
                        property.Converter = ToStringConverter.UIDInstance;
                        break;

                    case nameof(IObjectSummary.Domain):
                    case nameof(ObjectBase<IObjectSummary>.Tags):
                    case nameof(AccessRule.Action):
                        property.Converter = ToStringConverter.Instance;
                        break;

                    default:
                        if (
                            property.PropertyType.GetTypeInfo().IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof(MemberMembershipChangeTracking<>)
                            )
                            property.Converter = ToStringConverter.UIDInstance;

                        if (typeof(IEnumerable<object>).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                            property.ShouldSerialize = instance =>
                                {
                                    var p = instance.GetType().GetTypeInfo().GetAllProperties().FirstOrDefault(prop => prop.Name.Equals(property.UnderlyingName));
                                    object value = p?.GetValue(instance);
                                    if (value is IEnumerable<object> e)
                                        return e.Any();

                                    return true;
                                };
                        break;
                }
            }
            else if (member.DeclaringType == typeof(WhereUsed.WhereUsedResults) && member.Name.Equals(nameof(WhereUsed.WhereUsedResults.Objects)))
            {
                property.Converter = ToStringConverter.UIDInstance;
            }
            else if (member.DeclaringType == typeof(WhereUsed.WhereUsedResults.Rules) &&
                (
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.Rules.Layer))  ||
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.Rules.Rule))   ||
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.Rules.Package))
                ))
            {
                property.Converter = ToStringConverter.UIDInstance;
            }
            else if (member.DeclaringType == typeof(WhereUsed.WhereUsedResults.NATs) &&
                (
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.NATs.Rule))   ||
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.NATs.Package))
                ))
            {
                property.Converter = ToStringConverter.UIDInstance;
            }
            else if (member.DeclaringType == typeof(WhereUsed.WhereUsedResults.ThreatRules) &&
                (
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.ThreatRules.Layer))  || 
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.ThreatRules.Rule))   ||
                    member.Name.Equals(nameof(WhereUsed.WhereUsedResults.ThreatRules.Package))
                ))
            {
                property.Converter = ToStringConverter.UIDInstance;
            }
            else if (member.DeclaringType == typeof(AccessRules.Track) && member.Name.Equals(nameof(AccessRules.Track.Type)))
            {
                property.Converter = ToStringConverter.Instance;
            }
            else if (member.DeclaringType == typeof(JsonExport) && member.Name.Equals(nameof(JsonExport.WhereUsed)))
            {
                property.ShouldSerialize = instance =>
                {
                    if (instance is JsonExport export)
                        return export.WhereUsed.Count > 0;

                    throw new Exception("Should not hit this, ever");
                };
            }

            if (property.Converter?.GetType() == typeof(CheckPointDateTimeConverter))
                property.Converter = ToStringConverter.Instance;

            return property;
        }

        #endregion Methods
    }
}