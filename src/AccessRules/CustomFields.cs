using Koopman.CheckPoint.Common;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    public class CustomFields : ChangeTracking
    {
        #region Fields

        private string _field1;
        private string _field2;
        private string _field3;

        #endregion Fields

        #region Properties

        [JsonProperty(PropertyName = "field-1")]
        public string Field1
        {
            get => _field1;
            set
            {
                _field1 = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "field-2")]
        public string Field2
        {
            get => _field2;
            set
            {
                _field2 = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "field-3")]
        public string Field3
        {
            get => _field3;
            set
            {
                _field3 = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}