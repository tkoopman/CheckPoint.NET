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
            property.Order = 100;

            if (typeof(JsonExport) != member.DeclaringType)
            {
                if (typeof(Domain).GetTypeInfo().IsAssignableFrom(property.PropertyType) ||
                    typeof(IEnumerable<Tag>).GetTypeInfo().IsAssignableFrom(property.PropertyType) ||
                    typeof(Tag).GetTypeInfo().IsAssignableFrom(property.PropertyType) ||
                    typeof(AccessRules.RulebaseAction).GetTypeInfo().IsAssignableFrom(property.PropertyType) ||
                    typeof(AccessRules.TrackType).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                {
                    property.Converter = ToStringConverter.Instance;
                }
                else if (typeof(IEnumerable<IObjectSummary>).GetTypeInfo().IsAssignableFrom(property.PropertyType) ||
                    typeof(IObjectSummary).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                {
                    property.Converter = ToStringConverter.UIDInstance;
                }
                if (typeof(IEnumerable<object>).GetTypeInfo().IsAssignableFrom(property.PropertyType))
                {
                    property.ShouldSerialize = instance =>
                    {
                        var p = instance.GetType().GetTypeInfo().GetAllProperties().FirstOrDefault(prop => prop.Name.Equals(property.UnderlyingName));
                        object value = p?.GetValue(instance);
                        if (value is IEnumerable<object> e)
                            return e.Any();

                        return true;
                    };
                }
            }

            if (typeof(IObjectSummary).GetTypeInfo().IsAssignableFrom(member.DeclaringType))
            {
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
                }
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