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
        /// Finds all objects that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="type">The object type.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of IObjectSummary
        /// </returns>
        public Task<IObjectSummary[]> FindAllObjects
            (
                string filter = null,
                string type = null,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<IObjectSummary>
                (
                    Session: this,
                    Type: type,
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Searches for unusage objects.
        /// </summary>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of
        /// IObjectSummary objects
        /// </returns>
        public async Task<IObjectSummary[]> FindAllUnusedObjects
            (
                DetailLevels detailLevel = DetailLevels.Standard,
                int limit = 50,
                IOrder order = null,
                CancellationToken cancellationToken = default
            )
        {
            int offset = 0;
            var objectConverter = new ObjectConverter(this, detailLevel, detailLevel);
            var objs = new List<IObjectSummary>();

            while (true)
            {
                var data = new Dictionary<string, dynamic>
                {
                    { "details-level", detailLevel.ToString() },
                    { "limit", limit },
                    { "offset", offset },
                    { "order", (order == null)? null:new IOrder[] { order } }
                };

                string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

                string result = await PostAsync("show-unused-objects", jsonData, cancellationToken);

                var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<IObjectSummary>>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

                foreach (var o in results)
                    objs.Add(o);

                if (results.To == results.Total)
                {
                    objectConverter.PostDeserilization(objs);
                    return objs.ToArray();
                }

                offset = results.To;
            }
        }

        /// <summary>
        /// Finds an object by UID.
        /// </summary>
        /// <param name="uid">The UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// IObjectSummary object
        /// </returns>
        public Task<IObjectSummary> FindObject
            (
                string uid,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.InvokeAsync
                (
                    Session: this,
                    uid: uid,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds objects that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="type">The object type.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// NetworkObjectsPagingResults of IObjectSummary
        /// </returns>
        public Task<NetworkObjectsPagingResults<IObjectSummary>> FindObjects
            (
                string filter = null,
                string type = null,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<IObjectSummary>
                (
                    Session: this,
                    Type: type,
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Searches for unusage objects.
        /// </summary>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// NetworkObjectsPagingResults of IObjectSummary objects
        /// </returns>
        public async Task<NetworkObjectsPagingResults<IObjectSummary>> FindUnusedObjects
            (
                DetailLevels detailLevel = DetailLevels.Standard,
                int limit = 50,
                int offset = 0,
                IOrder order = null,
                CancellationToken cancellationToken = default
            )
        {
            var objectConverter = new ObjectConverter(this, detailLevel, detailLevel);

            var data = new Dictionary<string, dynamic>
            {
                { "details-level", detailLevel.ToString() },
                { "limit", limit },
                { "offset", offset },
                { "order", (order == null)? null:new IOrder[] { order } }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-unused-objects", jsonData, cancellationToken);

            var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<IObjectSummary>>(result, new JsonSerializerSettings() { Converters = { objectConverter } });

            if (results != null)
            {
                objectConverter.PostDeserilization(results);
                results.Next = delegate (CancellationToken ct)
                {
                    if (results.To == results.Total) return Task.FromResult((NetworkObjectsPagingResults<IObjectSummary>)null);
                    return FindUnusedObjects(detailLevel, limit, results.To, order, ct);
                };
            }

            return results;
        }

        #endregion Methods
    }
}