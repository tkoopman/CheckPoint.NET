using Koopman.CheckPoint.Common;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Rulebase Actions
    /// </summary>
    public class RulebaseAction : IObjectSummary
    {
        #region Fields

        /// <summary>
        /// The accept action
        /// </summary>
        public static readonly RulebaseAction Accept = new RulebaseAction(Domain.DataDomain, "Accept", "6c488338-8eec-4103-ad21-cd461ac2c472");

        /// <summary>
        /// The ask action
        /// </summary>
        public static readonly RulebaseAction Ask = new RulebaseAction(Domain.DataDomain, "Ask", "942f11f5-4160-40a8-8832-878d3db83998");

        /// <summary>
        /// The client authentication action
        /// </summary>
        public static readonly RulebaseAction ClientAuth = new RulebaseAction(Domain.DataDomain, "Client Auth", "7af86cd9-2733-4fff-8341-3d4965e81994");

        /// <summary>
        /// The drop action
        /// </summary>
        public static readonly RulebaseAction Drop = new RulebaseAction(Domain.DataDomain, "Drop", "6c488338-8eec-4103-ad21-cd461ac2c473");

        /// <summary>
        /// The inform action
        /// </summary>
        public static readonly RulebaseAction Inform = new RulebaseAction(Domain.DataDomain, "Inform", "eb542a2f-9df8-47db-939d-dba9c1b1082d");

        /// <summary>
        /// The inline layer action
        /// </summary>
        public static readonly RulebaseAction InlineLayer = new RulebaseAction(Domain.DataDomain, "Inline Layer", "ea28da66-c5ed-11e2-bc66-aa5c6188709b", "Global");

        /// <summary>
        /// The reject action
        /// </summary>
        public static readonly RulebaseAction Reject = new RulebaseAction(Domain.DataDomain, "Reject", "6c488338-8eec-4103-ad21-cd461ac2c474");

        /// <summary>
        /// The user authentication action
        /// </summary>
        public static readonly RulebaseAction UserAuth = new RulebaseAction(Domain.DataDomain, "User Auth", "641ce709-989a-4941-8ab4-2f9bcb9d17dd");

        internal static readonly RulebaseAction[] Actions = new RulebaseAction[] { Accept, Drop, InlineLayer, Reject, Ask, Inform, UserAuth, ClientAuth };

        #endregion Fields

        #region Constructors

        private RulebaseAction(Domain domain, string name, string uID, string type = "RulebaseAction")
        {
            Domain = domain;
            Name = name;
            UID = uID;
            Type = type;
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public DetailLevels DetailLevel => DetailLevels.Full;

        /// <inheritdoc />
        public Domain Domain { get; }

        /// <inheritdoc />
        public bool IsNew => false;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public string UID { get; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        /// <inheritdoc />
        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard) => this;

        /// <inheritdoc />
        public override string ToString() => GetIdentifier();

        #endregion Methods
    }
}