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

using Koopman.CheckPoint.Exceptions;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Koopman.CheckPoint.Common
{
    public enum ChangeAction
    {
        None,
        Set,
        Add,
        Remove
    }

    public class MembershipChangeTracking<T> : IEnumerable<T>, IList<T>, IChangeTracking where T : ObjectSummary
    {
        #region Fields

        protected internal List<T> Members = new List<T>();

        #endregion Fields

        #region Constructors

        [JsonConstructor]
        internal MembershipChangeTracking()
        {
        }

        #endregion Constructors

        #region Properties

        public int Count => ((IList<T>)Members).Count;

        public bool IsChanged
        {
            get { return Action != ChangeAction.None; }
        }

        public bool IsReadOnly => ((IList<T>)Members).IsReadOnly;
        protected internal ChangeAction Action { get; private set; } = ChangeAction.None;
        protected internal List<string> ChangedMembers { get; private set; } = null;
        protected internal bool HadMembers { get; private set; }
        protected internal bool IsDeserializing { get; private set; } = false;
        protected bool HasDeserialized { get; private set; } = false;

        #endregion Properties

        #region Indexers

        public T this[int index] { get => ((IList<T>)Members)[index]; set { throw new System.NotImplementedException($"Use Add, Remove and Clear methods only to modify membership."); } }

        #endregion Indexers

        #region Methods

        public void AcceptChanges()
        {
            throw new System.NotImplementedException("Use AcceptChanges() on parent object.");
        }

        public void Add(string item)
        {
            if (Action == ChangeAction.None || Action == ChangeAction.Add || Action == ChangeAction.Set)
            {
                Action = (Action == ChangeAction.None) ? ChangeAction.Add : Action;
                if (ChangedMembers == null)
                {
                    ChangedMembers = new List<string>();
                }

                ChangedMembers.Add(item);
            }
            else if (HasDeserialized)
            {
                Action = ChangeAction.Set;
                List<string> ToRemove = new List<string>(ChangedMembers);
                ChangedMembers.Clear();

                foreach (var m in Members)
                {
                    if (!ToRemove.Contains(m.Name) && !ToRemove.Contains(m.UID))
                    {
                        ChangedMembers.Add(m.UID);
                    }
                }

                ChangedMembers.Add(item);
            }
            else
            {
                throw new DetailLevelException(DetailLevels.Standard, DetailLevels.Full);
            }
        }

        public void Add(T item)
        {
            if (IsDeserializing)
            {
                Members.Add(item);
            }
            else
            {
                Add(item.UID);
            }
        }

        public void Clear()
        {
            Action = ChangeAction.Set;
            ChangedMembers = new List<string>();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)Members).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)Members).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Members).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)Members).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)Members).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new System.NotImplementedException($"Use Add, Remove and Clear methods only to modify membership.");
        }

        public bool Remove(string item)
        {
            if (Action == ChangeAction.None || Action == ChangeAction.Remove)
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
            else if (HasDeserialized)
            {
                Action = ChangeAction.Set;
                string[] ToAdd = ChangedMembers.ToArray();
                ChangedMembers.Clear();
                foreach (var m in Members)
                {
                    ChangedMembers.Add(m.UID);
                }
                ChangedMembers.AddRange(ToAdd);

                return ChangedMembers.Remove(item);
            }
            else
            {
                throw new DetailLevelException(DetailLevels.Standard, DetailLevels.Full);
            }
        }

        public bool Remove(T item)
        {
            if (!Remove(item.UID))
            {
                return Remove(item.Name);
            }
            else { return true; }
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException($"Use Add, Remove and Clear methods only to modify membership.");
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            HadMembers = Count > 0;
            HasDeserialized = true;
            ChangedMembers = null;
            Action = ChangeAction.None;
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
            Members.Clear();
        }

        #endregion Methods
    }
}