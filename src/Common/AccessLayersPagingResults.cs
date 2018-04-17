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
using System.Collections.Generic;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Result from Session.FindAccessLayers
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectsPagingResults{T,U}" />
    public class AccessLayersPagingResults : ObjectsPagingResults<AccessLayer, AccessLayersPagingResults>
    {
        #region Properties

        /// <summary>
        /// <para type="description">
        /// How much details are returned depends on the details-level field of the request. This
        /// table shows the level of detail shown when details-level is set to standard.
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "access-layers", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<AccessLayer> AccessLayers { get => _Objects; internal set => _Objects = value; }

        #endregion Properties
    }
}