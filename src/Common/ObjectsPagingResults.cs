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
using System.Collections;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// <para type="description">Result from commands that return multiple objects.</para>
    /// </summary>
    [JsonObject]
    public abstract class ObjectsPagingResults<T> : IEnumerable<T>
    {
        #region Fields

        /// <summary>
        /// List of objects that other classes should implement a public property to expose.
        /// </summary>
        protected List<T> _Objects;

        #endregion Fields

        #region Properties

        /// <summary>
        /// <para type="description">From which element number the query was done.</para>
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public int From { get; set; }

        /// <summary>
        /// <para type="description">To which element number the query was done.</para>
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public int To { get; set; }

        /// <summary>
        /// <para type="description">Total number of elements returned by the query.</para>
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        protected internal Func<NetworkObjectsPagingResults<T>> Next { get; set; }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets the <see cref="T" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T this[int index] { get => ((IList<T>)_Objects)[index]; set { throw new System.NotImplementedException($"Read-Only"); } }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection of Objects.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_Objects).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of Objects.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_Objects).GetEnumerator();
        }

        /// <summary>
        /// Gets the next page of results from management server.
        /// </summary>
        public NetworkObjectsPagingResults<T> NextPage()
        {
            if (Next == null) { return null; }

            return Next();
        }

        #endregion Methods
    }
}