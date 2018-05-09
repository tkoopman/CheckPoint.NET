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
    /// <para type="synopsis">Response from Remove-CheckPointIdentity</para>
    /// <para type="description"></para>
    /// </summary>
    public class DeleteIdentityResponse
    {
        #region Properties

        /// <summary>
        /// <para type="description">Number of deleted identities</para>
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public uint Count { get; set; }

        /// <summary>
        /// <para type="description">Deleted IPv4 association</para>
        /// </summary>
        [JsonProperty(PropertyName = "ipv4-address")]
        public string IPv4Address { get; set; }

        /// <summary>
        /// <para type="description">Deleted IPv6 association</para>
        /// </summary>
        [JsonProperty(PropertyName = "ipv6-address")]
        public string IPv6Address { get; set; }

        /// <summary>
        /// <para type="description">Textual description of the command’s result</para>
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        #endregion Properties
    }
}