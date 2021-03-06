﻿// MIT License
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
using System.Diagnostics;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Common properties all objects contain when full detail level returned
    /// </summary>
    /// <typeparam name="T">Type from derived classes</typeparam>
    /// <seealso cref="Koopman.CheckPoint.ObjectSummary" />
    public abstract class ObjectBase<T> : ObjectSummary<T> where T : IObjectSummary
    {
        #region Fields

        private Colors _color;
        private string _comments;
        private string _icon;
        private MetaInfo _metaInfo;
        private bool _readOnly;
        private bool _setIfExists = false;
        private MemberMembershipChangeTracking<Tag> _tags;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase{T}" /> class.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level.</param>
        protected ObjectBase(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _tags = new MemberMembershipChangeTracking<Tag>(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Color of the object
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">Color</exception>
        [JsonProperty(PropertyName = "color", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Colors? Color
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _color : (Colors?)null;
            set
            {
                _color = value ?? throw new System.ArgumentNullException(nameof(Color));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Comments string
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "comments")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Comments
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _comments : null;
            set
            {
                _comments = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Object icon
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "icon")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Icon
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _icon : null;
            private set => _icon = value;
        }

        /// <summary>
        /// Meta Information
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        [JsonProperty(PropertyName = "meta-info", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MetaInfo MetaInfo
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _metaInfo : null;
            private set => _metaInfo = value;
        }

        /// <summary>
        /// Indicates whether the object is read-only
        /// </summary>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of <see cref="DetailLevels.Full" /></remarks>
        /// <exception cref="System.ArgumentNullException">ReadOnly</exception>
        [JsonProperty(PropertyName = "read-only")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool? ReadOnly
        {
            get => (TestDetailLevel(DetailLevels.Full)) ? _readOnly : (bool?)null;
            set => _readOnly = value ?? throw new System.ArgumentNullException(nameof(ReadOnly));
        }

        /// <summary>
        /// Tags assigned to object
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemberMembershipChangeTracking<Tag> Tags
        {
            get => _tags;
            internal set => _tags = value;
        }

        /// <summary>
        /// If another object with the same name already exists, it will be updated. Pay attention
        /// that original object's fields will be overwritten by the fields provided in the request payload!
        /// </summary>
        [JsonProperty(PropertyName = "set-if-exists")]
        protected bool SetIfExists
        {
            get => _setIfExists;
            set
            {
                if (_setIfExists == value) return;
                _setIfExists = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}