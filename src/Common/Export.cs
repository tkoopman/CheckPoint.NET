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

using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Used to export selected objects to JSON
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonExport
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExport" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="excludeDetailsByType">Types to exclude details of.</param>
        /// <param name="excludeDetailsByName">Names to exclude details of.</param>
        /// <param name="excludeByType">Types to exclude.</param>
        /// <param name="excludeByName">Names to exclude.</param>
        public JsonExport(
            Session session,
            string[] excludeDetailsByType = null,
            string[] excludeDetailsByName = null,
            string[] excludeByType = null,
            string[] excludeByName = null)
        {
            Session = session;
            ExcludeDetailsByType = excludeDetailsByType;
            ExcludeDetailsByName = excludeDetailsByName;
            ExcludeByType = excludeByType;
            ExcludeByName = excludeByName;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty(PropertyName = "where-used")]
        internal Dictionary<string, WhereUsed> WhereUsed = new Dictionary<string, WhereUsed>();

        /// <summary>
        /// Gets the number of objects to be exported.
        /// </summary>
        /// <value>The number of objects to export.</value>
        public int Count => Objects.Count;

        /// <summary>
        /// Gets the names to exclude.
        /// </summary>
        public string[] ExcludeByName { get; }

        /// <summary>
        /// Gets the types to exclude.
        /// </summary>
        public string[] ExcludeByType { get; }

        /// <summary>
        /// Gets the names to exclude details of.
        /// </summary>
        public string[] ExcludeDetailsByName { get; }

        /// <summary>
        /// Gets the types to exclude details of.
        /// </summary>
        public string[] ExcludeDetailsByType { get; }

        [JsonProperty(PropertyName = "objects")]
        internal IEnumerable<IObjectSummary> ExportObjects => Objects.Values;

        internal Dictionary<string, IObjectSummary> Objects { get; } = new Dictionary<string, IObjectSummary>();
        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds all objects by calling <see cref="Add(IObjectSummary, int)" /> for each object
        /// </summary>
        /// <param name="objs">The objects.</param>
        /// <param name="maxDepth">
        /// The maximum depth of finding related objects. A value of 0 or less will just add this
        /// object and no related objects.
        /// </param>
        public void Add(IEnumerable<IObjectSummary> objs, int maxDepth = int.MaxValue)
        {
            if (objs == null) return;
            foreach (var obj in objs)
                Add(obj, maxDepth);
        }

        /// <summary>
        /// Adds the specified object to the export database if it not already included. Will also
        /// include any related objects to this object like groups. Calling this will also
        /// automatically cause a reload of this object if, object not already in export database and
        /// it doesn't already contain all the data for export.
        /// </summary>
        /// <param name="objectSummary">The object to export.</param>
        /// <param name="maxDepth">
        /// The maximum depth of finding related objects. A value of 0 or less will just add this
        /// object and no related objects.
        /// </param>
        public void Add(IObjectSummary objectSummary, int maxDepth = int.MaxValue)
        {
            if (objectSummary == null) return;
            try
            {
                if (objectSummary.UID == null)
                    objectSummary = objectSummary.Reload(true);
            }
            catch (GenericException ge) { Session.WriteDebug(ge.ToString(true)); }

            if (objectSummary.UID != null && !Objects.ContainsKey(objectSummary.UID))
            {
                if (Contains(objectSummary.Name, ExcludeByName) || Contains(objectSummary.Type, ExcludeByType)) return;

                try
                {
                    objectSummary = objectSummary.Reload(true);
                }
                catch (GenericException ge) { Session.WriteDebug(ge.ToString(true)); }

                Objects.Add(objectSummary.UID, objectSummary);

                if (maxDepth <= 0 || Contains(objectSummary.Name, ExcludeDetailsByName) || Contains(objectSummary.Type, ExcludeDetailsByType)) return;
                maxDepth -= 1;
                switch (objectSummary)
                {
                    case AccessRule accessRule:
                        Add(accessRule.Layer, maxDepth);
                        Add(accessRule.Source, maxDepth);
                        Add(accessRule.Destination, maxDepth);
                        Add(accessRule.Service, maxDepth);
                        Add(accessRule.InstallOn, maxDepth);
                        Add(accessRule.InlineLayer, maxDepth);
                        Add(accessRule.Content, maxDepth);
                        Add(accessRule.VPN, maxDepth);
                        break;

                    case AccessSection accessSection:
                        Add(accessSection.Objects, maxDepth);
                        Add(accessSection.Rulebase, maxDepth);
                        break;

                    case AccessRulebasePagingResults accessRules:
                        Add(accessRules.Objects, maxDepth);
                        Add(accessRules.Rulebase, maxDepth);
                        break;

                    case AddressRange addressRange:
                        Add(addressRange.Groups, maxDepth);
                        break;

                    case ApplicationCategory applicationCategory:
                        Add(applicationCategory.Groups, maxDepth);
                        break;

                    case ApplicationGroup applicationGroup:
                        Add(applicationGroup.Groups, maxDepth);
                        Add(applicationGroup.Members, maxDepth);
                        break;

                    case ApplicationSite applicationSite:
                        Add(applicationSite.Groups, maxDepth);
                        break;

                    case Group group:
                        Add(group.Groups, maxDepth);
                        Add(group.Members, maxDepth);
                        break;

                    case GroupWithExclusion groupWithExclusion:
                        Add(groupWithExclusion.Include, maxDepth);
                        Add(groupWithExclusion.Except, maxDepth);
                        break;

                    case Host host:
                        Add(host.Groups, maxDepth);
                        break;

                    case MulticastAddressRange multicastAddressRange:
                        Add(multicastAddressRange.Groups, maxDepth);
                        break;

                    case Network network:
                        Add(network.Groups, maxDepth);
                        break;

                    case ServiceDceRpc serviceDceRpc:
                        Add(serviceDceRpc.Groups, maxDepth);
                        break;

                    case ServiceGroup serviceGroup:
                        Add(serviceGroup.Groups, maxDepth);
                        Add(serviceGroup.Members, maxDepth);
                        break;

                    case ServiceICMP serviceICMP:
                        Add(serviceICMP.Groups, maxDepth);
                        break;

                    case ServiceICMP6 serviceICMP6:
                        Add(serviceICMP6.Groups, maxDepth);
                        break;

                    case ServiceOther serviceOther:
                        Add(serviceOther.Groups, maxDepth);
                        break;

                    case ServiceRPC serviceRPC:
                        Add(serviceRPC.Groups, maxDepth);
                        break;

                    case ServiceSCTP serviceSCTP:
                        Add(serviceSCTP.Groups, maxDepth);
                        break;

                    case ServiceTCP serviceTCP:
                        Add(serviceTCP.Groups, maxDepth);
                        break;

                    case ServiceUDP serviceUDP:
                        Add(serviceUDP.Groups, maxDepth);
                        break;

                    case SimpleGateway simpleGateway:
                        Add(simpleGateway.Groups, maxDepth);
                        break;

                    case Time time:
                        Add(time.Groups, maxDepth);
                        break;

                    case TimeGroup timeGroup:
                        Add(timeGroup.Groups, maxDepth);
                        Add(timeGroup.Members, maxDepth);
                        break;
                }
            }
        }

        /// <summary>
        /// Adds all objects related to Where Used to export. Also exports the WhereUsed results
        /// linked to the identifier which can be used for highlighting.
        /// </summary>
        /// <param name="identifier">The identifier WhereUsed was based on.</param>
        /// <param name="whereUsed">The where used results.</param>
        /// <param name="maxDepth">
        /// The maximum depth of finding related objects. A value of 0 or less will just add this
        /// object and no related objects.
        /// </param>
        public void Add(string identifier, WhereUsed whereUsed, int maxDepth = int.MaxValue)
        {
            WhereUsed[identifier] = whereUsed;
            Add(whereUsed.UsedDirectly, maxDepth);
            Add(whereUsed.UsedIndirectly, maxDepth);
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <param name="indent">if set to <c>true</c> JSON output will be formatted with indents.</param>
        /// <returns>JSON data of all included export data</returns>
        public string Export(bool indent = false) =>
            JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = ExportContractResolver.Instance,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = (indent) ? Formatting.Indented : Formatting.None
            });

        private void Add(WhereUsed.WhereUsedResults results, int maxDepth)
        {
            if (results == null) return;

            Add(results.Objects, maxDepth);

            foreach (var rule in results.AccessControlRules)
            {
                Add(rule.Layer, maxDepth);
                Add(rule.Rule, maxDepth);
                Add(rule.Package, maxDepth);
            }

            foreach (var rule in results.NatRules)
                Add(rule.Rule, maxDepth);

            foreach (var rule in results.ThreatPreventionRules)
            {
                Add(rule.Layer, maxDepth);
                Add(rule.Rule, maxDepth);
            }
        }

        private bool Contains(string value, string[] array)
        {
            if (array == null) return false;
            return array.Contains(value, StringComparer.CurrentCultureIgnoreCase);
        }

        #endregion Methods
    }
}