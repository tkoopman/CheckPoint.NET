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

using Newtonsoft.Json;

namespace Koopman.CheckPoint.IA
{
    /// <summary>
    /// <para type="synopsis">Response from Get-CheckPointIdentity</para>
    /// <para type="description"></para>
    /// </summary>
    public class ShowIdentityResponse
    {
        #region Properties

        /// <summary>
        /// <para type="description">
        /// List of all the access roles on this IP, for auditing and enforcement purposes.
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "combined-roles")]
        public string[] CombinedRoles { get; set; }

        /// <summary>
        /// <para type="description">Queried IPv4 identity</para>
        /// </summary>
        [JsonProperty(PropertyName = "ipv4-address")]
        public string IPv4Address { get; set; }

        /// <summary>
        /// <para type="description">Queried IPv6 identity</para>
        /// </summary>
        [JsonProperty(PropertyName = "ipv6-address")]
        public string IPv6Address { get; set; }

        /// <summary>
        /// <para type="description">Computer name, if available</para>
        /// </summary>
        [JsonProperty(PropertyName = "machine")]
        public string Machine { get; set; }

        /// <summary>
        /// <para type="description">List of computer groups</para>
        /// </summary>
        [JsonProperty(PropertyName = "machine-groups")]
        public string[] MachineGroups { get; set; }

        /// <summary>
        /// <para type="description">
        /// Machine session’s identity source, if the machine session is available.
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "machine-identity-source")]
        public string MachineIdentitySource { get; set; }

        /// <summary>
        /// <para type="description">Textual description of the command’s result</para>
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// <para type="description">All user identities on this IP.</para>
        /// </summary>
        [JsonProperty(PropertyName = "users")]
        public UserResponse[] Users { get; set; }

        #endregion Properties

        #region Classes

        /// <summary>
        /// <para type="synopsis">Response from Get-CheckPointIdentity Users parameter</para>
        /// <para type="description"></para>
        /// </summary>
        public class UserResponse
        {
            #region Properties

            /// <summary>
            /// <para type="description">Array of groups</para>
            /// </summary>
            [JsonProperty(PropertyName = "groups")]
            public string[] Groups { get; set; }

            /// <summary>
            /// <para type="description">Identity source</para>
            /// </summary>
            [JsonProperty(PropertyName = "identity-source")]
            public string IdentitySource { get; set; }

            /// <summary>
            /// <para type="description">Array of roles</para>
            /// </summary>
            [JsonProperty(PropertyName = "roles")]
            public string[] Roles { get; set; }

            /// <summary>
            /// <para type="description">
            /// Users' full names (full name if available, falls back to user name if not)
            /// </para>
            /// </summary>
            [JsonProperty(PropertyName = "user")]
            public string User { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}