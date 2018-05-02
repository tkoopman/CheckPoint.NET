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

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Access rule track settings
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class Track : ChangeTracking
    {
        #region Fields

        private bool _accounting;
        private AlertType _alert;
        private bool _perConnection;
        private bool _perSession;
        private TrackType _type;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets if accounting is enabled.
        /// </summary>
        /// <value>The accounting status.</value>
        [JsonProperty(PropertyName = "accounting")]
        public bool Accounting
        {
            get => _accounting;
            set
            {
                _accounting = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the alert type for track.
        /// </summary>
        /// <value>The alert type.</value>
        [JsonProperty(PropertyName = "alert")]
        public AlertType Alert
        {
            get => _alert;
            set
            {
                _alert = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if logs are genereated per connection.
        /// </summary>
        /// <value>The per connection logging status.</value>
        [JsonProperty(PropertyName = "per-connection")]
        public bool PerConnection
        {
            get => _perConnection;
            set
            {
                _perConnection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if logs are genereated per session.
        /// </summary>
        /// <value>The per session logging status.</value>
        [JsonProperty(PropertyName = "per-session")]
        public bool PerSession
        {
            get => _perSession;
            set
            {
                _perSession = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the type of tracking to use.
        /// </summary>
        /// <value>The tracking type.</value>
        [JsonProperty(PropertyName = "type")]
        public TrackType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => _type?.ToString();

        #endregion Methods
    }
}