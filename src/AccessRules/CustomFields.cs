using Koopman.CheckPoint.Common;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Custom fields assigned to rule
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class CustomFields : ChangeTracking
    {
        #region Fields

        private string _field1;
        private string _field2;
        private string _field3;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the field1.
        /// </summary>
        /// <value>The field1.</value>
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

        /// <summary>
        /// Gets or sets the field2.
        /// </summary>
        /// <value>The field2.</value>
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

        /// <summary>
        /// Gets or sets the field3.
        /// </summary>
        /// <value>The field3.</value>
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