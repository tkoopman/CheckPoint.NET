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

using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Contains the Hour Ranges of a Time object
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.SimpleChangeTracking" />
    [JsonConverter(typeof(HourRangesConverter))]
    public class HourRanges : SimpleChangeTracking, IEnumerable<TimeRange>
    {
        #region Fields

        /// <summary>
        /// The number of hour ranges allowed
        /// </summary>
        internal const int Size = 3;

        private TimeRange[] items = new TimeRange[Size];

        #endregion Fields

        #region Indexers

        /// <summary>
        /// Gets or sets the <see cref="TimeRange" /> at the specified index.
        /// </summary>
        /// <value>The <see cref="TimeRange" />.</value>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="TimeRange" /></returns>
        public TimeRange this[int index]
        {
            get => items[index];
            set
            {
                items[index] = value;
                OnPropertyChanged();
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TimeRange> GetEnumerator() => ((IEnumerable<TimeRange>)items).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TimeRange>)items).GetEnumerator();

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        protected override void OnDeserializing()
        {
            // Clear existing TimeRanges before Deserializing
            for (int i = 0; i < Size; i++)
            {
                items[i] = null;
            }
        }

        #endregion Methods
    }
}