using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Results from <see cref="Session.WhereUsed(string, DetailLevels, bool, int)"/>
    /// </summary>
    public class WhereUsed
    {
        [JsonConstructor]
        private WhereUsed(WhereUsedResults usedDirectly, WhereUsedResults usedIndirectly)
        {
            UsedDirectly = usedDirectly;
            UsedIndirectly = usedIndirectly;
        }

        /// <summary>
        /// Gets the direct usage results.
        /// </summary>
        /// <value>
        /// The direct usage results.
        /// </value>
        [JsonProperty(PropertyName = "used-directly")]
        public WhereUsedResults UsedDirectly { get; }

        /// <summary>
        /// Gets the indirect usage results.
        /// </summary>
        /// <value>
        /// The indirect usage results.
        /// </value>
        [JsonProperty(PropertyName = "used-indirectly")]
        public WhereUsedResults UsedIndirectly { get; }

        /// <summary>
        /// Where Used Object Details
        /// </summary>
        public class WhereUsedResults
        {
            [JsonConstructor]
            private WhereUsedResults(int total, IObjectSummary[] objects, IObjectSummary[] accessControlRules, IObjectSummary[] natRules, IObjectSummary[] threatPreventionRules)
            {
                Total = total;
                Objects = objects;
                AccessControlRules = accessControlRules;
                NatRules = natRules;
                ThreatPreventionRules = threatPreventionRules;
            }

            /// <summary>
            /// Gets the total.
            /// </summary>
            /// <value>
            /// The total.
            /// </value>
            [JsonProperty(PropertyName = "total")]
            public int Total { get; }

            /// <summary>
            /// Gets the objects.
            /// </summary>
            /// <value>
            /// The objects.
            /// </value>
            [JsonProperty(PropertyName = "objects")]
            public IObjectSummary[] Objects { get; }

            /// <summary>
            /// Gets the access control rules.
            /// </summary>
            /// <value>
            /// The access control rules.
            /// </value>
            //[JsonProperty(PropertyName = "access-control-rules")]
            public IObjectSummary[] AccessControlRules { get; }

            /// <summary>
            /// Gets the nat rules.
            /// </summary>
            /// <value>
            /// The nat rules.
            /// </value>
            //[JsonProperty(PropertyName = "nat-rules")]
            public IObjectSummary[] NatRules { get; }

            /// <summary>
            /// Gets the threat prevention rules.
            /// </summary>
            /// <value>
            /// The threat prevention rules.
            /// </value>
            //[JsonProperty(PropertyName = "threat-prevention-rules")]
            public IObjectSummary[] ThreatPreventionRules { get; }
        }
    }
}
