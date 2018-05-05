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
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Used to keep track if any properties have been changed. Does not track which properties have
    /// changed, just that some have changed.
    /// </summary>
    /// <seealso cref="System.ComponentModel.IChangeTracking" />
    public abstract class SimpleChangeTracking : IChangeTracking
    {
        #region Properties

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether this instance is serializing.
        /// </summary>
        /// <value><c>true</c> if this instance is serialising; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        protected internal bool IsSerializing { get; private set; } = false;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public virtual Task AcceptChanges(CancellationToken cancellationToken = default) => throw new NotImplementedException("Use AcceptChanges from Parent Object.");

        void IChangeTracking.AcceptChanges() => AcceptChanges().GetAwaiter().GetResult();

        /// <summary>
        /// Called when deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            IsChanged = false;
            OnDeserialized();
        }

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
            IsNew = false;
            OnDeserializing();
        }

        /// <summary>
        /// Called when serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnSerialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            IsSerializing = false;
            OnSerialized();
        }

        /// <summary>
        /// Called when serializing.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            IsSerializing = true;
            OnSerializing();
        }

        /// <summary>
        /// Called when deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
        }

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        protected virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Should be called, by the property setter, when any property value is updated.
        /// </summary>
        /// <param name="propertyName">Name of the property being updated.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!IsDeserializing)
                IsChanged = true;
        }

        /// <summary>
        /// Called when serialized.
        /// </summary>
        protected virtual void OnSerialized()
        {
        }

        /// <summary>
        /// Called when Serializing.
        /// </summary>
        protected virtual void OnSerializing()
        {
        }

        #endregion Methods
    }
}