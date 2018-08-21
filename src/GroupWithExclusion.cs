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
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Group with Exclusion Class
    /// </summary>
    /// <example>
    /// Add new group with exclusion using <see cref="GroupWithExclusion.GroupWithExclusion(Session, bool)" />
    /// <code>
    /// var gwe = new GroupWithExclusion(Session) {
    /// Name = "MyGroupWithExclusion",
    /// Include = ObjectSummary.Any,
    /// Except = Session.FindGroup("ExcludeGroup")
    /// };
    /// gwe.AcceptChanges();
    /// </code>
    /// Find group with exclusion using <see cref="Session.FindGroupWithExclusion(string, DetailLevels, CancellationToken)" />
    /// <code>
    /// var gwe = Session.FindGroupWithExclusion("MyGroupWithExclusion");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    public class GroupWithExclusion : ObjectBase<GroupWithExclusion>
    {
        #region Fields

        private IObjectSummary _except;
        private IObjectSummary _include;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Add new group with exclusion using <see cref="GroupWithExclusion" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var gwe = new GroupWithExclusion(Session) {
        /// Name = "MyGroupWithExclusion",
        /// Include = ObjectSummary.Any,
        /// Except = Session.FindGroup("ExcludeGroup")
        /// };
        /// gwe.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public GroupWithExclusion(Session session, bool setIfExists = false) : base(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupWithExclusion" /> class ready to be
        /// populated with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal GroupWithExclusion(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get or sets the object this group excludes.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "except", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public IObjectSummary Except
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _except : null;
            set
            {
                _except = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the object this group includes.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "include", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public IObjectSummary Include
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _include : null;
            set
            {
                _include = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public override ObjectType ObjectType => ObjectType.GroupWithExclusion;

        /// <inheritdoc />
        public override string Type => "group-with-exclusion";

        /// <inheritdoc />
        protected override IContractResolver AddContractResolver => GroupWithExclusionContractResolver.AddInstance;

        /// <inheritdoc />
        protected override IContractResolver SetContractResolver => GroupWithExclusionContractResolver.SetInstance;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the object this group excepts.
        /// </summary>
        /// <param name="value">The name or UID of object.</param>
        public void SetExcept(string value) => Except = new AddAsString(value);

        /// <summary>
        /// Sets the object this group includes.
        /// </summary>
        /// <param name="value">The name or UID of object.</param>
        public void SetInclude(string value) => Include = new AddAsString(value);

        internal override void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            base.UpdateGenericMembers(objectConverter);
            if (DetailLevel >= DetailLevels.Standard)
            {
                if (_include is GenericMember i)
                {
                    var summary = i.GetFromCache(objectConverter);
                    if (summary != null)
                        _include = summary;
                }
                if (_except is GenericMember e)
                {
                    var summary = e.GetFromCache(objectConverter);
                    if (summary != null)
                        _except = summary;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDeserializing()
        {
            base.OnDeserializing();
            _include = null;
            _except = null;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Valid sort orders for group with exclusions
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

        private class AddAsString : IObjectSummary
        {
            #region Constructors

            public AddAsString(string value)
            {
                if (value.IsUID())
                    UID = value;
                else
                    Name = value;
            }

            #endregion Constructors

            #region Properties

            public DetailLevels DetailLevel => throw new System.NotImplementedException();

            DetailLevels IObjectSummary.DetailLevel => throw new System.NotImplementedException();
            public Domain Domain => throw new System.NotImplementedException();

            Domain IObjectSummary.Domain => throw new System.NotImplementedException();
            public bool IsNew => throw new System.NotImplementedException();

            bool IObjectSummary.IsNew => throw new System.NotImplementedException();
            public string Name { get; }

            string IObjectSummary.Name => throw new System.NotImplementedException();
            public ObjectType ObjectType => throw new System.NotImplementedException();
            public string Type => throw new System.NotImplementedException();

            string IObjectSummary.Type => throw new System.NotImplementedException();
            public string UID { get; }
            string IObjectSummary.UID => throw new System.NotImplementedException();

            #endregion Properties

            #region Methods

            public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

            string IObjectSummary.GetIdentifier() => throw new System.NotImplementedException();

            Task<IObjectSummary> IObjectSummary.Reload(bool OnlyIfPartial, DetailLevels detailLevel, CancellationToken cancellationToken) => throw new System.NotImplementedException();

            public override string ToString() => GetIdentifier();

            #endregion Methods
        }

        #endregion Classes
    }
}