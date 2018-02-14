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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Koopman.CheckPoint.Common
{
    public abstract class SimpleChangeTracking : IChangeTracking
    {
        #region Properties

        [JsonIgnore]
        public virtual bool IsChanged { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool IsNew { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether this instance is deserializing.
        /// </summary>
        /// <value><c>true</c> if this instance is deserializing; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        protected internal bool IsDeserializing { get; private set; } = false;

        #endregion Properties

        #region Methods

        public virtual void AcceptChanges()
        {
            throw new NotImplementedException("Use AcceptChanges from Parent Object.");
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            IsNew = false;
            IsChanged = false;
            OnDeserialized();
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
            OnDeserializing();
        }

        protected virtual void OnDeserialized()
        {
        }

        protected virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Should be called, by the property setter, when any property value is updated. Does not
        /// need to be called for properties that implement IChangeTracking.
        /// </summary>
        /// <param name="propertyName">Name of the property being updated.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (!IsDeserializing)
            {
                IsChanged = true;
            }
        }

        #endregion Methods
    }
}