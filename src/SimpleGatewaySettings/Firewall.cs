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
using System;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    /// <summary>
    /// Simple Gateway Firewall Settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class Firewall : ChangeTracking
    {
        #region Fields

        private bool _autoCalculateConnectionsHashTableSizeAndMemoryPool;
        private bool _autoMaximumLimitForConcurrentConnections;
        private int _connectionsHashSize;
        private int _maximumLimitForConcurrentConnections;
        private int _maximumMemoryPoolSize;
        private int _memoryPoolSize;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether automatic calculate connections hash table size
        /// and memory pool setting is enabled or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatic calculate connections hash table size and memory pool is
        /// enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "auto-calculate-connections-hash-table-size-and-memory-pool")]
        public bool AutoCalculateConnectionsHashTableSizeAndMemoryPool
        {
            get => _autoCalculateConnectionsHashTableSizeAndMemoryPool;
            set
            {
                _autoCalculateConnectionsHashTableSizeAndMemoryPool = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic maximum limit for concurrent
        /// connections is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatic maximum limit for concurrent connections enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "auto-maximum-limit-for-concurrent-connections")]
        public bool AutoMaximumLimitForConcurrentConnections
        {
            get => _autoMaximumLimitForConcurrentConnections;
            set
            {
                _autoMaximumLimitForConcurrentConnections = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the connections hash.
        /// </summary>
        /// <value>The size of the connections hash.</value>
        [JsonProperty(PropertyName = "connections-hash-size")]
        public int ConnectionsHashSize
        {
            get => _connectionsHashSize;
            set
            {
                _connectionsHashSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the maximum limit for concurrent connections.
        /// </summary>
        /// <value>The maximum limit for concurrent connections.</value>
        [JsonProperty(PropertyName = "maximum-limit-for-concurrent-connections")]
        public int MaximumLimitForConcurrentConnections
        {
            get => _maximumLimitForConcurrentConnections;
            set
            {
                _maximumLimitForConcurrentConnections = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the maximum size of the memory pool.
        /// </summary>
        /// <value>The maximum size of the memory pool.</value>
        [JsonProperty(PropertyName = "maximum-memory-pool-size")]
        public int MaximumMemoryPoolSize
        {
            get => _maximumMemoryPoolSize;
            set
            {
                _maximumMemoryPoolSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the memory pool.
        /// </summary>
        /// <value>The size of the memory pool.</value>
        [JsonProperty(PropertyName = "memory-pool-size")]
        public int MemoryPoolSize
        {
            get => _memoryPoolSize;
            set
            {
                _memoryPoolSize = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        /// <exception cref="NotImplementedException">Use AcceptChanges from Parent Object.</exception>
        public override void AcceptChanges()
        {
            throw new NotImplementedException("Use AcceptChanges from Parent Object.");
        }

        #endregion Methods
    }
}