using Koopman.CheckPoint.Common;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Rulebase Actions
    /// </summary>
    public class RulebaseAction : IObjectSummary
    {
        #region Fields

        public static readonly RulebaseAction Accept = new RulebaseAction(Domain.DataDomain, "Accept", "6c488338-8eec-4103-ad21-cd461ac2c472");
        public static readonly RulebaseAction Ask = new RulebaseAction(Domain.DataDomain, "Ask", "942f11f5-4160-40a8-8832-878d3db83998");
        public static readonly RulebaseAction ClientAuth = new RulebaseAction(Domain.DataDomain, "Client Auth", "7af86cd9-2733-4fff-8341-3d4965e81994");
        public static readonly RulebaseAction Drop = new RulebaseAction(Domain.DataDomain, "Drop", "6c488338-8eec-4103-ad21-cd461ac2c473");
        public static readonly RulebaseAction Inform = new RulebaseAction(Domain.DataDomain, "Inform", "eb542a2f-9df8-47db-939d-dba9c1b1082d");
        public static readonly RulebaseAction InlineLayer = new RulebaseAction(Domain.DataDomain, "Inline Layer", "ea28da66-c5ed-11e2-bc66-aa5c6188709b", "Global");
        public static readonly RulebaseAction Reject = new RulebaseAction(Domain.DataDomain, "Reject", "6c488338-8eec-4103-ad21-cd461ac2c474");
        public static readonly RulebaseAction UserAuth = new RulebaseAction(Domain.DataDomain, "User Auth", "641ce709-989a-4941-8ab4-2f9bcb9d17dd");
        internal static readonly RulebaseAction[] Actions = new RulebaseAction[] { Accept, Drop, Reject, Ask, Inform, InlineLayer, UserAuth, ClientAuth };

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

        public DetailLevels DetailLevel => DetailLevels.Full;

        public Domain Domain { get; }

        public bool IsNew => false;

        public string Name { get; }

        public string Type { get; }

        public string UID { get; }

        #endregion Properties

        #region Methods

        public string GetMembershipID() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard) => this;

        public override string ToString() => GetMembershipID();

        #endregion Methods
    }
}