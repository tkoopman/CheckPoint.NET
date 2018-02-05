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

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;
using System;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// String case to write values in.
    /// </summary>
    public enum StringCases
    {
        NoChange,
        Uppercase,
        Lowercase
    }

    /// <summary>
    /// Converts an <see cref="Enum"/> to and from its name string value.
    /// </summary>
    internal class EnumConverter : JsonConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumConverter" /> class.
        /// </summary>
        public EnumConverter() : this(StringCases.NoChange, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumConverter" /> class.
        /// </summary>
        /// <param name="outputCase">The string case to use when writing the value.</param>
        public EnumConverter(StringCases outputCase) : this(outputCase, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumConverter" /> class.
        /// </summary>
        /// <param name="outputCase">The string case to use when writing the value.</param>
        /// <param name="outputSplitWords">if set to <c>true</c> CamelCase Enum values will be split into separate words.</param>
        public EnumConverter(StringCases outputCase, string splitWordsWith)
        {
            OutputCase = outputCase;
            SplitWordsWith = splitWordsWith;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets string case to use when writing the value.
        /// </summary>
        public StringCases OutputCase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CamelCase Enum values will be split into separate words.
        /// </summary>
        public string SplitWordsWith { get; set; }

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

            return t.IsEnum;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
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
                    string enumText = reader.Value.ToString();

                    if (enumText == string.Empty && isNullable)
                    {
                        return null;
                    }

                    // Remove SplitWordsWith from value so they can be matched to CamelCase Enum values
                    if (SplitWordsWith != null)
                    {
                        enumText = enumText.Replace(SplitWordsWith, "");
                    }

                    return Enum.Parse(enumType: t, value: enumText, ignoreCase: true);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    throw new JsonSerializationException($"Integer value {reader.Value} is not allowed.");
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Error converting value {reader.Value} to type '{objectType}'.");
            }

            throw new JsonSerializationException($"Error converting value {reader.Value} to type '{objectType}'.");
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            string v = e.ToString();
            if (SplitWordsWith != null)
            {
                v = v.CamelCaseToRegular(SplitWordsWith);
            }

            switch (OutputCase)
            {
                case StringCases.Lowercase:
                    v = v.ToLower();
                    break;

                case StringCases.Uppercase:
                    v = v.ToUpper();
                    break;
            }

            writer.WriteValue(v);
        }

        private static bool GetIsNullable(Type objectType)
        {
            return (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        #endregion Methods
    }
}