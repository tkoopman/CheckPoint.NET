﻿using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    public partial class Session
    {
        #region Methods

        /// <summary>
        /// Deletes a service group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public Task DeleteServiceGroup
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-group",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all service groups that match filter.
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
        /// A task that represents the asynchronous operation. The task result contains the Array of ServiceTCP
        /// </returns>
        public Task<ServiceGroup[]> FindAllServiceGroups
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
            return FindAll.Invoke<ServiceGroup>
                (
                    Session: this,
                    Type: "service-group",
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
        /// Finds all service groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="showMembership">
        /// Indicates whether to calculate and populate "groups" field for every object in reply.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of ServiceTCP
        /// </returns>
        public Task<ServiceGroup[]> FindAllServiceGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                bool showMembership = FindAll.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceGroup>
                (
                    Session: this,
                    Command: "show-service-groups",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    ShowMembership: showMembership,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a service group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// ServiceGroup object
        /// </returns>
        public Task<ServiceGroup> FindServiceGroup
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceGroup>
                (
                    Session: this,
                    Command: "show-service-group",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds service groups that match filter.
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
        /// NetworkObjectsPagingResults of ServiceTCP
        /// </returns>
        public Task<NetworkObjectsPagingResults<ServiceGroup>> FindServiceGroups
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
            return Finds.Invoke<ServiceGroup>
                (
                    Session: this,
                    Type: "service-group",
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
        /// Finds service groups.
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
        /// NetworkObjectsPagingResults of ServiceTCP
        /// </returns>
        public Task<NetworkObjectsPagingResults<ServiceGroup>> FindServiceGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                bool showMembership = Finds.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceGroup>
                (
                    Session: this,
                    Command: "show-service-groups",
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