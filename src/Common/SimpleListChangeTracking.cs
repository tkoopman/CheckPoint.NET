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
    public class SimpleListChangeTracking<T> : SimpleChangeTracking, IList<T>
    {
        #region Fields

        private List<T> items = new List<T>();

        #endregion Fields

        #region Properties

        public int Count => ((IList<T>)items).Count;

        public bool IsReadOnly => ((IList<T>)items).IsReadOnly;

        #endregion Properties

        #region Indexers

        public T this[int index]
        {
            get => ((IList<T>)items)[index];
            set
            {
                ((IList<T>)items)[index] = value;
                OnPropertyChanged();
            }
        }

        public T this[string str]
        {
            get
            {
                foreach (T i in items)
                {
                    if (i.ToString().Equals(str)) return i;
                }

                return default(T);
            }
        }

        #endregion Indexers

        #region Methods

        public void Add(T item)
        {
            ((IList<T>)items).Add(item);
            OnPropertyChanged();
        }

        public void Clear()
        {
            ((IList<T>)items).Clear();
            OnPropertyChanged();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)items).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)items).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)items).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)items).Insert(index, item);
            OnPropertyChanged();
        }

        public bool Remove(T item)
        {
            OnPropertyChanged();
            return ((IList<T>)items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)items).RemoveAt(index);
            OnPropertyChanged();
        }

        protected override void OnDeserializing()
        {
            items.Clear();
        }

        #endregion Methods
    }
}