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

using System.Collections;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// To keep track of if changes have been made to a list of objects
    /// </summary>
    /// <typeparam name="T">Type of list object</typeparam>
    /// <seealso cref="Koopman.CheckPoint.Common.SimpleChangeTracking" />
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    public class SimpleListChangeTracking<T> : SimpleChangeTracking, IList<T>
    {
        #region Fields

        private List<T> items = new List<T>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public int Count => ((IList<T>)items).Count;

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        public bool IsReadOnly => ((IList<T>)items).IsReadOnly;

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public T this[int index]
        {
            get => ((IList<T>)items)[index];
            set
            {
                ((IList<T>)items)[index] = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the object with the specified string.
        /// </summary>
        /// <param name="str">The string to match to ToString() method of object.</param>
        /// <returns>First object that's ToString() value equals <paramref name="str" />.</returns>
        public T this[string str]
        {
            get
            {
                foreach (var i in items)
                    if (i.ToString().Equals(str)) return i;

                return default;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        public void Add(T item)
        {
            ((IList<T>)items).Add(item);
            OnPropertyChanged();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Clear()
        {
            ((IList<T>)items).Clear();
            OnPropertyChanged();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see>
        /// contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> is found in the
        /// <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(T item) => ((IList<T>)items).Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see>
        /// to an <see cref="T:System.Array"></see>, starting at a particular
        /// <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"></see> that is the destination of the
        /// elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The
        /// <see cref="T:System.Array"></see> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => ((IList<T>)items).CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => ((IList<T>)items).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IList<T>)items).GetEnumerator();

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>
        /// The index of <paramref name="item">item</paramref> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item) => ((IList<T>)items).IndexOf(item);

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the
        /// specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        public void Insert(int index, T item)
        {
            ((IList<T>)items).Insert(index, item);
            OnPropertyChanged();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> was successfully removed from the
        /// <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This
        /// method also returns false if <paramref name="item">item</paramref> is not found in the
        /// original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        public bool Remove(T item)
        {
            OnPropertyChanged();
            return ((IList<T>)items).Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            ((IList<T>)items).RemoveAt(index);
            OnPropertyChanged();
        }

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        protected override void OnDeserializing() => items.Clear();

        #endregion Methods
    }
}