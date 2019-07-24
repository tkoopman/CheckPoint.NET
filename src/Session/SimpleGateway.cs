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
        /// Deletes a simple gateway.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public Task DeleteSimpleGateway
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-simple-gateway",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all simple gateways that match filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="ipOnly">
        /// if set to <c>true</c> will search objects by their IP address only, without involving the
        /// textual search.
        /// </param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="showMembership">
        /// Indicates whether to calculate and populate "groups" field for every object in reply.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of SimpleGateways
        /// </returns>
        public Task<SimpleGateway[]> FindAllSimpleGateways
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                bool showMembership = FindAll.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<SimpleGateway>
                (
                    Session: this,
                    Type: "simple-gateway",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    ShowMembership: showMembership,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all simple gateways.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="showMembership">
        /// Indicates whether to calculate and populate "groups" field for every object in reply.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of SimpleGateways
        /// </returns>
        public Task<SimpleGateway[]> FindAllSimpleGateways
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                bool showMembership = FindAll.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateways",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    ShowMembership: showMembership,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a simple gateway.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// SimpleGateway object
        /// </returns>
        public Task<SimpleGateway> FindSimpleGateway
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateway",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds simple gateways that match filter.
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
        /// <param name="showMembership">
        /// Indicates whether to calculate and populate "groups" field for every object in reply.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// NetworkObjectsPagingResults of SimpleGateways
        /// </returns>
        public Task<NetworkObjectsPagingResults<SimpleGateway>> FindSimpleGateways
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                bool showMembership = Finds.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<SimpleGateway>
                (
                    Session: this,
                    Type: "simple-gateway",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    ShowMembership: showMembership,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds simple gateways.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="showMembership">
        /// Indicates whether to calculate and populate "groups" field for every object in reply.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// NetworkObjectsPagingResults of SimpleGateways
        /// </returns>
        public Task<NetworkObjectsPagingResults<SimpleGateway>> FindSimpleGateways
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                bool showMembership = Finds.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<SimpleGateway>
                (
                    Session: this,
                    Command: "show-simple-gateways",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    ShowMembership: showMembership,
                    cancellationToken: cancellationToken
                );
        }

        #endregion Methods
    }
}