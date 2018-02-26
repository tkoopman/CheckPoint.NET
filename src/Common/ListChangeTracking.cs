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

using System.ComponentModel;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Used to keep track of changes to a List of IChangeTracking objects
    /// </summary>
    /// <typeparam name="T">Object of type IChangeTracking stored in List</typeparam>
    /// <seealso cref="Koopman.CheckPoint.Common.SimpleListChangeTracking{T}" />
    public class ListChangeTracking<T> : SimpleListChangeTracking<T> where T : IChangeTracking
    {
        #region Properties

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <value>
        /// <c>true</c> if any properties have been changed or if any contained
        /// MembershipChangeTracking properties have had membership modifications.
        /// </value>
        public override bool IsChanged
        {
            get
            {
                if (base.IsChanged)
                {
                    return true;
                }
                else
                {
                    foreach (var i in this)
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

        #endregion Properties
    }
}