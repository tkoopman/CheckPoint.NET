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
using System.Collections.Generic;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// For creating Check Point JSON request of add or removing items from lists like groups and tags.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class MembershipChangeTrackingConverter : JsonConverter
    {
        #region Properties

        public override bool CanRead => false;

        public override bool CanWrite => true;

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType)
        {
            return (objectType.IsGenericType) ?
                typeof(MembershipChangeTracking<>) == objectType.GetGenericTypeDefinition() ||
                typeof(ObjectMembershipChangeTracking<>) == objectType.GetGenericTypeDefinition()
                : false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ChangeAction Action = (ChangeAction)value.GetType().GetProperty("Action", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(value);
            List<string> ChangedMembers = (List<string>)value.GetType().GetProperty("ChangedMembers", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(value);

            if (Action == ChangeAction.Set)
            {
                // Replace any existing members with new list, or clearing all to empty list.
                serializer.Serialize(writer, ChangedMembers);
            }
            else if (Action == ChangeAction.None)
            {
                // Check Point makes no changes when null sent.
                writer.WriteNull();
            }
            else
            {
                // Output list of add/removed members
                string action = (Action == ChangeAction.Add) ? "add" : "remove";
                writer.WriteStartObject();
                writer.WritePropertyName(action);
                serializer.Serialize(writer, ChangedMembers);
                writer.WriteEndObject();
            }
        }

        #endregion Methods
    }
}