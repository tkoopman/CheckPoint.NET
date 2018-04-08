using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Used to represent an unknown object type when only the UID is returned.
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.IObjectSummary" />
    /// <seealso cref="Koopman.CheckPoint.Common.IGroupMember" />
    /// <seealso cref="Koopman.CheckPoint.Common.IServiceGroupMember" />
    public class GenericMember : IObjectSummary, IGroupMember, IServiceGroupMember, IApplicationGroupMember
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMember" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="uid">The uid.</param>
        internal GenericMember(Session session, string uid)
        {
            UID = uid;
            Session = session;
        }

        #endregion Constructors

        #region Fields

        private IObjectSummary cache = null;
        private bool cached = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the current detail level of retrieved objects. You will not be able to get the
        /// values of some properties if the detail level is too low. You can still set property
        /// values however to override existing values.
        /// </summary>
        /// <value>The current detail level.</value>
        public DetailLevels DetailLevel => DetailLevels.UID;

        /// <summary>
        /// Information about the domain the object belongs to.
        /// </summary>
        /// <value>The domain.</value>
        /// <exception cref="DetailLevelException"></exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        public Domain Domain => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew => false;

        /// <summary>
        /// Object name. Should be unique in the domain.
        /// </summary>
        /// <value>The object's name.</value>
        /// <exception cref="DetailLevelException"></exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        public string Name => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        /// <exception cref="DetailLevelException"></exception>
        public string Type => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        /// <summary>
        /// Object unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        public string UID { get; }

        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the identifier that is used when adding this object to a group.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        public string GetMembershipID() => UID;

        /// <summary>
        /// Reloads the current object. This should return the full IOBjectSummary that matches this UID
        /// </summary>
        /// <param name="OnlyIfPartial">
        /// Only perform reload if <paramref name="detailLevel" /> is not already <see cref="DetailLevels.Full" />
        /// </param>
        /// <param name="detailLevel">The detail level of child objects to retrieve.</param>
        /// <returns>IObjectSummary of reloaded object</returns>
        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = Find.Defaults.DetailLevel)
        {
            if (cache == null || !OnlyIfPartial || cache.DetailLevel < detailLevel)
                cache = Find.Invoke(Session, UID, detailLevel);

            return cache;
        }

        internal IObjectSummary GetFromCache(ObjectConverter objectConverter)
        {
            if (!cached)
            {
                cache = objectConverter.GetFromCache(UID);
                cached = true;
            }

            return cache;
        }

        #endregion Methods
    }
}