using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;

namespace Koopman.CheckPoint
{
    public class SessionInfo
    {
        #region Constructors

        [JsonConstructor]
        private SessionInfo(Session session, string application, int changes, Colors color, string comments, ConnectionModes connectionMode, string description, string email, string icon, IPAddress iPAddress, bool isExpiredSession, bool isInWork, DateTime lastLoginTime, DateTime lastLogoutTime, int locks, string name, string phoneNumber, DateTime publishTime, bool readOnly, int sessionTimeout, States state, Tag[] tags, string type, string uID, string userName)
        {
            Application = application;
            Changes = changes;
            Color = color;
            Comments = comments;
            ConnectionMode = connectionMode;
            Description = description;
            Email = email;
            Icon = icon;
            IPAddress = iPAddress;
            IsExpiredSession = isExpiredSession;
            IsInWork = isInWork;
            LastLoginTime = lastLoginTime;
            LastLogoutTime = lastLogoutTime;
            Locks = locks;
            Name = name;
            PhoneNumber = phoneNumber;
            PublishTime = publishTime;
            ReadOnly = readOnly;
            SessionTimeout = sessionTimeout;
            State = state;
            Tags = tags;
            Type = type;
            UID = uID;
            UserName = userName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the application serving the web_api requests.
        /// </summary>
        /// <value>The application name.</value>
        [JsonProperty(PropertyName = "application")]
        public string Application { get; }

        /// <summary>
        /// Gets the number of pending changes.
        /// </summary>
        /// <value>Number of pending changes.</value>
        [JsonProperty(PropertyName = "changes")]
        public int Changes { get; }

        /// <summary>
        /// Color of the object
        /// </summary>
        [JsonProperty(PropertyName = "color")]
        public Colors Color { get; }

        /// <summary>
        /// Comments string
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; }

        /// <summary>
        /// Gets the connection mode.
        /// </summary>
        /// <value>The connection mode.</value>
        [JsonProperty(PropertyName = "connection-mode")]
        public ConnectionModes ConnectionMode { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty(PropertyName = "description")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Description { get; }

        /// <summary>
        /// Gets the Administrator's email address.
        /// </summary>
        /// <value>The Administrator's email address.</value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; }

        /// <summary>
        /// Object icon
        /// </summary>
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; }

        /// <summary>
        /// Gets the IP address from which the session was initiated.
        /// </summary>
        /// <value>The IP address from which the session was initiated.</value>
        [JsonProperty(PropertyName = "ip-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPAddress { get; }

        /// <summary>
        /// Gets a value indicating whether this session is expired.
        /// </summary>
        /// <value><c>true</c> if this session is expired; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "expired-session")]
        public bool IsExpiredSession { get; }

        /// <summary>
        /// Gets a value indicating whether this session is in work state.
        /// </summary>
        /// <value><c>true</c> if this session is in work state; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "in-work")]
        public bool IsInWork { get; }

        /// <summary>
        /// Gets the last login time.
        /// </summary>
        /// <value>The last login time.</value>
        [JsonProperty(PropertyName = "last-login-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime LastLoginTime { get; }

        /// <summary>
        /// Gets the last logout time.
        /// </summary>
        /// <value>The last logout time.</value>
        [JsonProperty(PropertyName = "last-logout-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime LastLogoutTime { get; }

        /// <summary>
        /// Gets the number of locked objects.
        /// </summary>
        /// <value>Number of locked objects.</value>
        [JsonProperty(PropertyName = "locks")]
        public int Locks { get; }

        /// <summary>
        /// Session name.
        /// </summary>
        /// <value>The object's name.</value>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        /// <summary>
        /// Gets the Administrator's phone number.
        /// </summary>
        /// <value>The Administrator's phone number.</value>
        [JsonProperty(PropertyName = "phone-number")]
        public string PhoneNumber { get; }

        /// <summary>
        /// Gets the timestamp when user published changes on the management server.
        /// </summary>
        /// <value>The timestamp when user published changes on the management server.</value>
        [JsonProperty(PropertyName = "publish-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime PublishTime { get; }

        /// <summary>
        /// Indicates whether the session is read-only
        /// </summary>
        [JsonProperty(PropertyName = "read-only")]
        public bool ReadOnly { get; }

        /// <summary>
        /// Gets the session expiration timeout in seconds.
        /// </summary>
        /// <value>Session expiration timeout in seconds.</value>
        [JsonProperty(PropertyName = "session-timeout")]
        public int SessionTimeout { get; }

        /// <summary>
        /// Gets the session state.
        /// </summary>
        /// <value>The session state.</value>
        [JsonProperty(PropertyName = "state")]
        public States State { get; }

        /// <summary>
        /// Tags assigned to object
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        public Tag[] Tags { get; }

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Session unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; }

        /// <summary>
        /// Gets the name of the logged in user.
        /// </summary>
        /// <value>The name of the logged in user.</value>
        [JsonProperty(PropertyName = "user-name")]
        public string UserName { get; }

        #endregion Properties

        #region Enums

        [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, " ")]
        public enum ConnectionModes
        {
            ReadWrite,
            ReadOnly,
            ReadWriteWithGlobalLock
        }

        [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, " ")]
        public enum States
        {
            Open,
            Published,
            Discarded
        }

        #endregion Enums
    }
}