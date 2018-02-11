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

using Newtonsoft.Json;
using System;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Converts bool values into and from custom string equivalent values.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class CustomBoolConverter : JsonConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomBoolConverter" /> class.
        /// </summary>
        /// <param name="true">The string that represents a true value.</param>
        /// <param name="false">The string that represents a false value.</param>
        /// <exception cref="ArgumentNullException">true or false</exception>
        public CustomBoolConverter(string @true, string @false)
        {
            True = @true ?? throw new ArgumentNullException(nameof(@true));
            False = @false ?? throw new ArgumentNullException(nameof(@false));
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the string that represents a false value.
        /// </summary>
        public string False { get; set; }

        /// <summary>
        /// Gets or sets the string that represents a true value.
        /// </summary>
        public string True { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            Type t = (GetIsNullable(objectType))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return typeof(bool) == t;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!GetIsNullable(objectType))
                {
                    throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
                }

                return null;
            }

            bool isNullable = GetIsNullable(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string value = reader.Value.ToString();

                    if (value == string.Empty && isNullable)
                    {
                        return null;
                    }

                    return value.Equals(True, StringComparison.CurrentCultureIgnoreCase);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    throw new JsonSerializationException($"Integer value {reader.Value} is not allowed.");
                }
            }
            catch (Exception)
            {
                throw new JsonSerializationException($"Error converting value {reader.Value} to type '{objectType}'.");
            }

            throw new JsonSerializationException($"Error converting value {reader.Value} to type '{objectType}'.");
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            bool b = (bool)value;

            writer.WriteValue((b) ? True : False);
        }

        private static bool GetIsNullable(Type objectType)
        {
            return (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        #endregion Methods
    }
}