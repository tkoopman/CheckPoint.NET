using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Extra action settings for rule
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class ActionSettings : ChangeTracking
    {
        #region Fields

        private bool _enableIdentityCaptivePortal;
        private Limit _limit;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether identity captive portal is enabled for this rule.
        /// </summary>
        /// <value><c>true</c> if enable identity captive portal; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "enable-identity-captive-portal")]
        public bool EnableIdentityCaptivePortal
        {
            get => _enableIdentityCaptivePortal;
            set
            {
                _enableIdentityCaptivePortal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the limit object assigned to this rule.
        /// </summary>
        /// <value>The limit.</value>
        [JsonProperty(PropertyName = "limit")]
        public Limit Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the limit by name or UID.
        /// </summary>
        /// <param name="identifier">The name or UID of Limit object.</param>
        public void SetLimit(string identifier)
        {
            var l = new Limit(null, DetailLevels.Full);
            if (identifier.IsUID())
                l.UID = identifier;
            else
                l.Name = identifier;

            Limit = l;
        }

        #endregion Methods
    }
}