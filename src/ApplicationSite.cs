﻿// MIT License
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
    /// Application Site Class.
    /// </summary>
    /// <example>
    /// Add new site using <see cref="ApplicationSite.ApplicationSite(Session, bool)" />
    /// <code>
    /// var site = new ApplicationSite(Session) {
    /// Name = "MySite"
    /// };
    /// site.UrlList.Add("www.mysite.com");
    /// site.AcceptChanges();
    /// </code>
    /// Find site using <see cref="Session.FindApplicationSite(string, DetailLevels, CancellationToken)" />
    /// <code>
    /// var site = Session.FindApplicationSite("MySite");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.IApplicationGroupMember" />
    public class ApplicationSite : ObjectBase<ApplicationSite>, IApplicationGroupMember
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="ApplicationSite" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var site = new ApplicationSite(Session) {
        /// Name = "MySite"
        /// };
        /// site.UrlList.Add("www.mysite.com");
        /// site.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public ApplicationSite(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSite" /> class ready to be
        /// populated with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal ApplicationSite(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new MemberMembershipChangeTracking<ApplicationGroup>(this);
            _urlList = new MembershipChangeTracking<string>(this);
            _applicationSignature = new MembershipChangeTracking<string>(this);
            _additionalCategories = new MembershipChangeTracking<string>(this);
        }

        #endregion Constructors

        #region Fields

        private MembershipChangeTracking<string> _additionalCategories;
        private int? _applicationID;
        private MembershipChangeTracking<string> _applicationSignature;
        private string _description;
        private MemberMembershipChangeTracking<ApplicationGroup> _groups;
        private string _primaryCategory;
        private MembershipChangeTracking<string> _urlList;
        private bool? _urlsDefinedAsRegularExpression;
        private bool? _userDefined;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Used to configure or edit the additional categories of a custom application / site used
        /// in the Application and URL Filtering or Threat Prevention.
        /// </summary>
        /// <value>The additional categories assigned.</value>
        [JsonProperty(PropertyName = "additional-categories")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> AdditionalCategories
        {
            get => _additionalCategories;
            internal set => _additionalCategories = value;
        }

        /// <summary>
        /// Used to configure or edit the additional categories of a custom application / site used
        /// in the Application and URL Filtering or Threat Prevention.
        /// </summary>
        /// <value>The additional categories assigned.</value>
        [JsonProperty(PropertyName = "application-id")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int? ApplicationID
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _applicationID : null;
            internal set => _applicationID = value;
        }

        /// <summary>
        /// Application signature generated by Signature Tool.
        /// </summary>
        /// <value>The application signature.</value>
        [JsonProperty(PropertyName = "application-signature")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> ApplicationSignature
        {
            get => _applicationSignature;
            internal set => _applicationSignature = value;
        }

        /// <summary>
        /// States whether the URL is defined as a Regular Expression or not.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "urls-defined-as-regular-expression")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? AreUrlsDefinedAsRegularExpression
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _urlsDefinedAsRegularExpression : null;
            set
            {
                _urlsDefinedAsRegularExpression = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Description.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "description")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Description
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _description : null;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

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
        /// Is User Defined.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "user-defined")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? IsUserDefined
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _userDefined : null;
            internal set => _userDefined = value;
        }

        /// <inheritdoc />
        public override ObjectType ObjectType => ObjectType.ApplicationSite;

        /// <summary>
        /// Each application is assigned to one primary category based on its most defining aspect.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "primary-category")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string PrimaryCategory
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _primaryCategory : null;
            set
            {
                _primaryCategory = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// URLs that determine this particular application.
        /// </summary>
        /// <value>The URLs that determine this particular application.</value>
        [JsonProperty(PropertyName = "risk")]
        public string Risk { get; internal set; }

        /// <inheritdoc />
        public override string Type => "application-site";

        /// <summary>
        /// URLs that determine this particular application.
        /// </summary>
        /// <value>The URLs that determine this particular application.</value>
        [JsonProperty(PropertyName = "url-list")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MembershipChangeTracking<string> UrlList
        {
            get => _urlList;
            internal set => _urlList = value;
        }

        #endregion Properties

        #region Methods

        internal override void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            base.UpdateGenericMembers(objectConverter);
            Groups.UpdateGenericMembers(objectConverter);
        }

        #endregion Methods
    }
}