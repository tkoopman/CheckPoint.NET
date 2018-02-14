using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Koopman.CheckPoint
{
    [JsonConverter(typeof(EnumConverter))]
    [Flags]
    public enum Days
    {
        [JsonIgnore]
        None = 0,

        [EnumMember(Value = "Sun")]
        Sunday = 1,

        [EnumMember(Value = "Mon")]
        Monday = 2,

        [EnumMember(Value = "Tue")]
        Tuesday = 4,

        [EnumMember(Value = "Wed")]
        Wednesday = 8,

        [EnumMember(Value = "Thu")]
        Thursday = 16,

        [EnumMember(Value = "Fri")]
        Friday = 32,

        [EnumMember(Value = "Sat")]
        Saturday = 64
    }
}