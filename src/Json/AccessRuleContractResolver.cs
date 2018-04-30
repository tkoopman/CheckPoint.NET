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

using Koopman.CheckPoint.AccessRules;
using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Special;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used to make sure Include and Except properties just output the name or UID of the object.
    /// </summary>
    internal class AccessRuleContractResolver : ChangeTrackingContractResolver
    {
        #region Fields

        /// <summary>
        /// Default instance to be used when adding new objects.
        /// </summary>
        public new static readonly AccessRuleContractResolver AddInstance = new AccessRuleContractResolver() { SetMethod = false };

        /// <summary>
        /// Default instance to be used when updating existing objects.
        /// </summary>
        public new static readonly AccessRuleContractResolver SetInstance = new AccessRuleContractResolver() { SetMethod = true };

        #endregion Fields

        #region Methods

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(RulebaseAction) ||
                property.PropertyType == typeof(TrackType) ||
                property.PropertyType == typeof(CpmiAppfwLimit))
                property.Converter = ToStringConverter.Instance;
            else if (property.UnderlyingName.Equals(nameof(AccessRule.Layer)))
            {
                property.Converter = ToStringConverter.Instance;
                property.ShouldSerialize = null;
            }
            else if (property.UnderlyingName.Equals(nameof(AccessRule.Position)))
            {
                property.ShouldSerialize =
                        instance =>
                        {
                            var c = (ChangeTracking)instance;
                            return c.IsNew || c.IsPropertyChanged(member.Name);
                        };
                if (SetMethod)
                    property.PropertyName = "new-position";
            }
            else if (property.UnderlyingName.Equals(nameof(IObjectSummary.UID)))
            {
                property.ShouldSerialize =
                        instance =>
                        {
                            var o = (IObjectSummary)instance;
                            return !o.IsNew;
                        };
            }
            else if (property.UnderlyingName.Equals("OldName"))
            {
                property.ShouldSerialize = null;
                property.Ignored = true;
            }

            return property;
        }

        #endregion Methods
    }
}