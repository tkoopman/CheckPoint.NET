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

using Koopman.CheckPoint.Common;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Network Object Group Class.
    /// </summary>
    /// <example>
    /// Add new group using <see cref="Group.Group(Session, bool)" />
    /// <code>
    /// var group = new Group(Session) {
    ///     Name = "MyGroup"
    /// };
    /// group.Members.Add("MyHost1");
    /// group.Members.Add("MyHost2");
    /// group.Groups.Add("AnotherGroup");
    /// group.AcceptChanges();
    /// </code>
    /// Find group using <see cref="Session.FindGroup(string, DetailLevels)" />
    /// <code>
    /// var group = Session.FindGroup("MyGroup");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase" />
    /// <seealso cref="Koopman.CheckPoint.Common.IGroupMember" />
    public class Group : ObjectBase, IGroupMember
    {
        #region Fields

        private MemberMembershipChangeTracking<Group> _groups;
        private MemberMembershipChangeTracking<IGroupMember> _members;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a new <see cref="Group" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var group = new Group(Session) {
        ///     Name = "MyGroup"
        /// };
        /// group.Members.Add("MyHost1");
        /// group.Members.Add("MyHost2");
        /// group.Groups.Add("AnotherGroup");
        /// group.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public Group(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group" /> class ready to be populated with
        /// current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal Group(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<Group>(this);
            _members = new MemberMembershipChangeTracking<IGroupMember>(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<Group> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Members of this group.
        /// </summary>
        [JsonProperty(PropertyName = "members")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<IGroupMember> Members
        {
            get => _members;
            internal set => _members = value;
        }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Valid sort orders for Groups
        /// </summary>
        public static class Order
        {
            #region Fields

            /// <summary>
            /// Sort by name in ascending order
            /// </summary>
            public readonly static IOrder NameAsc = new OrderAscending("name");

            /// <summary>
            /// Sort by name in descending order
            /// </summary>
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        #endregion Classes
    }
}