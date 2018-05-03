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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        #region Fields

        private SemaphoreSlim ExportLock = new SemaphoreSlim(0);

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the cancellation token for async operations.
        /// </summary>
        /// <value>The cancellation token.</value>
        public CancellationToken CancellationToken { get; set; } = default;

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

        /// <summary>
        /// Gets or sets the provider for progress updates. Progress is given in number of objects
        /// added to export.
        /// </summary>
        /// <value>The provider for progress updates.</value>
        public IProgress<int> Progress { get; set; }

        [JsonProperty(PropertyName = "objects")]
        internal IEnumerable<IObjectSummary> ExportObjects => Objects.Values;

        internal ConcurrentDictionary<string, IObjectSummary> Objects { get; } = new ConcurrentDictionary<string, IObjectSummary>();

        [JsonProperty(PropertyName = "where-used")]
        internal ConcurrentDictionary<string, WhereUsed> WhereUsed { get; } = new ConcurrentDictionary<string, WhereUsed>();

        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds all objects by calling <see cref="AddAsync(IObjectSummary, int)" /> for each object
        /// </summary>
        /// <param name="objs">The objects.</param>
        /// <param name="maxDepth">
        /// The maximum depth of finding related objects. A value of 0 or less will just add this
        /// object and no related objects.
        /// </param>
        public System.Threading.Tasks.Task AddAsync(IEnumerable<IObjectSummary> objs, int maxDepth = int.MaxValue)
        {
            ExportLock.Release();
            try
            {
                var tasks = new List<System.Threading.Tasks.Task>();
                if (objs != null)
                    foreach (var obj in objs)
                        tasks.Add(AddAsync(obj, maxDepth));

                System.Threading.Tasks.Task.WaitAll(tasks.ToArray(), CancellationToken);
            }
            finally
            {
                ExportLock.Wait();
            }

            return System.Threading.Tasks.Task.FromResult(0);
        }

        /// <summary>
        /// Adds the specified access rule base and all related objects to export.
        /// </summary>
        /// <param name="accessRules">The access rule base.</param>
        /// <param name="maxDepth">
        /// The maximum depth of finding related objects. A value of 0 or less will just add this
        /// object and no related objects.
        /// </param>
        public System.Threading.Tasks.Task AddAsync(AccessRulebasePagingResults accessRules, int maxDepth = int.MaxValue)
        {
            if (accessRules == null) return System.Threading.Tasks.Task.FromResult(0);
            ExportLock.Release();
            try
            {
                var tasks = new System.Threading.Tasks.Task[2];
                tasks[0] = AddAsync(accessRules.Objects, maxDepth);
                tasks[1] = AddAsync(accessRules.Rulebase, maxDepth);
                System.Threading.Tasks.Task.WaitAll(tasks, CancellationToken);
            }
            finally
            {
                ExportLock.Wait();
            }
            return System.Threading.Tasks.Task.FromResult(0);
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
        public async System.Threading.Tasks.Task AddAsync(IObjectSummary objectSummary, int maxDepth = int.MaxValue)
        {
            CancellationToken.ThrowIfCancellationRequested();
            ExportLock.Release();

            try
            {
                if (objectSummary == null) return;
                if (objectSummary.UID == null)
                    objectSummary = await ReloadAsync(objectSummary);

                if (objectSummary.UID != null && !Objects.ContainsKey(objectSummary.UID))
                {
                    if (objectSummary.DetailLevel == DetailLevels.UID)
                        objectSummary = await ReloadAsync(objectSummary);

                    if (Contains(objectSummary.Name, ExcludeByName) || Contains(objectSummary.Type, ExcludeByType)) return;

                    objectSummary = await ReloadAsync(objectSummary);

                    if (!Objects.TryAdd(objectSummary.UID, objectSummary)) return;
                    Progress?.Report(Count);

                    if (maxDepth <= 0 || Contains(objectSummary.Name, ExcludeDetailsByName) || Contains(objectSummary.Type, ExcludeDetailsByType)) return;
                    maxDepth -= 1;
                    var tasks = new List<System.Threading.Tasks.Task>();
                    switch (objectSummary)
                    {
                        case AccessRule accessRule:
                            tasks.Add(AddAsync(accessRule.Layer, maxDepth));
                            tasks.Add(AddAsync(accessRule.Source, maxDepth));
                            tasks.Add(AddAsync(accessRule.Destination, maxDepth));
                            tasks.Add(AddAsync(accessRule.Service, maxDepth));
                            tasks.Add(AddAsync(accessRule.InstallOn, maxDepth));
                            tasks.Add(AddAsync(accessRule.InlineLayer, maxDepth));
                            tasks.Add(AddAsync(accessRule.Content, maxDepth));
                            tasks.Add(AddAsync(accessRule.VPN, maxDepth));
                            break;

                        case AccessSection accessSection:
                            tasks.Add(AddAsync(accessSection.Objects, maxDepth));
                            tasks.Add(AddAsync(accessSection.Rulebase, maxDepth));
                            break;

                        case AddressRange addressRange:
                            tasks.Add(AddAsync(addressRange.Groups, maxDepth));
                            break;

                        case ApplicationCategory applicationCategory:
                            tasks.Add(AddAsync(applicationCategory.Groups, maxDepth));
                            break;

                        case ApplicationGroup applicationGroup:
                            tasks.Add(AddAsync(applicationGroup.Groups, maxDepth));
                            tasks.Add(AddAsync(applicationGroup.Members, maxDepth));
                            break;

                        case ApplicationSite applicationSite:
                            tasks.Add(AddAsync(applicationSite.Groups, maxDepth));
                            break;

                        case Group group:
                            tasks.Add(AddAsync(group.Groups, maxDepth));
                            tasks.Add(AddAsync(group.Members, maxDepth));
                            break;

                        case GroupWithExclusion groupWithExclusion:
                            tasks.Add(AddAsync(groupWithExclusion.Include, maxDepth));
                            tasks.Add(AddAsync(groupWithExclusion.Except, maxDepth));
                            break;

                        case Host host:
                            tasks.Add(AddAsync(host.Groups, maxDepth));
                            break;

                        case MulticastAddressRange multicastAddressRange:
                            tasks.Add(AddAsync(multicastAddressRange.Groups, maxDepth));
                            break;

                        case Network network:
                            tasks.Add(AddAsync(network.Groups, maxDepth));
                            break;

                        case ServiceDceRpc serviceDceRpc:
                            tasks.Add(AddAsync(serviceDceRpc.Groups, maxDepth));
                            break;

                        case ServiceGroup serviceGroup:
                            tasks.Add(AddAsync(serviceGroup.Groups, maxDepth));
                            tasks.Add(AddAsync(serviceGroup.Members, maxDepth));
                            break;

                        case ServiceICMP serviceICMP:
                            tasks.Add(AddAsync(serviceICMP.Groups, maxDepth));
                            break;

                        case ServiceICMP6 serviceICMP6:
                            tasks.Add(AddAsync(serviceICMP6.Groups, maxDepth));
                            break;

                        case ServiceOther serviceOther:
                            tasks.Add(AddAsync(serviceOther.Groups, maxDepth));
                            break;

                        case ServiceRPC serviceRPC:
                            tasks.Add(AddAsync(serviceRPC.Groups, maxDepth));
                            break;

                        case ServiceSCTP serviceSCTP:
                            tasks.Add(AddAsync(serviceSCTP.Groups, maxDepth));
                            break;

                        case ServiceTCP serviceTCP:
                            tasks.Add(AddAsync(serviceTCP.Groups, maxDepth));
                            break;

                        case ServiceUDP serviceUDP:
                            tasks.Add(AddAsync(serviceUDP.Groups, maxDepth));
                            break;

                        case SimpleGateway simpleGateway:
                            tasks.Add(AddAsync(simpleGateway.Groups, maxDepth));
                            break;

                        case Time time:
                            tasks.Add(AddAsync(time.Groups, maxDepth));
                            break;

                        case TimeGroup timeGroup:
                            tasks.Add(AddAsync(timeGroup.Groups, maxDepth));
                            tasks.Add(AddAsync(timeGroup.Members, maxDepth));
                            break;
                    }

                    System.Threading.Tasks.Task.WaitAll(tasks.ToArray(), CancellationToken);
                }
            }
            finally
            {
                ExportLock.Wait();
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
        public async System.Threading.Tasks.Task AddAsync(string identifier, WhereUsed whereUsed, int maxDepth = int.MaxValue)
        {
            ExportLock.Release();
            try
            {
                WhereUsed[identifier] = whereUsed;
                await AddAsync(whereUsed.UsedDirectly, maxDepth);
                await AddAsync(whereUsed.UsedIndirectly, maxDepth);
            }
            finally
            {
                ExportLock.Wait();
            }
        }

        /// <summary>
        /// Exports this instance. Will wait for all currently running AddAsync methods to complete
        /// before exporting.
        /// </summary>
        /// <param name="indent">if set to <c>true</c> JSON output will be formatted with indents.</param>
        /// <returns>JSON data of all included export data</returns>
        public async System.Threading.Tasks.Task<string> Export(bool indent = false)
        {
            while (ExportLock.CurrentCount > 0)
                await System.Threading.Tasks.Task.Delay(1000, CancellationToken);

            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = ExportContractResolver.Instance,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = (indent) ? Formatting.Indented : Formatting.None
            });
        }

        private async System.Threading.Tasks.Task AddAsync(WhereUsed.WhereUsedResults results, int maxDepth)
        {
            if (results == null) return;

            await AddAsync(results.Objects, maxDepth);

            foreach (var rule in results.AccessControlRules)
            {
                await AddAsync(rule.Layer, maxDepth);
                await AddAsync(rule.Rule, maxDepth);
                await AddAsync(rule.Package, maxDepth);
            }

            foreach (var rule in results.NatRules)
                await AddAsync(rule.Rule, maxDepth);

            foreach (var rule in results.ThreatPreventionRules)
            {
                await AddAsync(rule.Layer, maxDepth);
                await AddAsync(rule.Rule, maxDepth);
            }
        }

        private bool Contains(string value, string[] array)
        {
            if (array == null) return false;
            return array.Contains(value, StringComparer.CurrentCultureIgnoreCase);
        }

        private async Task<IObjectSummary> ReloadAsync(IObjectSummary objectSummary)
        {
            try
            {
                objectSummary = await objectSummary.ReloadAsync(true);
            }
            catch (GenericException ge) { Session.WriteDebug(ge.ToString(true)); }

            return objectSummary;
        }

        #endregion Methods
    }
}