using Koopman.CheckPoint.Common;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Track type
    /// </summary>
    public class TrackType : IObjectSummary
    {
        #region Fields

        public static readonly TrackType DetailedLog = new TrackType(Domain.DataDomain, "Detailed Log", "d395a3bb-c96c-4781-8017-b9cd0418e982");
        public static readonly TrackType ExtendedLog = new TrackType(Domain.DataDomain, "Extended Log", "78566494-7e96-4513-ada9-ded83f4ee9ea");
        public static readonly TrackType Log = new TrackType(Domain.DataDomain, "None", "598ead32-aa42-4615-90ed-f51a5928d41d");
        public static readonly TrackType None = new TrackType(Domain.DataDomain, "None", "29e53e3d-23bf-48fe-b6b1-d59bd88036f9");
        internal static readonly TrackType[] Types = new TrackType[] { None, Log, DetailedLog, ExtendedLog };

        #endregion Fields

        #region Constructors

        private TrackType(Domain domain, string name, string uID, string type = "RulebaseAction")
        {
            Domain = domain;
            Name = name;
            UID = uID;
            Type = type;
        }

        #endregion Constructors

        #region Properties

        public DetailLevels DetailLevel => DetailLevels.Full;

        public Domain Domain { get; }

        public bool IsNew => false;

        public string Name { get; }

        public string Type { get; }

        public string UID { get; }

        #endregion Properties

        #region Methods

        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard) => this;

        public override string ToString() => GetIdentifier();

        #endregion Methods
    }
}