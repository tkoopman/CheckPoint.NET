using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    public class Limit : GenericObjectSummary
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMember" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="type">The type.</param>
        internal Limit(Session session, DetailLevels detailLevel) : base(session, detailLevel, "CpmiAppfwLimit")
        {
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "color")]
        public Colors Color { get; private set; }

        [JsonProperty(PropertyName = "maxDownloadLimit")]
        public string MaxDownloadLimit { get; private set; }

        [JsonProperty(PropertyName = "maxUploadLimit")]
        public string MaxUploadLimit { get; private set; }

        #endregion Properties
    }
}