using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
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
        /// <returns>Array of IObjectSummary</returns>
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
        /// Finds an object by UID.
        /// </summary>
        /// <param name="uid">The UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IObjectSummary object</returns>
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
        /// <returns>NetworkObjectsPagingResults of IObjectSummary</returns>
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

        #endregion Methods
    }
}