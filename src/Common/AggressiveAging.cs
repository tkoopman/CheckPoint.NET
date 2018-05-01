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

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Sets short (aggressive) timeouts for idle connections.
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class AggressiveAging : ChangeTracking
    {
        #region Fields

        private bool _enable;
        private int _timeout;
        private bool _useDefaultTimeout;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the default timeout.
        /// </summary>
        /// <value>The default timeout.</value>
        [JsonProperty(PropertyName = "default-timeout")]
        public int DefaultTimeout { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether aggressive aging is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "enable")]
        public bool Enable
        {
            get => _enable;

            set
            {
                _enable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the aggressive aging timeout in seconds.
        /// </summary>
        /// <value>The timeout.</value>
        [JsonProperty(PropertyName = "timeout")]
        public int Timeout
        {
            get => _timeout;

            set
            {
                _timeout = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use default timeout.
        /// </summary>
        /// <value><c>true</c> if using default timeout; otherwise, <c>false</c>.</value>
        [JsonProperty(PropertyName = "use-default-timeout")]
        public bool UseDefaultTimeout
        {
            get => _useDefaultTimeout;

            set
            {
                _useDefaultTimeout = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}