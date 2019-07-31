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
        /// Deletes an access layer.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public Task DeleteAccessLayer
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-access-layer",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds an access layer.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// AccessLayer object
        /// </returns>
        public Task<AccessLayer> FindAccessLayer
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<AccessLayer>
                (
                    Session: this,
                    Command: "show-access-layer",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds access layers that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
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
        /// NetworkObjectsPagingResults of AccessLayers
        /// </returns>
        public Task<NetworkObjectsPagingResults<AccessLayer>> FindAccessLayers
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<AccessLayer>
                (
                    Session: this,
                    Type: "access-layer",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    ShowMembership: null,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds access layers.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the AccessLayersPagingResults
        /// </returns>
        public Task<AccessLayersPagingResults> FindAccessLayers
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<AccessLayer, AccessLayersPagingResults>
                (
                    Session: this,
                    Command: "show-access-layers",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    ShowMembership: null,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all access layers that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of AccessLayers
        /// </returns>
        public Task<AccessLayer[]> FindAllAccessLayers
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<AccessLayer>
                (
                    Session: this,
                    Type: "access-layer",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    ShowMembership: null,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all access layers.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of AccessLayers
        /// </returns>
        public Task<AccessLayer[]> FindAllAccessLayers
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<AccessLayer, AccessLayersPagingResults>
                (
                    Session: this,
                    Command: "show-access-layers",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    ShowMembership: null,
                    cancellationToken: cancellationToken
                );
        }

        #endregion Methods
    }
}