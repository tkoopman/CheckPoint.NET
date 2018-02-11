using Koopman.CheckPoint.Common;
using Newtonsoft.Json;

namespace Koopman.CheckPoint
{
    public static class SecurityZoneSettings
    {
        #region Classes

        public class AutoCalculated : ISecurityZoneSettings
        {
            #region Properties

            [JsonProperty(PropertyName = "auto-calculated")]
            private bool Value { get => true; }

            #endregion Properties
        }

        public class SpecificZone : ISecurityZoneSettings
        {
            #region Properties

            [JsonProperty(PropertyName = "specific-zone")]
            public string Name { get; set; }

            #endregion Properties

            #region Methods

            public override string ToString()
            {
                return Name;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}