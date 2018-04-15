using Koopman.CheckPoint.Common;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Track type
    /// </summary>
    public class TrackType : IObjectSummary
    {
        #region Fields

        /// <summary>
        /// Track with detailed logging
        /// </summary>
        public static readonly TrackType DetailedLog = new TrackType(Domain.DataDomain, "Detailed Log", "d395a3bb-c96c-4781-8017-b9cd0418e982");

        /// <summary>
        /// Track with extended logging
        /// </summary>
        public static readonly TrackType ExtendedLog = new TrackType(Domain.DataDomain, "Extended Log", "78566494-7e96-4513-ada9-ded83f4ee9ea");

        /// <summary>
        /// Track with standard logging
        /// </summary>
        public static readonly TrackType Log = new TrackType(Domain.DataDomain, "Log", "598ead32-aa42-4615-90ed-f51a5928d41d");

        /// <summary>
        /// No tracking
        /// </summary>
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

        /// <inheritdoc />
        public DetailLevels DetailLevel => DetailLevels.Full;

        /// <inheritdoc />
        public Domain Domain { get; }

        /// <inheritdoc />
        public bool IsNew => false;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public string UID { get; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        /// <inheritdoc />
        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard) => this;

        /// <inheritdoc />
        public override string ToString() => GetIdentifier();

        #endregion Methods
    }
}