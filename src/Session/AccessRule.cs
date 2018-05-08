using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Finds the access rule by rule number.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="ruleNumber">The rule number.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the AccessRule
        /// </returns>
        public async Task<AccessRule> FindAccessRule(string layer, int ruleNumber, DetailLevels detailLevel = Find.Defaults.DetailLevel, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, dynamic>
            {
                { "layer", layer },
                { "rule-number", ruleNumber },
                { "details-level", detailLevel.ToString() }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-access-rule", jsonData, cancellationToken);

            var objectConverter = new ObjectConverter(this, DetailLevels.Full, detailLevel);

            var accessRule = JsonConvert.DeserializeObject<AccessRule>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            objectConverter.PostDeserilization(accessRule);

            return accessRule;
        }

        /// <summary>
        /// Finds the access rule base.
        /// </summary>
        /// <param name="value">The name or UID to layer to get rulebase of.</param>
        /// <param name="filter">
        /// Search expression to filter the rulebase. The provided text should be exactly the same as
        /// it would be given in Smart Console. The logical operators in the expression ('AND', 'OR')
        /// should be provided in capital letters.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The sort order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the AccessRulebasePagingResults
        /// </returns>
        public async Task<AccessRulebasePagingResults> FindAccessRulebase
            (
                string value,
                string filter = null,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { value.IsUID() ? "uid" : "name", value },
                { "filter", filter },
                { "use-object-dictionary", true },
                { "details-level", detailLevel.ToString() },
                { "limit", limit },
                { "offset", offset },
                { "order", (order == null)? null:new IOrder[] { order } }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-access-rulebase", jsonData, cancellationToken);

            var objectConverter = new ObjectConverter(this, DetailLevels.Full, detailLevel);

            var ruleBase = JsonConvert.DeserializeObject<AccessRulebasePagingResults>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            if (ruleBase != null)
            {
                objectConverter.PostDeserilization(ruleBase.Objects);
                objectConverter.PostDeserilization(ruleBase.Rulebase);

                var layer = new AccessLayer(this, DetailLevels.UID);
                layer.OnDeserializingMethod(default);
                if (value.IsUID())
                    layer.UID = value;
                else
                    layer.Name = value;
                layer.OnDeserializedMethod(default);

                foreach (var rule in ruleBase.Rulebase)
                    if (rule is AccessRule r)
                        r.Layer = layer;

                ruleBase.Next = delegate (CancellationToken ct)
                {
                    if (ruleBase.To == ruleBase.Total) return Task.FromResult((AccessRulebasePagingResults)null);
                    return FindAccessRulebase(value, filter, detailLevel, limit, ruleBase.To, order, ct);
                };
            }

            return ruleBase;
        }

        /// <summary>
        /// Finds the access rule base.
        /// </summary>
        /// <param name="value">The name or UID to layer to get rulebase of.</param>
        /// <param name="filter">
        /// Search expression to filter the rulebase. The provided text should be exactly the same as
        /// it would be given in Smart Console. The logical operators in the expression ('AND', 'OR')
        /// should be provided in capital letters.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The sort order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the AccessRulebasePagingResults
        /// </returns>
        public async Task<AccessRulebasePagingResults> FindAllAccessRulebase
            (
                string value,
                string filter = null,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            int offset = 0;
            var ruleBase = new AccessRulebasePagingResults
            {
                From = 1,
                Objects = new List<IObjectSummary>(),
                Rulebase = new List<IRulebaseEntry>()
            };
            var objectConverter = new ObjectConverter(this, DetailLevels.Full, detailLevel);

            while (true)
            {
                var data = new Dictionary<string, dynamic>
                {
                    { value.IsUID() ? "uid" : "name", value },
                    { "filter", filter },
                    { "use-object-dictionary", true },
                    { "details-level", detailLevel.ToString() },
                    { "limit", limit },
                    { "offset", offset },
                    { "order", (order == null)? null:new IOrder[] { order } }
                };

                string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

                string result = await PostAsync("show-access-rulebase", jsonData, cancellationToken);

                var rb = JsonConvert.DeserializeObject<AccessRulebasePagingResults>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

                ruleBase.Objects.AddRange(rb.Objects);
                ruleBase.Rulebase.AddRange(rb.Rulebase);
                ruleBase.To = rb.To;
                ruleBase.Total = rb.Total;
                ruleBase.UID = rb.UID;
                ruleBase.Name = rb.Name;

                if (ruleBase.To == ruleBase.Total) break;

                offset = rb.To;
            }
            if (ruleBase != null)
            {
                objectConverter.PostDeserilization(ruleBase.Objects);
                objectConverter.PostDeserilization(ruleBase.Rulebase);

                var layer = new AccessLayer(this, DetailLevels.UID);
                layer.OnDeserializingMethod(default);
                if (value.IsUID())
                    layer.UID = value;
                else
                    layer.Name = value;
                layer.OnDeserializedMethod(default);

                foreach (var rule in ruleBase.Rulebase)
                    if (rule is AccessRule r)
                        r.Layer = layer;
            }

            return ruleBase;
        }

        #endregion Methods
    }
}