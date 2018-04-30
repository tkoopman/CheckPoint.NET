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
using Koopman.CheckPoint.Exceptions;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Koopman.CheckPoint.Internal
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class Export
    {

        public Export(Session session)
        {
            Session = session;
        }
        #region Properties

        private Session Session { get; }

        [JsonProperty(PropertyName = "where-used")]
        internal Dictionary<string, WhereUsed> WhereUsed = new Dictionary<string, WhereUsed>();

        [JsonProperty(PropertyName = "objects")]
        private IEnumerable<IObjectSummary> ExportObjects { get => Objects.Values; }

        private Dictionary<string, IObjectSummary> Objects { get; } = new Dictionary<string, IObjectSummary>();

        #endregion Properties

        #region Methods

        public void Populate() => Populate(WhereUsed.Values);

        public void Populate(object o)
        {
            switch (o)
            {
                case IEnumerable enumerable:
                    foreach (object e in enumerable)
                            Populate(e);
                    break;

                case IObjectSummary objectSummary:
                    try
                    {
                        if (objectSummary.UID == null)
                            objectSummary = objectSummary.Reload(true);
                    }
                    catch (GenericException ge)
                    {
                        Session.WriteDebug(ge.ToString(true));
                    }
                    if (objectSummary.UID != null && !Objects.ContainsKey(objectSummary.UID))
                    {
                        try
                        {
                            objectSummary = objectSummary.Reload(true);
                        }
                        catch (GenericException ge)
                        {
                            Session.WriteDebug(ge.ToString(true));
                        }
                        Objects.Add(objectSummary.UID, objectSummary);
                        switch (objectSummary)
                        {
                            case AccessRule accessRule:
                                Populate(accessRule.Layer);
                                Populate(accessRule.Source);
                                Populate(accessRule.Destination);
                                Populate(accessRule.Service);
                                Populate(accessRule.InstallOn);
                                Populate(accessRule.InlineLayer);
                                break;
                            case AccessSection accessSection:
                                Populate(accessSection.Objects);
                                Populate(accessSection.Rulebase);
                                break;

                            case AccessRulebasePagingResults accessRules:
                                Populate(accessRules.Objects);
                                Populate(accessRules.Rulebase);
                                break;

                            case AddressRange addressRange:
                                Populate(addressRange.Groups);
                                break;

                            case ApplicationCategory applicationCategory:
                                Populate(applicationCategory.Groups);
                                break;

                            case ApplicationGroup applicationGroup:
                                Populate(applicationGroup.Groups);
                                Populate(applicationGroup.Members);
                                break;

                            case ApplicationSite applicationSite:
                                Populate(applicationSite.Groups);
                                break;

                            case Group group:
                                Populate(group.Groups);
                                Populate(group.Members);
                                break;

                            case GroupWithExclusion groupWithExclusion:
                                Populate(groupWithExclusion.Include);
                                Populate(groupWithExclusion.Except);
                                break;

                            case Host host:
                                Populate(host.Groups);
                                break;

                            case MulticastAddressRange multicastAddressRange:
                                Populate(multicastAddressRange.Groups);
                                break;

                            case Network network:
                                Populate(network.Groups);
                                break;

                            case ServiceDceRpc serviceDceRpc:
                                Populate(serviceDceRpc.Groups);
                                break;

                            case ServiceGroup serviceGroup:
                                Populate(serviceGroup.Groups);
                                Populate(serviceGroup.Members);
                                break;

                            case ServiceICMP serviceICMP:
                                Populate(serviceICMP.Groups);
                                break;

                            case ServiceICMP6 serviceICMP6:
                                Populate(serviceICMP6.Groups);
                                break;

                            case ServiceOther serviceOther:
                                Populate(serviceOther.Groups);
                                break;

                            case ServiceRPC serviceRPC:
                                Populate(serviceRPC.Groups);
                                break;

                            case ServiceSCTP serviceSCTP:
                                Populate(serviceSCTP.Groups);
                                break;

                            case ServiceTCP serviceTCP:
                                Populate(serviceTCP.Groups);
                                break;

                            case ServiceUDP serviceUDP:
                                Populate(serviceUDP.Groups);
                                break;

                            case SimpleGateway simpleGateway:
                                Populate(simpleGateway.Groups);
                                break;

                            case Time time:
                                Populate(time.Groups);
                                break;

                            case TimeGroup timeGroup:
                                Populate(timeGroup.Groups);
                                Populate(timeGroup.Members);
                                break;
                        }
                    }
                    break;

                case WhereUsed whereUsed:
                    Populate(whereUsed.UsedDirectly);
                    Populate(whereUsed.UsedIndirectly);
                    break;

                case WhereUsed.WhereUsedResults results:
                    Populate(results.Objects);
                    Populate(results.AccessControlRules);
                    Populate(results.NatRules);
                    Populate(results.ThreatPreventionRules);
                    break;

                case WhereUsed.WhereUsedResults.Rules rules:
                    Populate(rules.Layer);
                    Populate(rules.Rule);
                    break;

                case WhereUsed.WhereUsedResults.NATs nats:
                    Populate(nats.Rule);
                    break;

                case WhereUsed.WhereUsedResults.ThreatRules threatRules:
                    Populate(threatRules.Layer);
                    Populate(threatRules.Rule);
                    break;
            }
        }

        public bool ShouldSerializeWhereUsed() => WhereUsed.Count > 0;

        #endregion Methods
    }
}