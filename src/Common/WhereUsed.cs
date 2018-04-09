using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koopman.CheckPoint.Common
{
    public class WhereUsed
    {
        [JsonConstructor]
        private WhereUsed(WhereUsedResults usedDirectly, WhereUsedResults usedIndirectly)
        {
            UsedDirectly = usedDirectly;
            UsedIndirectly = usedIndirectly;
        }

        [JsonProperty(PropertyName = "used-directly")]
        public WhereUsedResults UsedDirectly { get; }

        [JsonProperty(PropertyName = "used-indirectly")]
        public WhereUsedResults UsedIndirectly { get; }

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

            [JsonProperty(PropertyName = "total")]
            public int Total { get; }

            [JsonProperty(PropertyName = "objects")]
            public IObjectSummary[] Objects { get; }

            //[JsonProperty(PropertyName = "access-control-rules")]
            public IObjectSummary[] AccessControlRules { get; }

            //[JsonProperty(PropertyName = "nat-rules")]
            public IObjectSummary[] NatRules { get; }

            //[JsonProperty(PropertyName = "threat-prevention-rules")]
            public IObjectSummary[] ThreatPreventionRules { get; }
        }
    }
}
