using Koopman.CheckPoint.Common;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    public class Track : ChangeTracking
    {
        #region Fields

        private AlertType _alert;
        private TrackType _type;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the alert type for track.
        /// </summary>
        /// <value>The alert type.</value>
        [JsonProperty(PropertyName = "alert")]
        public AlertType Alert
        {
            get => _alert;
            set
            {
                _alert = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "type")]
        public TrackType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        public override string ToString() => _type?.ToString();

        #endregion Methods
    }
}