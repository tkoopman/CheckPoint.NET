using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// The days of the week.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    [Flags]
    public enum Days
    {
        /// <summary>
        /// None
        /// </summary>
        [JsonIgnore]
        None = 0,

        /// <summary>
        /// Sunday
        /// </summary>
        [EnumMember(Value = "Sun")]
        Sunday = 1,

        /// <summary>
        /// Monday
        /// </summary>
        [EnumMember(Value = "Mon")]
        Monday = 2,

        /// <summary>
        /// Tuesday
        /// </summary>
        [EnumMember(Value = "Tue")]
        Tuesday = 4,

        /// <summary>
        /// Wednesday
        /// </summary>
        [EnumMember(Value = "Wed")]
        Wednesday = 8,

        /// <summary>
        /// Thursday
        /// </summary>
        [EnumMember(Value = "Thu")]
        Thursday = 16,

        /// <summary>
        /// Friday
        /// </summary>
        [EnumMember(Value = "Fri")]
        Friday = 32,

        /// <summary>
        /// Saturday
        /// </summary>
        [EnumMember(Value = "Sat")]
        Saturday = 64,

        /// <summary>
        /// All Weekdays. Monday - Friday
        /// </summary>
        [JsonIgnore]
        Weekdays = 62,

        /// <summary>
        /// All Weekend. Saturday and Sunday
        /// </summary>
        [JsonIgnore]
        Weekend = 65
    }
}