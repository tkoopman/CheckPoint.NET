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
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Application Group Class.
    /// </summary>
    /// <example>
    /// Add new group using <see cref="ApplicationGroup.ApplicationGroup(Session, bool)" />
    /// <code>
    /// var group = new ApplicationGroup(Session) {
    /// Name = "MyAppGroup"
    /// };
    /// group.Members.Add("App1");
    /// group.Members.Add("App2");
    /// group.Groups.Add("AnotherAppGroup");
    /// group.AcceptChanges();
    /// </code>
    /// Find group using <see cref="Session.FindApplicationGroup(string, DetailLevels, CancellationToken)" />
    /// <code>
    /// var group = Session.FindApplicationGroup("MyAppGroup");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.IApplicationGroupMember" />
    public class ApplicationGroup : ObjectBase<ApplicationGroup>, IApplicationGroupMember
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="ApplicationGroup" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var group = new ApplicationGroup(Session) {
        /// Name = "MyAppGroup"
        /// };
        /// group.Members.Add("App1");
        /// group.Members.Add("App2");
        /// group.Groups.Add("AnotherAppGroup");
        /// group.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public ApplicationGroup(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationGroup" /> class ready to be
        /// populated with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal ApplicationGroup(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<ApplicationGroup>(this);
            _members = new MemberMembershipChangeTracking<IApplicationGroupMember>(this);
        }

        #endregion Constructors

        #region Fields

        private MemberMembershipChangeTracking<ApplicationGroup> _groups;
        private MemberMembershipChangeTracking<IApplicationGroupMember> _members;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<ApplicationGroup> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Members of this group.
        /// </summary>
        [JsonProperty(PropertyName = "members")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<IApplicationGroupMember> Members
        {
            get => _members;
            internal set => _members = value;
        }

        /// <inheritdoc />
        public override ObjectType ObjectType => ObjectType.ApplicationSiteGroup;

        /// <inheritdoc />
        public override string Type => "application-site-group";

        #endregion Properties

        #region Methods

        internal override void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            base.UpdateGenericMembers(objectConverter);
            Groups.UpdateGenericMembers(objectConverter);
            Members.UpdateGenericMembers(objectConverter);
        }

        #endregion Methods
    }
}