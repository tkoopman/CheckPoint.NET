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
    /// Check Point RPC Service
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.IServiceGroupMember" />
    public class ServiceRPC : ObjectBase<ServiceRPC>, IServiceGroupMember
    {
        #region Fields

        private MemberMembershipChangeTracking<ServiceGroup> _groups;
        private bool? _keepConnectionsOpenAfterPolicyInstallation;
        private int? _programNumber;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create new <see cref="ServiceRPC" />.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public ServiceRPC(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRPC" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal ServiceRPC(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<ServiceGroup>(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<ServiceGroup> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        /// <summary>
        /// Gets or sets the keep connections open after policy installation.
        /// </summary>
        /// <value>The keep connections open after policy installation.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "keep-connections-open-after-policy-installation")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? KeepConnectionsOpenAfterPolicyInstallation
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _keepConnectionsOpenAfterPolicyInstallation : null;

            set
            {
                _keepConnectionsOpenAfterPolicyInstallation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Program Number.
        /// </summary>
        /// <value>The Program Number.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "program-number")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int? ProgramNumber
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _programNumber : null;

            set
            {
                _programNumber = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Valid sort orders for TCP services
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