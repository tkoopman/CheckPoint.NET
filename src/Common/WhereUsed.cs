using Newtonsoft.Json;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Results from <see cref="Session.FindWhereUsed(string, DetailLevels, bool, int)" />
    /// </summary>
    public class WhereUsed
    {
        #region Constructors

        [JsonConstructor]
        private WhereUsed(WhereUsedResults usedDirectly, WhereUsedResults usedIndirectly)
        {
            UsedDirectly = usedDirectly;
            UsedIndirectly = usedIndirectly;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the direct usage results.
        /// </summary>
        /// <value>The direct usage results.</value>
        [JsonProperty(PropertyName = "used-directly")]
        public WhereUsedResults UsedDirectly { get; }

        /// <summary>
        /// Gets the indirect usage results.
        /// </summary>
        /// <value>The indirect usage results.</value>
        [JsonProperty(PropertyName = "used-indirectly")]
        public WhereUsedResults UsedIndirectly { get; }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Where Used Object Details
        /// </summary>
        public class WhereUsedResults
        {
            #region Constructors

            [JsonConstructor]
            private WhereUsedResults(int total, IObjectSummary[] objects, Rules[] accessControlRules, NATs[] natRules, ThreatRules[] threatPreventionRules)
            {
                Total = total;
                Objects = objects;
                AccessControlRules = accessControlRules;
                NatRules = natRules;
                ThreatPreventionRules = threatPreventionRules;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the access control rules.
            /// </summary>
            /// <value>The access control rules.</value>
            [JsonProperty(PropertyName = "access-control-rules")]
            public Rules[] AccessControlRules { get; }

            /// <summary>
            /// Gets the nat rules.
            /// </summary>
            /// <value>The nat rules.</value>
            [JsonProperty(PropertyName = "nat-rules")]
            public NATs[] NatRules { get; }

            /// <summary>
            /// Gets the objects.
            /// </summary>
            /// <value>The objects.</value>
            [JsonProperty(PropertyName = "objects")]
            public IObjectSummary[] Objects { get; }

            /// <summary>
            /// Gets the threat prevention rules.
            /// </summary>
            /// <value>The threat prevention rules.</value>
            [JsonProperty(PropertyName = "threat-prevention-rules")]
            public ThreatRules[] ThreatPreventionRules { get; }

            /// <summary>
            /// Gets the total.
            /// </summary>
            /// <value>The total.</value>
            [JsonProperty(PropertyName = "total")]
            public int Total { get; }

            #endregion Properties

            #region Classes

            /// <summary>
            /// Where Used NAT results
            /// </summary>
            public class NATs
            {
                #region Constructors

                /// <summary>
                /// JSON Constructor for Where Used NAT Results
                /// </summary>
                /// <param name="rule">The rule.</param>
                /// <param name="ruleColumns">The rule columns.</param>
                /// <param name="position">The position.</param>
                /// <param name="package">The package.</param>
                [JsonConstructor]
                private NATs(IObjectSummary rule, string[] ruleColumns, string position, IObjectSummary package)
                {
                    Rule = rule;
                    RuleColumns = ruleColumns;
                    Position = position;
                    Package = package;
                }

                #endregion Constructors

                #region Properties

                /// <summary>
                /// Package NAT rule exists in.
                /// </summary>
                /// <value>The package.</value>
                [JsonProperty(PropertyName = "package", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Package { get; }

                /// <summary>
                /// Rule position
                /// </summary>
                /// <value>The position.</value>
                [JsonProperty(PropertyName = "position", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public string Position { get; }

                /// <summary>
                /// NAT Rule found
                /// </summary>
                /// <value>The rule.</value>
                [JsonProperty(PropertyName = "rule", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Rule { get; }

                /// <summary>
                /// Columns where object is used in rule.
                /// </summary>
                /// <value>The rule columns.</value>
                [JsonProperty(PropertyName = "rule-columns", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public string[] RuleColumns { get; }

                #endregion Properties
            }

            /// <summary>
            /// Where Used Rule results
            /// </summary>
            public class Rules
            {
                #region Constructors

                /// <summary>
                /// JSON Constructor for Where Used Rule Results
                /// </summary>
                /// <param name="rule">The rule.</param>
                /// <param name="ruleColumns">The rule columns.</param>
                /// <param name="position">The position.</param>
                /// <param name="layer">The layer.</param>
                [JsonConstructor]
                private Rules(IObjectSummary rule, string[] ruleColumns, int position, IObjectSummary layer)
                {
                    Rule = rule;
                    RuleColumns = ruleColumns;
                    Position = position;
                    Layer = layer;
                }

                #endregion Constructors

                #region Properties

                /// <summary>
                /// Layer rule exists in.
                /// </summary>
                /// <value>The layer.</value>
                [JsonProperty(PropertyName = "layer", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Layer { get; }

                /// <summary>
                /// Rule position
                /// </summary>
                /// <value>The position.</value>
                [JsonProperty(PropertyName = "position", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public int Position { get; }

                /// <summary>
                /// Access control rule found
                /// </summary>
                /// <value>The rule.</value>
                [JsonProperty(PropertyName = "rule", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Rule { get; }

                /// <summary>
                /// Columns where object is used in rule.
                /// </summary>
                /// <value>The rule columns.</value>
                [JsonProperty(PropertyName = "rule-columns", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public string[] RuleColumns { get; }

                #endregion Properties
            }

            /// <summary>
            /// Where used threat rule results
            /// </summary>
            public class ThreatRules
            {
                #region Constructors

                /// <summary>
                /// JSON Constructor for Where Used Threat Rule Results
                /// </summary>
                /// <param name="rule">The rule.</param>
                /// <param name="ruleColumns">The rule columns.</param>
                /// <param name="position">The position.</param>
                /// <param name="layer">The layer.</param>
                /// <param name="layerPosition">The layer position.</param>
                /// <param name="package">The package.</param>
                [JsonConstructor]
                private ThreatRules(IObjectSummary rule, string[] ruleColumns, string position, IObjectSummary layer, string layerPosition, IObjectSummary package)
                {
                    Rule = rule;
                    RuleColumns = ruleColumns;
                    Position = position;
                    Layer = layer;
                    LayerPosition = layerPosition;
                    Package = package;
                }

                #endregion Constructors

                #region Properties

                /// <summary>
                /// Layer rule exists in.
                /// </summary>
                /// <value>The layer.</value>
                [JsonProperty(PropertyName = "layer", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Layer { get; }

                /// <summary>
                /// Layer position
                /// </summary>
                /// <value>The layer position.</value>
                [JsonProperty(PropertyName = "layer-position", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public string LayerPosition { get; private set; }

                /// <summary>
                /// Package NAT rule exists in.
                /// </summary>
                /// <value>The package.</value>
                [JsonProperty(PropertyName = "package", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Package { get; }

                /// <summary>
                /// Rule position
                /// </summary>
                /// <value>The position.</value>
                [JsonProperty(PropertyName = "position", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public string Position { get; }

                /// <summary>
                /// Threat rule found
                /// </summary>
                /// <value>The rule.</value>
                [JsonProperty(PropertyName = "rule", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public IObjectSummary Rule { get; }

                /// <summary>
                /// Columns where object is used in rule.
                /// </summary>
                /// <value>The rule columns.</value>
                [JsonProperty(PropertyName = "rule-columns", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
                public string[] RuleColumns { get; }

                #endregion Properties
            }

            #endregion Classes
        }

        #endregion Classes
    }
}