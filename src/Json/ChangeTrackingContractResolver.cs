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
using System.ComponentModel;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used to set which properties should be serialised on objects that inherit ChangeTracking class.
    /// </summary>
    internal class ChangeTrackingContractResolver : DefaultContractResolver
    {
        #region Fields

        /// <summary>
        /// Default instance to be used when adding new objects.
        /// </summary>
        public static readonly ChangeTrackingContractResolver AddInstance = new ChangeTrackingContractResolver() { SetMethod = false };

        /// <summary>
        /// Default instance to be used when updating existing objects.
        /// </summary>
        public static readonly ChangeTrackingContractResolver SetInstance = new ChangeTrackingContractResolver() { SetMethod = true };

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this is a set post.
        /// </summary>
        /// <value><c>true</c> if [set method]; otherwise, <c>false</c> for add.</value>
        public bool SetMethod { get; set; } = false;

        #endregion Properties

        #region Methods

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (typeof(ChangeTracking).GetTypeInfo().IsAssignableFrom(property.DeclaringType))
            {
                bool useUID = (GlobalOptions.IdentifierForSetCalls == GlobalOptions.Identifier.UID);
                if (property.UnderlyingName.Equals(nameof(IObjectSummary.UID)))
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var o = (IObjectSummary)instance;
                            bool hasOldName = o.TryGetProperty("OldName", out object oldName, BindingFlags.NonPublic | BindingFlags.Instance);
                            hasOldName = (hasOldName && oldName != null);
                            return !o.IsNew && (GlobalOptions.IdentifierForSetCalls == GlobalOptions.Identifier.UID || !hasOldName);
                        };
                    property.NullValueHandling = NullValueHandling.Ignore;
                }
                else if (property.UnderlyingName.Equals("OldName"))
                {
                    property.ShouldSerialize =
                            instance =>
                            {
                                var o = (IObjectSummary)instance;
                                bool hasUID = o.UID != null;
                                return !o.IsNew && (GlobalOptions.IdentifierForSetCalls == GlobalOptions.Identifier.Name || !hasUID);
                            };
                    property.NullValueHandling = NullValueHandling.Ignore;
                    if (SetMethod)
                        property.PropertyName = "name";
                }
                else
                {
                    if (SetMethod && property.UnderlyingName.Equals(nameof(IObjectSummary.Name)))
                    {
                        // Make sure Name changes sent out as "new-name" As for if this property
                        // should be sent at all that will be handled by code below
                        property.PropertyName = "new-name";
                    }

                    if (typeof(IChangeTracking).GetTypeInfo().IsAssignableFrom(((PropertyInfo)member).PropertyType))
                    {
                        // For properties that implement IChangeTracking check it's IsChanged value.
                        property.ShouldSerialize =
                        instance =>
                        {
                            var c = (ChangeTracking)instance;
                            if (c.IsPropertyChanged(member.Name))
                                return true;

                            var ic = (IChangeTracking)((PropertyInfo)member).GetValue(instance);
                            if (ic == null)
                                return false;

                            return ic.IsChanged;
                        };
                    }
                    else
                    {
                        // For all other properties check if property was changed
                        property.ShouldSerialize =
                        instance =>
                        {
                            var c = (ChangeTracking)instance;
                            return c.IsPropertyChanged(member.Name);
                        };
                    }
                }
            }

            return property;
        }

        #endregion Methods
    }
}