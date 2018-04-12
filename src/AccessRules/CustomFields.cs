using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    public class CustomFields
    {
        #region Constructors

        public CustomFields(string field1, string field2, string field3)
        {
            Field1 = field1;
            Field2 = field2;
            Field3 = field3;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "field-1")]
        public string Field1 { get; private set; }

        [JsonProperty(PropertyName = "field-2")]
        public string Field2 { get; private set; }

        [JsonProperty(PropertyName = "field-3")]
        public string Field3 { get; private set; }

        #endregion Properties
    }
}