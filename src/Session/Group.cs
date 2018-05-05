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
        /// Deletes a group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken"></param>
        public Task DeleteGroup
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-group",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all groups that match filter.
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
        /// <returns>Array of Groups</returns>
        public Task<Group[]> FindAllGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<Group>
                (
                    Session: this,
                    Type: "group",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of Groups</returns>
        public Task<Group[]> FindAllGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<Group>
                (
                    Session: this,
                    Command: "show-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Group object</returns>
        public Task<Group> FindGroup
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<Group>
                (
                    Session: this,
                    Command: "show-group",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds groups that match filter.
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
        /// <returns>NetworkObjectsPagingResults of Groups</returns>
        public Task<NetworkObjectsPagingResults<Group>> FindGroups
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
            return Finds.Invoke<Group>
                (
                    Session: this,
                    Type: "group",
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
        /// Finds groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of Groups</returns>
        public Task<NetworkObjectsPagingResults<Group>> FindGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<Group>
                (
                    Session: this,
                    Command: "show-groups",
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