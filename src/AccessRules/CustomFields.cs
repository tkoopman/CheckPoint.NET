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
using System.Collections;
using System.Collections.Generic;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Custom fields assigned to rule
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    [JsonObject]
    public class CustomFields : ChangeTracking, IEnumerable<string>
    {
        #region Fields

        /// <summary>
        /// The number of custom fields allowed
        /// </summary>
        private const int Size = 3;

        private string[] fields = new string[Size];

        #endregion Fields

        #region Indexers

        /// <summary>
        /// Gets or sets the custom field at the specified index.
        /// </summary>
        /// <value>The custom field value.</value>
        /// <param name="index">The index.</param>
        /// <returns>The custom field value.</returns>
        public string this[int index]
        {
            get => fields[index];
            set
            {
                fields[index] = value;
                OnPropertyChanged($"Field{index}");
            }
        }

        #endregion Indexers

        #region Properties

        /// <summary>
        /// Gets or sets the field1.
        /// </summary>
        /// <value>The field1.</value>
        [JsonProperty(PropertyName = "field-1")]
        public string Field1
        {
            get => fields[0];
            set
            {
                fields[0] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the field2.
        /// </summary>
        /// <value>The field2.</value>
        [JsonProperty(PropertyName = "field-2")]
        public string Field2
        {
            get => fields[1];
            set
            {
                fields[1] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the field3.
        /// </summary>
        /// <value>The field3.</value>
        [JsonProperty(PropertyName = "field-3")]
        public string Field3
        {
            get => fields[2];
            set
            {
                fields[2] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)fields).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<string>)fields).GetEnumerator();

        #endregion Properties
    }
}