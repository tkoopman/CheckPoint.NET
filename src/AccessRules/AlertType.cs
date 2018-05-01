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
using System.Runtime.Serialization;
using static Koopman.CheckPoint.Json.EnumConverter;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Type of Check Point Alert to raise
    /// </summary>
    [JsonConverter(typeof(EnumConverter), StringCases.Lowercase, " ")]
    public enum AlertType
    {
        /// <summary>
        /// No alerts
        /// </summary>
        None,

        /// <summary>
        /// Alert
        /// </summary>
        Alert,

        /// <summary>
        /// E-mail alert
        /// </summary>
        Mail,

        /// <summary>
        /// Send SNMP trap alert
        /// </summary>
        SNMP,

        /// <summary>
        /// User defined alert No1
        /// </summary>
        [EnumMember(Value = "user alert 1")]
        UserAlert1,

        /// <summary>
        /// User defined alert No2
        /// </summary>
        [EnumMember(Value = "user alert 2")]
        UserAlert2,

        /// <summary>
        /// User defined alert No3
        /// </summary>
        [EnumMember(Value = "user alert 3")]
        UserAlert3
    }
}