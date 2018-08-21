using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Searches for usage of the target object in other objects and rules. Unlike
        /// <seealso cref="Session.FindWhereUsed(string, DetailLevels, bool, int, CancellationToken)" />,
        /// this method only makes indirect calls to the management server. It then follows only the
        /// object types defined in <paramref name="indirectTypes" />. This addresses the problem
        /// where the normal where-used can follow objects you may not wish to follow.
        /// </summary>
        /// <remarks>
        /// Due to the way this method manually follows indirect usages it will take longer on
        /// average than the normal where-used method. Also if/when Check Point implement this
        /// feature in the API natively this method will be depricated, in favor the standard API. If
        /// <paramref name="indirect" /> is <c>false</c> then this is identical to calling <seealso cref="Session.FindWhereUsed(string, DetailLevels, bool, int, CancellationToken)" />.
        /// </remarks>
        /// <param name="identifier">The object identifier to search for.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="indirect">if set to <c>true</c> results will include indirect uses.</param>
        /// <param name="indirectMaxDepth">The indirect maximum depth.</param>
        /// <param name="indirectTypes">
        /// Object types that should be followed. A value of <c>null</c> will default to only
        /// following groups.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the WhereUsed object
        /// </returns>
        public async Task<WhereUsed> FindWhereUsedCustom
            (
                string identifier,
                DetailLevels detailLevel = DetailLevels.Standard,
                bool indirect = false,
                int indirectMaxDepth = 5,
                ObjectType[] indirectTypes = null,
                CancellationToken cancellationToken = default
            )
        {
            if (indirectTypes == null)
                indirectTypes = new ObjectType[] { ObjectType.Group, ObjectType.GroupWithExclusion, ObjectType.ServiceGroup, ObjectType.ApplicationSiteGroup, ObjectType.TimeGroup };

            if (detailLevel == DetailLevels.UID)
                throw new System.Exception($"{nameof(detailLevel)} of UID not supported here.");

            if (indirect)
            {
                var custom = await FindWhereUsedCustomIndirect(
                    identifier: identifier,
                    objectConverter: null,
                    detailLevel: detailLevel,
                    indirectMaxDepth: indirectMaxDepth,
                    indirectTypes: indirectTypes,
                    cancellationToken: cancellationToken,
                    whereUsedCustom: null,
                    processed: null
                    );

                // Remove entries from UsedIndirectly that are included in UsedDirectly as they
                // should not appear in both.
                foreach (var o in custom.UsedDirectly.Objects)
                    if (o != null)
                        custom.UsedIndirectly.Objects.TryRemove(o.UID, out _);

                foreach (var o in custom.UsedDirectly.AccessControlRules)
                    if (o != null)
                        custom.UsedIndirectly.AccessControlRules.TryRemove(o.Rule.UID, out _);

                foreach (var o in custom.UsedDirectly.NatRules)
                    if (o != null)
                        custom.UsedIndirectly.NatRules.TryRemove(o.Rule.UID, out _);

                foreach (var o in custom.UsedDirectly.ThreatPreventionRules)
                    if (o != null)
                        custom.UsedIndirectly.ThreatPreventionRules.TryRemove(o.Rule.UID, out _);

                // Turn UsedIndirectly entries into arrays to create final result from
                var objects = new IObjectSummary[custom.UsedIndirectly.Objects.Count];
                custom.UsedIndirectly.Objects.Values.CopyTo(objects, 0);

                var accessControlRules = new WhereUsed.WhereUsedResults.Rules[custom.UsedIndirectly.AccessControlRules.Count];
                custom.UsedIndirectly.AccessControlRules.Values.CopyTo(accessControlRules, 0);

                var natRules = new WhereUsed.WhereUsedResults.NATs[custom.UsedIndirectly.NatRules.Count];
                custom.UsedIndirectly.NatRules.Values.CopyTo(natRules, 0);

                var threatPreventionRules = new WhereUsed.WhereUsedResults.ThreatRules[custom.UsedIndirectly.ThreatPreventionRules.Count];
                custom.UsedIndirectly.ThreatPreventionRules.Values.CopyTo(threatPreventionRules, 0);

                int total =
                    objects.Length +
                    accessControlRules.Length +
                    natRules.Length +
                    threatPreventionRules.Length;

                return new WhereUsed(
                    custom.UsedDirectly,
                    new WhereUsed.WhereUsedResults(
                        total: total,
                        objects: objects,
                        accessControlRules: accessControlRules,
                        natRules: natRules,
                        threatPreventionRules: threatPreventionRules
                        )
                );
            }
            else
                return await FindWhereUsed(
                    identifier: identifier,
                    objectConverter: null,
                    detailLevel: detailLevel,
                    indirect: false,
                    indirectMaxDepth: 0,
                    cancellationToken: cancellationToken
                    );
        }

        private async Task<WhereUsedCustom> FindWhereUsedCustomIndirect
            (
                string identifier,
                ObjectConverter objectConverter,
                DetailLevels detailLevel,
                int indirectMaxDepth,
                ObjectType[] indirectTypes,
                CancellationToken cancellationToken,
                WhereUsedCustom whereUsedCustom,
                List<string> processed
            )
        {
            if (objectConverter == null)
                objectConverter = new ObjectConverter(this, detailLevel, detailLevel);

            if (processed == null)
                processed = new List<string>() { identifier };
            else
                processed.Add(identifier);

            var whereUsed = await FindWhereUsed(
                identifier: identifier,
                objectConverter: null,
                detailLevel: detailLevel,
                indirect: false,
                indirectMaxDepth: 0,
                cancellationToken: cancellationToken
                );

            if (whereUsedCustom == null)
                whereUsedCustom = new WhereUsedCustom(whereUsed.UsedDirectly);

            foreach (var o in whereUsed.UsedDirectly.AccessControlRules)
                if (o != null)
                    whereUsedCustom.UsedIndirectly.AccessControlRules.TryAdd(o.Rule.UID, o);

            foreach (var o in whereUsed.UsedDirectly.NatRules)
                if (o != null)
                    whereUsedCustom.UsedIndirectly.NatRules.TryAdd(o.Rule.UID, o);

            foreach (var o in whereUsed.UsedDirectly.ThreatPreventionRules)
                if (o != null)
                    whereUsedCustom.UsedIndirectly.ThreatPreventionRules.TryAdd(o.Rule.UID, o);

            var tasks = new List<Task>();

            foreach (var o in whereUsed.UsedDirectly.Objects)
                if (
                    o != null &&
                    whereUsedCustom.UsedIndirectly.Objects.TryAdd(o.UID, o) &&
                    indirectMaxDepth > 0 &&
                    Array.IndexOf(indirectTypes, o.ObjectType) > -1
                    )
                {
                    tasks.Add(FindWhereUsedCustomIndirect(o.UID, objectConverter, detailLevel, indirectMaxDepth - 1, indirectTypes, cancellationToken, whereUsedCustom, processed));
                }

            await Task.WhenAll(tasks);

            return whereUsedCustom;
        }

        #endregion Methods

        #region Classes

        private class WhereUsedCustom
        {
            #region Constructors

            public WhereUsedCustom(WhereUsed.WhereUsedResults usedDirectly)
            {
                UsedDirectly = usedDirectly;
                UsedIndirectly = new WhereUsedResultsCustom();
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the direct usage results.
            /// </summary>
            /// <value>The direct usage results.</value>
            public WhereUsed.WhereUsedResults UsedDirectly { get; }

            /// <summary>
            /// Gets the indirect usage results.
            /// </summary>
            /// <value>The indirect usage results.</value>
            public WhereUsedResultsCustom UsedIndirectly { get; }

            #endregion Properties

            #region Classes

            /// <summary>
            /// Where Used Object Details
            /// </summary>
            public class WhereUsedResultsCustom
            {
                #region Properties

                /// <summary>
                /// Gets the access control rules.
                /// </summary>
                /// <value>The access control rules.</value>
                public ConcurrentDictionary<string, WhereUsed.WhereUsedResults.Rules> AccessControlRules { get; } = new ConcurrentDictionary<string, WhereUsed.WhereUsedResults.Rules>();

                /// <summary>
                /// Gets the nat rules.
                /// </summary>
                /// <value>The nat rules.</value>
                public ConcurrentDictionary<string, WhereUsed.WhereUsedResults.NATs> NatRules { get; } = new ConcurrentDictionary<string, WhereUsed.WhereUsedResults.NATs>();

                /// <summary>
                /// Gets the objects.
                /// </summary>
                /// <value>The objects.</value>
                public ConcurrentDictionary<string, IObjectSummary> Objects { get; } = new ConcurrentDictionary<string, IObjectSummary>();

                /// <summary>
                /// Gets the threat prevention rules.
                /// </summary>
                /// <value>The threat prevention rules.</value>
                public ConcurrentDictionary<string, WhereUsed.WhereUsedResults.ThreatRules> ThreatPreventionRules { get; } = new ConcurrentDictionary<string, WhereUsed.WhereUsedResults.ThreatRules>();

                #endregion Properties
            }

            #endregion Classes
        }

        #endregion Classes
    }
}