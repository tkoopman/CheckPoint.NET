using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    public class ActionSettings
    {
        #region Properties

        [JsonProperty(PropertyName = "enable-identity-captive-portal")]
        public bool EnableIdentityCaptivePortal { get; private set; }

        [JsonProperty(PropertyName = "limit")]
        public Limit Limit { get; private set; }

        #endregion Properties
    }
}