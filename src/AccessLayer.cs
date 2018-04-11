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
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Access Layer Object
    /// </summary>
    /// <example>
    /// Add new security zone using <see cref="AccessLayer.AccessLayer(Session, bool)" />
    /// <code>
    /// var al = new AccessLayer(Session) {
    ///     Name = "MyAccessLayer"
    /// };
    /// al.AcceptChanges();
    /// </code>
    /// Find access layer using <see cref="Session.FindAccessLayer(string, DetailLevels)" />
    /// <code>
    /// var al = Session.FindAccessLayer("MyAccessLayer");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    public class AccessLayer : ObjectBase<SecurityZone>
    {
        #region Constructors

        /// <summary>
        /// Create new <see cref="AccessLayer" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var al = new AccessLayer(Session) {
        ///     Name = "MyAccessLayer"
        /// };
        /// al.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public AccessLayer(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessLayer" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal AccessLayer(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
        }

        #endregion Constructors

        #region Fields

        private bool _applicationsAndUrlFiltering;
        private bool _contentAwareness;
        private bool _detectUsingXForwardFor;
        private bool _firewall;
        private bool _mobileAccess;
        private bool _shared;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Whether Applications and URL Filtering blade is enabled on this layer.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">ApplicationsAndUrlFiltering</exception>
        [JsonProperty(PropertyName = "applications-and-url-filtering", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? ApplicationsAndUrlFiltering
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _applicationsAndUrlFiltering : (bool?)null;
            set
            {
                _applicationsAndUrlFiltering = value ?? throw new System.ArgumentNullException(nameof(ApplicationsAndUrlFiltering));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether Content Awareness blade is enabled on this layer.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">ContentAwareness</exception>
        [JsonProperty(PropertyName = "content-awareness", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? ContentAwareness
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _contentAwareness : (bool?)null;
            set
            {
                _contentAwareness = value ?? throw new System.ArgumentNullException(nameof(ContentAwareness));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether X-Forward-For HTTP header is been used.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">DetectUsingXForwardFor</exception>
        [JsonProperty(PropertyName = "detect-using-x-forward-for", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? DetectUsingXForwardFor
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _detectUsingXForwardFor : (bool?)null;
            set
            {
                _detectUsingXForwardFor = value ?? throw new System.ArgumentNullException(nameof(DetectUsingXForwardFor));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether Firewall blade is enabled on this layer.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Firewall</exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "firewall", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? Firewall
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _firewall : (bool?)null;
            set
            {
                _firewall = value ?? throw new System.ArgumentNullException(nameof(Firewall));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether Mobile Access blade is enabled on this layer.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">MobileAccess</exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "mobile-access", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? MobileAccess
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _mobileAccess : (bool?)null;
            set
            {
                _mobileAccess = value ?? throw new System.ArgumentNullException(nameof(MobileAccess));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether this layer is shared.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Shared</exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "shared", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? Shared
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _shared : (bool?)null;
            set
            {
                _shared = value ?? throw new System.ArgumentNullException(nameof(Shared));
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Valid sort orders forAccess Layers
        /// </summary>
        public static class Order
        {
            #region Fields

            /// <summary>
            /// Sort by name in ascending order
            /// </summary>
            public readonly static IOrder NameAsc = new OrderAscending("name");

            /// <summary>
            /// Sort by name in descending order
            /// </summary>
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        #endregion Classes
    }
}