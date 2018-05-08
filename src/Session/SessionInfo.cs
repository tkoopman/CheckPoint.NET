using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Finds all sessions.
        /// </summary>
        /// <param name="viewPublishedSessions">if set to <c>true</c> returns published sessions.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of SessionInfos
        /// </returns>
        public async Task<SessionInfo[]> FindAllSessions
            (
                bool viewPublishedSessions = false,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            int offset = 0;
            var sessions = new List<SessionInfo>();

            while (true)
            {
                var data = new Dictionary<string, dynamic>
                {
                    { "view-published-sessions", viewPublishedSessions },
                    { "limit", limit },
                    { "offset", offset },
                    { "order", (order == null)? null:new IOrder[] { order } },
                    { "details-level", DetailLevels.Full }
                };

                string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

                string result = await PostAsync("show-sessions", jsonData, cancellationToken);

                var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<SessionInfo>>(result);

                foreach (var o in results)
                    sessions.Add(o);

                if (results.To == results.Total)
                    return sessions.ToArray();

                offset = results.To;
            }
        }

        /// <summary>
        /// Finds a session.
        /// </summary>
        /// <param name="uid">The UID to find. <c>null</c> for current session information</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// SessionInfo object
        /// </returns>
        public async Task<SessionInfo> FindSession
            (
                string uid = null,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "uid", uid }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-session", jsonData, cancellationToken);

            return JsonConvert.DeserializeObject<SessionInfo>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, DetailLevels.Full) } });
        }

        /// <summary>
        /// Finds sessions.
        /// </summary>
        /// <param name="viewPublishedSessions">if set to <c>true</c> returns published sessions.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// NetworkObjectsPagingResults of SessionInfos
        /// </returns>
        public async Task<NetworkObjectsPagingResults<SessionInfo>> FindSessions
            (
                bool viewPublishedSessions = false,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "view-published-sessions", viewPublishedSessions },
                { "limit", limit },
                { "offset", offset },
                { "order", (order == null)? null:new IOrder[] { order } },
                { "details-level", DetailLevels.Full }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-sessions", jsonData, cancellationToken);

            var results = JsonConvert.DeserializeObject<NetworkObjectsPagingResults<SessionInfo>>(result);

            if (results != null)
            {
                results.Next = delegate (CancellationToken ct)
                {
                    if (results.To == results.Total) return Task.FromResult((NetworkObjectsPagingResults<SessionInfo>)null);
                    return FindSessions(viewPublishedSessions, limit, results.To, order, ct);
                };
            }

            return results;
        }

        /// <summary>
        /// Edit user's current session. All <c>null</c> values will not be changed.
        /// </summary>
        /// <param name="name">The session name.</param>
        /// <param name="description">The session description.</param>
        /// <param name="tags">The session tags.</param>
        /// <param name="color">The session color.</param>
        /// <param name="comments">The session comments.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Updated SessionInfo
        /// </returns>
        public async Task<SessionInfo> SetSessionInfo(string name = null, string description = null, string[] tags = null, Colors? color = null, string comments = null, Ignore ignore = Ignore.No, CancellationToken cancellationToken = default)
        {
            var data = new JObject()
            {
                { "new-name", name },
                { "description", description },
                { "comments", comments }
            };

            if (tags != null) data.Add("tags", new JArray(tags));
            if (color != null) data.Add("color", JToken.FromObject(color));

            data.AddIgnore(ignore);

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            string result = await PostAsync("set-session", jsonData, cancellationToken);

            return JsonConvert.DeserializeObject<SessionInfo>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, DetailLevels.Full) } });
        }

        /// <summary>
        /// Switch to another session.
        /// </summary>
        /// <param name="uid">The UID to switch to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// SessionInfo object
        /// </returns>
        public async Task<SessionInfo> SwitchSession
            (
                string uid,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "uid", uid }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("switch-session", jsonData, cancellationToken);

            var si = JsonConvert.DeserializeObject<SessionInfo>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, DetailLevels.Full) } });

            UID = si.UID;

            return si;
        }

        #endregion Methods
    }
}