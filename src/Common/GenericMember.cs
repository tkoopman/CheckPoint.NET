using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;

namespace Koopman.CheckPoint.Common
{
    public class GenericMember : IObjectSummary, IGroupMember, IServiceGroupMember
    {
        #region Constructors

        internal GenericMember(Session session, string uid)
        {
            UID = uid;
            Session = session;
        }

        #endregion Constructors

        #region Properties

        public DetailLevels DetailLevel => DetailLevels.UID;

        public Domain Domain => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        public bool IsNew => false;

        public string Name => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        public string Type => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        public string UID { get; }

        private Session Session { get; }

        #endregion Properties

        #region Methods

        public string GetMembershipID() => UID;

        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard)
        {
            return Find.Invoke(Session, UID, detailLevel);
        }

        #endregion Methods
    }
}