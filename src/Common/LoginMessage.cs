using Newtonsoft.Json;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Login Message
    /// </summary>
    public class LoginMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginMessage" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        [JsonConstructor]
        protected LoginMessage(string header, string message)
        {
            Header = header;
            Message = message;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the login message header.
        /// </summary>
        /// <value>The header.</value>
        [JsonProperty(PropertyName = "header")]
        public string Header { get; }

        /// <summary>
        /// Gets the login message.
        /// </summary>
        /// <value>The message.</value>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Message;
        }

        #endregion Methods
    }
}