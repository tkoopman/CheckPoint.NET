// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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