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
using Newtonsoft.Json;

namespace Koopman.CheckPoint.Special
{
    /// <summary>
    /// Access Rule Limit object
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.GenericObjectSummary" />
    public class CpmiAppfwLimit : GenericObjectSummary
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CpmiAppfwLimit" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="detailLevel">The detail level.</param>
        internal CpmiAppfwLimit(Session session, DetailLevels detailLevel) : base(session, detailLevel, "CpmiAppfwLimit", false)
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
        /// Gets the maximum download limit.
        /// </summary>
        /// <value>The maximum download limit.</value>
        [JsonProperty(PropertyName = "maxDownloadLimit")]
        public string MaxDownloadLimit { get; private set; }

        /// <summary>
        /// Gets the maximum upload limit.
        /// </summary>
        /// <value>The maximum upload limit.</value>
        [JsonProperty(PropertyName = "maxUploadLimit")]
        public string MaxUploadLimit { get; private set; }

        #endregion Properties
    }
}