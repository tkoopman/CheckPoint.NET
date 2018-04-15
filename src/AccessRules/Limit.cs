using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Access Rule Limit object
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Internal.GenericObjectSummary" />
    public class Limit : GenericObjectSummary
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Limit" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="detailLevel">The detail level.</param>
        internal Limit(Session session, DetailLevels detailLevel) : base(session, detailLevel, "CpmiAppfwLimit")
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        [JsonProperty(PropertyName = "color")]
        public Colors Color { get; private set; }

        /// <summary>
        /// Gets the maximum download limit.
        /// </summary>
        /// <value>The maximum download limit.</value>
        [JsonProperty(PropertyName = "maxDownloadLimit")]
        public string MaxDownloadLimit { get; private set; }

        /// <summary>
        /// Gets the maximum upload limit.
        /// </summary>
        /// <value>The maximum upload limit.</value>
        [JsonProperty(PropertyName = "maxUploadLimit")]
        public string MaxUploadLimit { get; private set; }

        #endregion Properties
    }
}