using Koopman.CheckPoint.Common;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Access rule track settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class Track : ChangeTracking
    {
        #region Fields

        private bool _accounting;
        private AlertType _alert;
        private bool _perConnection;
        private bool _perSession;
        private TrackType _type;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets if accounting is enabled.
        /// </summary>
        /// <value>The accounting status.</value>
        [JsonProperty(PropertyName = "accounting")]
        public bool Accounting
        {
            get => _accounting;
            set
            {
                _accounting = value;
                OnPropertyChanged();
            }
        }

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

        /// <summary>
        /// Gets or sets if logs are genereated per connection.
        /// </summary>
        /// <value>The per connection logging status.</value>
        [JsonProperty(PropertyName = "per-connection")]
        public bool PerConnection
        {
            get => _perConnection;
            set
            {
                _perConnection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if logs are genereated per session.
        /// </summary>
        /// <value>The per session logging status.</value>
        [JsonProperty(PropertyName = "per-session")]
        public bool PerSession
        {
            get => _perSession;
            set
            {
                _perSession = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the type of tracking to use.
        /// </summary>
        /// <value>The tracking type.</value>
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

        /// <inheritdoc />
        public override string ToString() => _type?.ToString();

        #endregion Methods
    }
}