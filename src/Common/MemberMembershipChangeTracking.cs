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

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Track adds and removes of membership objects when members are of type ObjectSummary. Used
    /// when Check Point API supports these methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Koopman.CheckPoint.Common.MembershipChangeTracking{T}" />
    public class MemberMembershipChangeTracking<T> : MembershipChangeTracking<T> where T : IObjectSummary
    {
        #region Constructors

        internal MemberMembershipChangeTracking(IObjectSummary parent) : base(parent)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        public override void Add(T item)
        {
            if (IsDeserializing)
            {
                Members.Add(item);
            }
            else
            {
                if (item == null) { return; }
                Add(item.GetMembershipID());
            }
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
        public override bool Remove(T item)
        {
            if (item == null) { return false; }
            return Remove(item.GetMembershipID());
        }

        internal void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            for (int x = 0; x < Members.Count; x++)
                if (Members[x] is GenericMember)
                {
                    GenericMember m = (GenericMember)(IObjectSummary)Members[x];
                    IObjectSummary summary = m.GetFromCache(objectConverter);
                    if (summary != null)
                        Members[x] = (T)summary;
                }
        }

        #endregion Methods
    }
}