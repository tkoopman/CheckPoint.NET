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
using Newtonsoft.Json.Serialization;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Group with Exclusion Class
    /// </summary>
    /// <example>
    /// Add new group with exclusion using <see cref="GroupWithExclusion.GroupWithExclusion(Session)" />
    /// <code>
    /// var gwe = new GroupWithExclusion(Session) {
    ///     Name = "MyGroupWithExclusion",
    ///     Include = ObjectSummary.Any,
    ///     Except = Session.FindGroup("ExcludeGroup")
    /// };
    /// gwe.AcceptChanges();
    /// </code>
    /// Find group with exclusion using <see cref="Session.FindGroupWithExclusion(string, DetailLevels)" />
    /// <code>
    /// var gwe = Session.FindGroupWithExclusion("MyGroupWithExclusion");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase" />
    public class GroupWithExclusion : ObjectBase
    {
        #region Fields

        private ObjectSummary _except;
        private ObjectSummary _include;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Add new group with exclusion using <see cref="GroupWithExclusion" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var gwe = new GroupWithExclusion(Session) {
        ///     Name = "MyGroupWithExclusion",
        ///     Include = ObjectSummary.Any,
        ///     Except = Session.FindGroup("ExcludeGroup")
        /// };
        /// gwe.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        public GroupWithExclusion(Session session) : base(session, DetailLevels.Full)
        {
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
        [JsonProperty(PropertyName = "except")]
        public ObjectSummary Except
        {
            get
            {
                return (TestDetailLevel(DetailLevels.Full)) ? _except : null;
            }
            set
            {
                _except = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the object this group includes.
        /// </summary>
        [JsonProperty(PropertyName = "include")]
        public ObjectSummary Include
        {
            get
            {
                return (TestDetailLevel(DetailLevels.Full)) ? _include : null;
            }
            set
            {
                _include = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        protected override IContractResolver AddContractResolver => GroupWithExclusionContractResolver.AddInstance;

        /// <inheritdoc />
        protected override IContractResolver SetContractResolver => GroupWithExclusionContractResolver.SetInstance;

        #endregion Properties

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

        #endregion Classes
    }
}