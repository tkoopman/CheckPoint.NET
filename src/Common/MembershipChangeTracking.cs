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

using Koopman.CheckPoint.Exceptions;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Action to be done when Accepting Changes.
    /// </summary>
    public enum ChangeAction
    {
        /// <summary>
        /// No changes required
        /// </summary>
        None,

        /// <summary>
        /// Replace any existing values with new list of members
        /// </summary>
        Set,

        /// <summary>
        /// Add to the existing members
        /// </summary>
        Add,

        /// <summary>
        /// Remove from the existing members
        /// </summary>
        Remove
    }

    /// <summary>
    /// Track adds and removes of membership objects. Used when Check Point API supports these methods.
    /// </summary>
    /// <typeparam name="T">Type of Membership object</typeparam>
    /// <seealso cref="Koopman.CheckPoint.Common.SimpleChangeTracking" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    public class MembershipChangeTracking<T> : SimpleChangeTracking, IEnumerable<T>, IList<T>
    {
        #region Fields

        protected internal List<T> Members = new List<T>();

        #endregion Fields

        #region Constructors

        internal MembershipChangeTracking(ObjectSummary parent)
        {
            Parent = parent;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public int Count => ((IList<T>)Members).Count;

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        public override bool IsChanged
        {
            get { return Action != ChangeAction.None; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        public bool IsReadOnly => ((IList<T>)Members).IsReadOnly;

        /// <summary>
        /// Gets the action to be used.
        /// </summary>
        protected internal ChangeAction Action { get; private set; } = ChangeAction.None;

        /// <summary>
        /// Gets the changed members.
        /// </summary>
        protected internal List<string> ChangedMembers { get; private set; } = null;

        /// <summary>
        /// Gets a value indicating whether there were existing members when the object was deserilized.
        /// </summary>
        protected internal bool HadMembers { get; private set; }

        /// <summary>
        /// Gets the parent object that is tracking this membership.
        /// </summary>
        [JsonIgnore]
        protected internal ObjectSummary Parent { get; internal set; }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets the <see cref="T" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">Cannot use index to set members</exception>
        public T this[int index] { get => ((IList<T>)Members)[index]; set { throw new System.NotImplementedException($"Use Add, Remove and Clear methods only to modify membership."); } }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="DetailLevelException"></exception>
        public void Add(string item)
        {
            if (Action == ChangeAction.None || Action == ChangeAction.Add || Action == ChangeAction.Set)
            {
                if (Action == ChangeAction.None)
                {
                    ChangedMembers = new List<string>();
                    Action = (Parent.IsNew) ? ChangeAction.Set : ChangeAction.Add;
                }

                ChangedMembers.Add(item);
            }
            else if (!IsNew)
            {
                Action = ChangeAction.Set;
                List<string> ToRemove = new List<string>(ChangedMembers);
                ChangedMembers.Clear();

                foreach (var m in Members)
                {
                    if (!ToRemove.Contains(m.ToString()))
                    {
                        ChangedMembers.Add(m.ToString());
                    }
                }

                ChangedMembers.Add(item);
            }
            else
            {
                throw new DetailLevelException(DetailLevels.Standard, DetailLevels.Full);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        public virtual void Add(T item)
        {
            if (IsDeserializing)
            {
                Members.Add(item);
            }
            else
            {
                Add(item.ToString());
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Clear()
        {
            Action = ChangeAction.Set;
            ChangedMembers = new List<string>();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see>
        /// contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> is found in the <see
        /// cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return ((IList<T>)Members).Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see>
        /// to an <see cref="T:System.Array"></see>, starting at a particular <see
        /// cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"></see> that is the destination of the
        /// elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The
        /// <see cref="T:System.Array"></see> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)Members).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Members).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)Members).GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>
        /// The index of <paramref name="item">item</paramref> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return ((IList<T>)Members).IndexOf(item);
        }

        /// <summary>
        /// Use Add, Remove and Clear methods only to modify membership.
        /// </summary>
        /// <exception cref="System.NotImplementedException">
        /// Use Add, Remove and Clear methods only to modify membership..
        /// </exception>
        public void Insert(int index, T item)
        {
            throw new System.NotImplementedException($"Use Add, Remove and Clear methods only to modify membership.");
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="DetailLevelException"></exception>
        public bool Remove(string item)
        {
            if ((Action == ChangeAction.None || Action == ChangeAction.Remove) && !Parent.IsNew)
            {
                Action = ChangeAction.Remove;
                if (ChangedMembers == null)
                {
                    ChangedMembers = new List<string>();
                }

                ChangedMembers.Add(item);

                return true;
            }
            else if (Action == ChangeAction.Set)
            {
                return ChangedMembers.Remove(item);
            }
            else if (!IsNew)
            {
                Action = ChangeAction.Set;
                string[] ToAdd = ChangedMembers.ToArray();
                ChangedMembers.Clear();
                foreach (var m in Members)
                {
                    ChangedMembers.Add(m.ToString());
                }
                ChangedMembers.AddRange(ToAdd);

                return ChangedMembers.Remove(item);
            }
            else
            {
                throw new DetailLevelException(DetailLevels.Standard, DetailLevels.Full);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> was successfully removed from the <see
        /// cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method
        /// also returns false if <paramref name="item">item</paramref> is not found in the original
        /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        public virtual bool Remove(T item)
        {
            return Remove(item.ToString());
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException($"Use Add, Remove and Clear methods only to modify membership.");
        }

        /// <summary>
        /// Called when deserialized.
        /// </summary>
        protected override void OnDeserialized()
        {
            HadMembers = Count > 0;
            ChangedMembers = null;
            Action = ChangeAction.None;
        }

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        protected override void OnDeserializing()
        {
            Members.Clear();
        }

        #endregion Methods
    }
}