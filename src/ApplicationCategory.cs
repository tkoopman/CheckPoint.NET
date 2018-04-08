using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Application Category Class.
    /// </summary>
    /// <example>
    /// Add new category using <see cref="ApplicationCategory.ApplicationCategory(Session, bool)" />
    /// <code>
    /// var cat = new ApplicationCategory(Session) {
    ///     Name = "MyCategory"
    /// };
    /// cat.AcceptChanges();
    /// </code>
    /// Find group using <see cref="Session.FindApplicationCategory(string, DetailLevels)" />
    /// <code>
    /// var cat = Session.FindApplicationCategory("MyCategory");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    /// <seealso cref="Koopman.CheckPoint.Common.IApplicationGroupMember" />
    public class ApplicationCategory : ObjectBase<ApplicationCategory>, IApplicationGroupMember
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="ApplicationCategory" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var cat = new ApplicationCategory(Session) {
        ///     Name = "MyCategory"
        /// };
        /// cat.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public ApplicationCategory(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCategory" /> class ready to be
        /// populated with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal ApplicationCategory(Session session, DetailLevels detailLevel) : base(session, detailLevel, "application-site-category")
        {
            _groups = new MemberMembershipChangeTracking<ApplicationGroup>(this);
        }

        #endregion Constructors

        #region Fields

        private string _description;
        private MemberMembershipChangeTracking<ApplicationGroup> _groups;
        private bool? _userDefined;

        #endregion Fields

        #region Properties

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