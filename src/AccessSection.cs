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
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Koopman.CheckPoint
{
    public class AccessSection : ObjectBase<AccessSection>, IRulebaseEntry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessSection" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal AccessSection(Session session) : base(session, DetailLevels.Full)
        {
        }

        #endregion Constructors

        /// <summary>
        /// Section starting rule number.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public int From { get; set; }

        /// <summary>
        /// Section last rule number.
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public int To { get; set; }
        
        /// <summary>
        /// <para type="description">
        /// How much details are returned depends on the details-level field of the request. This
        /// table shows the level of detail shown when details-level is set to standard.
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "objects-dictionary", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<IObjectSummary> Objects { get; internal set; }

        /// <summary>
        /// <para type="description">
        /// How much details are returned depends on the details-level field of the request. This
        /// table shows the level of detail shown when details-level is set to standard.
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "rulebase", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<IRulebaseEntry> Rulebase { get; internal set; }

        internal override void UpdateGenericMembers(ObjectConverter objectConverter)
        {
            base.UpdateGenericMembers(objectConverter);
            objectConverter.PostDeserilization(Objects);
            objectConverter.PostDeserilization(Rulebase);
        }

        #region Classes

        /// <summary>
        /// Valid sort orders for Security Zones
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