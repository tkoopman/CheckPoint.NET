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

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Check Point Meta Information
    /// </summary>
    public class MetaInfo
    {
        #region Constructors

        [JsonConstructor]
        private MetaInfo(DateTime creationTime, string creator, string lastModifier, DateTime lastModifyTime, LockStates @lock, ValidationStates validationState)
        {
            CreationTime = creationTime;
            Creator = creator;
            LastModifier = lastModifier;
            LastModifyTime = lastModifyTime;
            Lock = @lock;
            ValidationState = validationState;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        [JsonProperty(PropertyName = "creation-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime CreationTime { get; }

        /// <summary>
        /// Gets the creator.
        /// </summary>
        /// <value>The creator.</value>
        [JsonProperty(PropertyName = "creator")]
        public string Creator { get; }

        /// <summary>
        /// Gets the last modifier.
        /// </summary>
        /// <value>The last modifier.</value>
        [JsonProperty(PropertyName = "last-modifier")]
        public string LastModifier { get; }

        /// <summary>
        /// Gets the last modify time.
        /// </summary>
        /// <value>The last modify time.</value>
        [JsonProperty(PropertyName = "last-modify-time")]
        [JsonConverter(typeof(CheckPointDateTimeConverter))]
        public DateTime LastModifyTime { get; }

        /// <summary>
        /// Gets the lock state.
        /// </summary>
        /// <value>The lock state.</value>
        [JsonProperty(PropertyName = "lock")]
        public LockStates Lock { get; }

        /// <summary>
        /// Gets the state of the validation.
        /// </summary>
        /// <value>The state of the validation.</value>
        [JsonProperty(PropertyName = "validation-state")]
        public ValidationStates ValidationState { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"Last modified by {LastModifier} on {LastModifyTime}";

        #endregion Methods
    }
}