using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Koopman.CheckPoint.Common
{
    public abstract class SimpleChangeTracking : IChangeTracking
    {
        #region Properties

        [JsonIgnore]
        public virtual bool IsChanged { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsNew { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether this instance is deserializing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deserializing; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        protected internal bool IsDeserializing { get; private set; } = false;

        #endregion Properties

        #region Methods

        public virtual void AcceptChanges()
        {
            throw new NotImplementedException("Use AcceptChanges from Parent Object.");
        }

        protected virtual void OnDeserialized()
        {
            IsChanged = false;
        }

        protected virtual void OnDeserializing()
        {
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            IsNew = false;
            OnDeserialized();
        }

        [OnDeserializing]
        private void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
            OnDeserializing();
        }

        #endregion Methods
    }
}