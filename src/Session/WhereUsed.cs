using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Searches for usage of the target object in other objects and rules.
        /// </summary>
        /// <param name="identifier">The object identifier to search for.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="indirect">if set to <c>true</c> results will include indirect uses.</param>
        /// <param name="indirectMaxDepth">The indirect maximum depth.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>WhereUsed object</returns>
        public Task<WhereUsed> FindWhereUsed
            (
                string identifier,
                DetailLevels detailLevel = DetailLevels.Standard,
                bool indirect = false,
                int indirectMaxDepth = 5,
                CancellationToken cancellationToken = default
            )
        {
            return FindWhereUsed(
                identifier: identifier,
                objectConverter: null,
                detailLevel: detailLevel,
                indirect: indirect,
                indirectMaxDepth: indirectMaxDepth,
                cancellationToken: cancellationToken
                );
        }

        private async Task<WhereUsed> FindWhereUsed
            (
                string identifier,
                ObjectConverter objectConverter,
                DetailLevels detailLevel,
                bool indirect,
                int indirectMaxDepth,
                CancellationToken cancellationToken
            )
        {
            var data = new JObject()
            {
                { identifier.IsUID() ? "uid" : "name", identifier },
                { "details-level", detailLevel.ToString() }
            };

            if (indirect)
            {
                data.Add("indirect", true);
                data.Add("indirect-max-depth", indirectMaxDepth);
            }

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("where-used", jsonData, cancellationToken);

            if (objectConverter == null)
                objectConverter = new ObjectConverter(this, detailLevel, detailLevel);

            var whereUsed = JsonConvert.DeserializeObject<WhereUsed>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            objectConverter.PostDeserilization(whereUsed.UsedDirectly?.Objects);
            objectConverter.PostDeserilization(whereUsed.UsedIndirectly?.Objects);

            return whereUsed;
        }

        #endregion Methods
    }
}