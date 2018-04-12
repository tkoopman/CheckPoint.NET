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

using Koopman.CheckPoint.AccessRules;
using Koopman.CheckPoint.Common;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Access Rule Object
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    public class AccessRule : ObjectBase<AccessRule>
    {
        #region Constructors

        public AccessRule(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRule" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal AccessRule(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _content = new MemberMembershipChangeTracking<IObjectSummary>(this);
            _destination = new MemberMembershipChangeTracking<IObjectSummary>(this);
            _service = new MemberMembershipChangeTracking<IObjectSummary>(this);
            _source = new MemberMembershipChangeTracking<IObjectSummary>(this);
        }

        #endregion Constructors

        #region Fields

        private RulebaseAction _action;
        private ActionSettings _actionSettings;
        private MemberMembershipChangeTracking<IObjectSummary> _content;
        private bool _contentNegate;
        private CustomFields _customFields;
        private MemberMembershipChangeTracking<IObjectSummary> _destination;
        private bool _destinationNegate;
        private bool _enabled;
        private AccessLayer _inlineLayer;
        private MemberMembershipChangeTracking<IObjectSummary> _service;
        private bool _serviceNegate;
        private MemberMembershipChangeTracking<IObjectSummary> _source;
        private bool _sourceNegate;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Rule action
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">Action</exception>
        [JsonProperty(PropertyName = "action", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public RulebaseAction Action
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _action : null;
            set
            {
                _action = value ?? throw new System.ArgumentNullException(nameof(Action));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Rule action settings
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">ActionSettings</exception>
        [JsonProperty(PropertyName = "action-settings", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ActionSettings ActionSettings
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _actionSettings : null;
            set
            {
                _actionSettings = value ?? throw new System.ArgumentNullException(nameof(ActionSettings));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Content objects.
        /// </summary>
        [JsonProperty(PropertyName = "content")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<IObjectSummary> Content
        {
            get => _content;
            internal set => _content = value;
        }

        /// <summary>
        /// True if negate is set for data.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">ContentNegate</exception>
        [JsonProperty(PropertyName = "content-negate", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? ContentNegate
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _contentNegate : (bool?)null;
            set
            {
                _contentNegate = value ?? throw new System.ArgumentNullException(nameof(ContentNegate));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Custom Fields
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">CustomFields</exception>
        [JsonProperty(PropertyName = "custom-fields", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public CustomFields CustomFields
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _customFields : null;
            set
            {
                _customFields = value ?? throw new System.ArgumentNullException(nameof(CustomFields));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Destination objects.
        /// </summary>
        [JsonProperty(PropertyName = "destination")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<IObjectSummary> Destination
        {
            get => _destination;
            internal set => _destination = value;
        }

        /// <summary>
        /// True if negate is set for destination.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">DestinationNegate</exception>
        [JsonProperty(PropertyName = "destination-negate", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? DestinationNegate
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _destinationNegate : (bool?)null;
            set
            {
                _destinationNegate = value ?? throw new System.ArgumentNullException(nameof(DestinationNegate));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Enable/Disable the rule.
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">Enabled</exception>
        [JsonProperty(PropertyName = "enabled", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? Enabled
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _enabled : (bool?)null;
            set
            {
                _enabled = value ?? throw new System.ArgumentNullException(nameof(Enabled));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Inline Layer
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">InlineLayer</exception>
        [JsonProperty(PropertyName = "inline-layer", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public AccessLayer InlineLayer
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _inlineLayer : null;
            set
            {
                _inlineLayer = value ?? throw new System.ArgumentNullException(nameof(InlineLayer));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Rule layer
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "layer", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public AccessLayer Layer { get; private set; }

        /// <summary>
        /// Service objects.
        /// </summary>
        [JsonProperty(PropertyName = "service")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<IObjectSummary> Service
        {
            get => _service;
            internal set => _service = value;
        }

        /// <summary>
        /// True if negate is set for service.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">ServiceNegate</exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "service-negate", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? ServiceNegate
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _serviceNegate : (bool?)null;
            set
            {
                _serviceNegate = value ?? throw new System.ArgumentNullException(nameof(ServiceNegate));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Source objects.
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<IObjectSummary> Source
        {
            get => _source;
            internal set => _source = value;
        }

        /// <summary>
        /// True if negate is set for source.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">SourceNegate</exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "source-negate", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? SourceNegate
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _sourceNegate : (bool?)null;
            set
            {
                _sourceNegate = value ?? throw new System.ArgumentNullException(nameof(SourceNegate));
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Valid sort orders for Access Rules
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