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
    public class TimeGroup : ObjectBase
    {
        #region Fields

        private ObjectMembershipChangeTracking<TimeGroup> _groups;
        private ObjectMembershipChangeTracking<ObjectSummary> _members;

        #endregion Fields

        #region Constructors

        public TimeGroup(Session session) : this(session, DetailLevels.Full)
        {
        }

        protected internal TimeGroup(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
            _groups = new ObjectMembershipChangeTracking<TimeGroup>(this);
            _members = new ObjectMembershipChangeTracking<ObjectSummary>(this);
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "groups")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<TimeGroup> Groups
        {
            get => _groups;
            internal set => _groups = value;
        }

        [JsonProperty(PropertyName = "members")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectMembershipChangeTracking<ObjectSummary> Members
        {
            get => _members;
            internal set => _members = value;
        }

        #endregion Properties

        #region Classes

        public static class Order
        {
            #region Fields

            public readonly static IOrder NameAsc = new OrderAscending("name");
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        #endregion Classes
    }
}