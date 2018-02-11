using Newtonsoft.Json;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    public class AntiSpoofing
    {
        #region Properties

        [JsonProperty(PropertyName = "action")]
        public AntiSpoofingAction Action { get; set; }

        #endregion Properties
    }
}