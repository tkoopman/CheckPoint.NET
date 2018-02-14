using Newtonsoft.Json;
using System;

namespace Koopman.CheckPoint
{
    [Flags]
    public enum Days
    {
        [JsonIgnore]
        None = 0,

        Sun = 1,
        Mon = 2,
        Tue = 4,
        Wed = 8,
        Thu = 16,
        Fri = 32,
        Sat = 64
    }
}