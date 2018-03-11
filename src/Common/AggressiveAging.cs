using Newtonsoft.Json;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Sets short (aggressive) timeouts for idle connections.
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class AggressiveAging : ChangeTracking
    {
        #region Fields

        private bool _enable;
        private int _timeout;
        private bool _useDefaultTimeout;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the default timeout.
        /// </summary>
        /// <value>The default timeout.</value>
        [JsonProperty(PropertyName = "default-timeout")]
        public int DefaultTimeout { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether aggressive aging is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "enable")]
        public bool Enable
        {
            get => _enable;

            set
            {
                _enable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the aggressive aging timeout in seconds.
        /// </summary>
        /// <value>The timeout.</value>
        [JsonProperty(PropertyName = "timeout")]
        public int Timeout
        {
            get => _timeout;

            set
            {
                _timeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use default timeout.
        /// </summary>
        /// <value><c>true</c> if using default timeout; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "use-default-timeout")]
        public bool UseDefaultTimeout
        {
            get => _useDefaultTimeout;

            set
            {
                _useDefaultTimeout = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}