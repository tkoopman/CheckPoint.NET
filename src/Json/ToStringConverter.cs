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

using Newtonsoft.Json;
using System;
using System.Collections;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used to output the ObjectSummary.Name or of that is empty the ObjectSummary.UID
    /// </summary>
    internal class ToStringConverter : JsonConverter
    {
        #region Fields

        internal static readonly ToStringConverter Instance = new ToStringConverter();
        internal static readonly ToStringConverter UIDInstance = new ToStringConverter(true);

        #endregion Fields

        #region Constructors

        public ToStringConverter(bool forceUID = false)
        {
            ForceUID = forceUID;
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public bool ForceUID { get; }

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType) => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    break;

                case IObjectSummary obj:
                    if (ForceUID)
                        writer.WriteValue(obj.UID);
                    else
                        writer.WriteValue(obj.ToString());
                    break;

                case IEnumerable e:
                    writer.WriteStartArray();
                    foreach (object v in e)
                        WriteJson(writer, v, serializer);
                    writer.WriteEndArray();
                    break;

                case DateTime dt:
                    writer.WriteValue(dt.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    break;

                default:
                    writer.WriteValue(value.ToString());
                    break;
            }
        }

        #endregion Methods
    }
}