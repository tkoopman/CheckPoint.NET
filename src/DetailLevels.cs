// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Koopman.CheckPoint.Json;
using Newtonsoft.Json;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Detail level of object to retrieve.
    /// </summary>
    /// <remarks>
    /// For standard Find methods Detail Level will be for related objects. Like groups & tags.
    /// For FindAll methods is is the actual Detail Level of the main objects returned.
    /// </remarks>
    [JsonConverter(typeof(EnumConverter), StringCases.Lowercase)]
    public enum DetailLevels
    {
        // Not currently allowing UID option
        //UID,

        /// <summary>
        /// Check Point standard level of detail
        /// </summary>
        Standard,

        /// <summary>
        /// Return full details.
        /// </summary>
        /// <remarks>
        /// Can cause a lot of data to be returned, taking extra time to execute, especially in large environments.
        /// </remarks>
        Full
    }
}