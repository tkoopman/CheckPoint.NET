using Newtonsoft.Json;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Login Message Full Details
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.LoginMessage" />
    public class LoginMessageDetails : LoginMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginMessageDetails" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="showMessage">if set to <c>true</c> show message.</param>
        /// <param name="warning">if set to <c>true</c> show warning.</param>
        [JsonConstructor]
        protected LoginMessageDetails(string header, string message, bool showMessage, bool warning) : base(header, message)
        {
            ShowMessage = showMessage;
            Warning = warning;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether login message should be shown at login.
        /// </summary>
        /// <value><c>true</c> if message shown; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "show-message")]
        public bool ShowMessage { get; }

        /// <summary>
        /// Gets a value indicating whether the warning sign should be added.
        /// </summary>
        /// <value><c>true</c> if warning sign added; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "warning")]
        public bool Warning { get; }

        #endregion Properties
    }
}