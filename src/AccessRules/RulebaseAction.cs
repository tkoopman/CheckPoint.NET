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
using System.Threading;
using System.Threading.Tasks;

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
        public static readonly RulebaseAction Accept = new RulebaseAction(Domain.DataDomain, "Accept", AcceptUID);

        /// <summary>
        /// The ask action
        /// </summary>
        public static readonly RulebaseAction Ask = new RulebaseAction(Domain.DataDomain, "Ask", AskUID);

        /// <summary>
        /// The client authentication action
        /// </summary>
        public static readonly RulebaseAction ClientAuth = new RulebaseAction(Domain.DataDomain, "Client Auth", ClientAuthUID);

        /// <summary>
        /// The drop action
        /// </summary>
        public static readonly RulebaseAction Drop = new RulebaseAction(Domain.DataDomain, "Drop", DropUID);

        /// <summary>
        /// The inform action
        /// </summary>
        public static readonly RulebaseAction Inform = new RulebaseAction(Domain.DataDomain, "Inform", InformUID);

        /// <summary>
        /// The inline layer action
        /// </summary>
        public static readonly RulebaseAction InnerLayer = new RulebaseAction(Domain.DataDomain, "Inner Layer", InnerLayerUID, "Global");

        /// <summary>
        /// The reject action
        /// </summary>
        public static readonly RulebaseAction Reject = new RulebaseAction(Domain.DataDomain, "Reject", RejectUID);

        /// <summary>
        /// The user authentication action
        /// </summary>
        public static readonly RulebaseAction UserAuth = new RulebaseAction(Domain.DataDomain, "User Auth", UserAuthUID);

        internal static readonly RulebaseAction[] Actions = new RulebaseAction[] { Drop, Accept, Reject, Ask, Inform, UserAuth, ClientAuth, InnerLayer };

        private const string AcceptUID = "6c488338-8eec-4103-ad21-cd461ac2c472";
        private const string AskUID = "942f11f5-4160-40a8-8832-878d3db83998";
        private const string ClientAuthUID = "7af86cd9-2733-4fff-8341-3d4965e81994";
        private const string DropUID = "6c488338-8eec-4103-ad21-cd461ac2c473";
        private const string InformUID = "eb542a2f-9df8-47db-939d-dba9c1b1082d";
        private const string InnerLayerUID = "ea28da66-c5ed-11e2-bc66-aa5c6188709b";
        private const string RejectUID = "6c488338-8eec-4103-ad21-cd461ac2c474";
        private const string UserAuthUID = "641ce709-989a-4941-8ab4-2f9bcb9d17dd";

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
        public ObjectType ObjectType => ObjectType.Unknown;

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public string UID { get; }

        #endregion Properties

        #region Methods

        public static implicit operator Actions(RulebaseAction a)
        {
            switch (a.UID)
            {
                case AcceptUID:
                    return AccessRules.Actions.Accept;

                case InnerLayerUID:
                    return AccessRules.Actions.InnerLayer;

                case AskUID:
                    return AccessRules.Actions.Ask;

                case ClientAuthUID:
                    return AccessRules.Actions.ClientAuth;

                case DropUID:
                    return AccessRules.Actions.Drop;

                case InformUID:
                    return AccessRules.Actions.Inform;

                case RejectUID:
                    return AccessRules.Actions.Reject;

                case UserAuthUID:
                    return AccessRules.Actions.UserAuth;

                default:
                    throw new System.InvalidCastException("Invalid Action");
            }
        }

        public static implicit operator RulebaseAction(string a)
        {
            switch (a.ToLower())
            {
                case "accept":
                    return RulebaseAction.Accept;

                case "applylayer":
                case "apply layer":
                case "innerlayer":
                case "inner layer":
                case "layer":
                    return RulebaseAction.InnerLayer;

                case "ask":
                    return RulebaseAction.Ask;

                case "clientauth":
                case "client auth":
                    return RulebaseAction.ClientAuth;

                case "drop":
                    return RulebaseAction.Drop;

                case "inform":
                    return RulebaseAction.Inform;

                case "reject":
                    return RulebaseAction.Reject;

                case "userauth":
                case "user auth":
                    return RulebaseAction.UserAuth;

                default:
                    throw new System.InvalidCastException("Invalid Action");
            }
        }

        public static implicit operator RulebaseAction(Actions a)
        {
            switch (a)
            {
                case AccessRules.Actions.Accept:
                    return RulebaseAction.Accept;

                case AccessRules.Actions.InnerLayer:
                    return RulebaseAction.InnerLayer;

                case AccessRules.Actions.Ask:
                    return RulebaseAction.Ask;

                case AccessRules.Actions.ClientAuth:
                    return RulebaseAction.ClientAuth;

                case AccessRules.Actions.Drop:
                    return RulebaseAction.Drop;

                case AccessRules.Actions.Inform:
                    return RulebaseAction.Inform;

                case AccessRules.Actions.Reject:
                    return RulebaseAction.Reject;

                case AccessRules.Actions.UserAuth:
                    return RulebaseAction.UserAuth;

                default:
                    throw new System.InvalidCastException("Invalid Action");
            }
        }

        public static implicit operator string(RulebaseAction a) => a.GetIdentifier();

        /// <inheritdoc />
        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        /// <inheritdoc />
        public Task<IObjectSummary> Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard, CancellationToken cancellationToken = default) => Task.FromResult((IObjectSummary)this);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => GetIdentifier();

        #endregion Methods
    }
}