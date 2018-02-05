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

using Newtonsoft.Json;

namespace Koopman.CheckPoint.Exceptions
{
    /// <summary>
    /// <para type="description">Extra details of errors or warnings</para>
    /// </summary>
    public class CheckPointErrorDetails
    {
        #region Constructors

        /// <summary>
        /// JSON Constructor for Errors
        /// </summary>
        [JsonConstructor]
        private CheckPointErrorDetails(string message, bool currentSession)
        {
            CurrentSession = currentSession;
            Message = message;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// <para type="description">Validation related to the current session.</para>
        /// </summary>
        [JsonProperty(PropertyName = "current-session", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public bool CurrentSession { get; private set; }

        /// <summary>
        /// <para type="description">Validation message.</para>
        /// </summary>
        [JsonProperty(PropertyName = "message", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; private set; }

        #endregion Properties
    }
}