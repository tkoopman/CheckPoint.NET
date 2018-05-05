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

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Net;

namespace Koopman.CheckPoint.Special
{
    /// <summary>
    /// Cluster Member object
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.GenericObjectSummary" />
    public class CpmiClusterMember : GenericObjectSummary, IGroupMember
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CpmiClusterMember" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="detailLevel">The detail level.</param>
        internal CpmiClusterMember(Session session, DetailLevels detailLevel) : base(session, detailLevel, "CpmiClusterMember", false)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        [JsonProperty(PropertyName = "color")]
        public Colors Color { get; private set; }

        /// <summary>
        /// Group memberships.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        public Group[] Groups { get; private set; }

        /// <summary>
        /// Gets or sets the IPv4 address of this host.
        /// </summary>
        [JsonProperty(PropertyName = "ipv4-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPv4Address { get; private set; }

        /// <summary>
        /// Gets or sets the IPv6 address of this host.
        /// </summary>
        [JsonProperty(PropertyName = "ipv6-address")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress IPv6Address { get; private set; }

        #endregion Properties
    }
}