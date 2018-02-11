using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Koopman.CheckPoint.Common
{
    public class ListChangeTracking<T> : SimpleChangeTracking, IList<T> where T : IChangeTracking
    {
        #region Fields

        private bool isChanged = false;

        private List<T> items = new List<T>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <value>
        /// <c>true</c> if any properties have been changed or if any contained MembershipChangeTracking properties have had membership modifications.
        /// </value>
        public override bool IsChanged
        {
            get
            {
                if (isChanged)
                {
                    return true;
                }
                else
                {
                    foreach (var i in items)
                    {
                        if (i.IsChanged)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

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
                isChanged = true;
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
            isChanged = true;
        }

        public void Clear()
        {
            ((IList<T>)items).Clear();
            isChanged = true;
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

        public int IndexOf(T item)
        {
            return ((IList<T>)items).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)items).Insert(index, item);
            isChanged = true;
        }

        public bool Remove(T item)
        {
            isChanged = true;
            return ((IList<T>)items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)items).RemoveAt(index);
            isChanged = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)items).GetEnumerator();
        }

        protected override void OnDeserialized()
        {
            isChanged = false;
        }

        protected override void OnDeserializing()
        {
            items.Clear();
        }

        #endregion Methods
    }
}