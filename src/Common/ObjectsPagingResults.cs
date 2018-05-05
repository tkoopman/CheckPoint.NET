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
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Result from commands that return multiple objects.
    /// </summary>
    /// <typeparam name="T">Type of the result objects</typeparam>
    /// <typeparam name="U">ObjectsPagingResults used for T</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    [JsonObject]
    public abstract class ObjectsPagingResults<T, U> : IEnumerable<T> where U : ObjectsPagingResults<T, U>
    {
        #region Fields

        /// <summary>
        /// List of objects that other classes should implement a public property to expose.
        /// </summary>
        protected List<T> _Objects;

        #endregion Fields

        #region Properties

        /// <summary>
        /// From which element number the query was done.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public int From { get; set; }

        /// <summary>
        /// To which element number the query was done.
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public int To { get; set; }

        /// <summary>
        /// Total number of elements found by the query.
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the function used to move to the next page of results.
        /// </summary>
        protected internal Func<CancellationToken, Task<U>> Next { get; set; }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public T this[int index] { get => ((IList<T>)_Objects)[index]; set => throw new System.NotImplementedException($"Read-Only"); }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection of Objects.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_Objects).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection of Objects.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_Objects).GetEnumerator();

        /// <summary>
        /// Gets the next page of results from management server.
        /// </summary>
        /// <returns>Returns the next page of results. Null if no next page to return.</returns>
        public async Task<U> NextPage(CancellationToken ct = default)
        {
            if (Next == null)
                return null;

            return await Next(ct);
        }

        #endregion Methods
    }
}