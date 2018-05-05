// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// An active management session. This class handles all communications to the Management server.
    /// </summary>
    /// <example>
    /// <code>
    /// var session = new Session(
    ///     managementServer: "192.168.1.1",
    ///     userName: "admin",
    ///     password: "***",
    ///     certificateValidation: false
    /// );
    /// </code>
    /// </example>
    /// <seealso cref="System.IDisposable" />
    public partial class Session : IDisposable
    {
        #region Service Methods

        #region ServiceDceRpc Methods

        /// <summary>
        /// Deletes a DCE-RPC service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceDceRpc
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-dce-rpc",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all DCE-RPC services that match filter.
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
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceDceRpc[]> FindAllServicesDceRpc
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Type: "service-dce-rpc",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all DCE-RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceDceRpc[]> FindAllServicesDceRpc
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Command: "show-services-dce-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a DCE-RPC service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceDceRpc object</returns>
        public Task<ServiceDceRpc> FindServiceDceRpc
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Command: "show-service-dce-rpc",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds DCE-RPC services that match filter.
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
        /// <returns>NetworkObjectsPagingResults of ServiceDceRpc</returns>
        public Task<NetworkObjectsPagingResults<ServiceDceRpc>> FindServicesDceRpc
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
            return Finds.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Type: "service-dce-rpc",
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
        /// Finds DCE-RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceDceRpc</returns>
        public Task<NetworkObjectsPagingResults<ServiceDceRpc>> FindServicesDceRpc
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceDceRpc>
                (
                    Session: this,
                    Command: "show-services-dce-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceDceRpc Methods

        #region ICMP Methods

        /// <summary>
        /// Deletes a service-icmp.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceICMP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-icmp",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all service-icmps that match filter.
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
        /// <returns>Array of Objects</returns>
        public Task<ServiceICMP[]> FindAllServicesICMP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceICMP>
                (
                    Session: this,
                    Type: "service-icmp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all services ICMP.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of Objects</returns>
        public Task<ServiceICMP[]> FindAllServicesICMP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceICMP>
                (
                    Session: this,
                    Command: "show-services-icmp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a service-icmp.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Object object</returns>
        public Task<ServiceICMP> FindServiceICMP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceICMP>
                (
                    Session: this,
                    Command: "show-service-icmp",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds service-icmps that match filter.
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
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public Task<NetworkObjectsPagingResults<ServiceICMP>> FindServicesICMP
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
            return Finds.Invoke<ServiceICMP>
                (
                    Session: this,
                    Type: "service-icmp",
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
        /// Finds services ICMP.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public Task<NetworkObjectsPagingResults<ServiceICMP>> FindServicesICMP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceICMP>
                (
                    Session: this,
                    Command: "show-services-icmp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ICMP Methods

        #region ICMP6 Methods

        /// <summary>
        /// Deletes a service-icmp6.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceICMP6
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-icmp6",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all service-icmp6s that match filter.
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
        /// <returns>Array of Objects</returns>
        public Task<ServiceICMP6[]> FindAllServicesICMP6
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Type: "service-icmp6",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all services ICMP6.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of Objects</returns>
        public Task<ServiceICMP6[]> FindAllServicesICMP6
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Command: "show-services-icmp6",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a service-icmp6.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Object object</returns>
        public Task<ServiceICMP6> FindServiceICMP6
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Command: "show-service-icmp6",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds service-icmp6s that match filter.
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
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public Task<NetworkObjectsPagingResults<ServiceICMP6>> FindServicesICMP6
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
            return Finds.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Type: "service-icmp6",
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
        /// Finds services ICMP6.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of Objects</returns>
        public Task<NetworkObjectsPagingResults<ServiceICMP6>> FindServicesICMP6
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceICMP6>
                (
                    Session: this,
                    Command: "show-services-icmp6",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ICMP6 Methods

        #region ServiceOther Methods

        /// <summary>
        /// Deletes a other service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceOther
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-other",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all other services that match filter.
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
        /// <returns>Array of ServiceOther</returns>
        public Task<ServiceOther[]> FindAllServicesOther
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceOther>
                (
                    Session: this,
                    Type: "service-other",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all other services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceOther</returns>
        public Task<ServiceOther[]> FindAllServicesOther
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceOther>
                (
                    Session: this,
                    Command: "show-services-other",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a other service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceOther object</returns>
        public Task<ServiceOther> FindServiceOther
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceOther>
                (
                    Session: this,
                    Command: "show-service-other",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds other services that match filter.
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
        /// <returns>NetworkObjectsPagingResults of ServiceOther</returns>
        public Task<NetworkObjectsPagingResults<ServiceOther>> FindServicesOther
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
            return Finds.Invoke<ServiceOther>
                (
                    Session: this,
                    Type: "service-other",
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
        /// Finds other services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceOther</returns>
        public Task<NetworkObjectsPagingResults<ServiceOther>> FindServicesOther
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceOther>
                (
                    Session: this,
                    Command: "show-services-other",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceOther Methods

        #region ServiceRPC Methods

        /// <summary>
        /// Deletes a RPC service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceRPC
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-rpc",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all RPC services that match filter.
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
        /// <returns>Array of ServiceRPC</returns>
        public Task<ServiceRPC[]> FindAllServicesRPC
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceRPC>
                (
                    Session: this,
                    Type: "service-rpc",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceRPC</returns>
        public Task<ServiceRPC[]> FindAllServicesRPC
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceRPC>
                (
                    Session: this,
                    Command: "show-services-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a RPC service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceRPC object</returns>
        public Task<ServiceRPC> FindServiceRPC
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceRPC>
                (
                    Session: this,
                    Command: "show-service-rpc",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds RPC services that match filter.
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
        /// <returns>NetworkObjectsPagingResults of ServiceRPC</returns>
        public Task<NetworkObjectsPagingResults<ServiceRPC>> FindServicesRPC
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
            return Finds.Invoke<ServiceRPC>
                (
                    Session: this,
                    Type: "service-rpc",
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
        /// Finds RPC services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceRPC</returns>
        public Task<NetworkObjectsPagingResults<ServiceRPC>> FindServicesRPC
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceRPC>
                (
                    Session: this,
                    Command: "show-services-rpc",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceRPC Methods

        #region ServiceSCTP Methods

        /// <summary>
        /// Deletes a SCTP service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceSCTP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-sctp",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all SCTP services that match filter.
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
        /// <returns>Array of ServiceSCTP</returns>
        public Task<ServiceSCTP[]> FindAllServicesSCTP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Type: "service-sctp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all SCTP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceSCTP</returns>
        public Task<ServiceSCTP[]> FindAllServicesSCTP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Command: "show-services-sctp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a SCTP service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceSCTP object</returns>
        public Task<ServiceSCTP> FindServiceSCTP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Command: "show-service-sctp",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds SCTP services that match filter.
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
        /// <returns>NetworkObjectsPagingResults of ServiceSCTP</returns>
        public Task<NetworkObjectsPagingResults<ServiceSCTP>> FindServicesSCTP
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
            return Finds.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Type: "service-sctp",
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
        /// Finds SCTP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceSCTP</returns>
        public Task<NetworkObjectsPagingResults<ServiceSCTP>> FindServicesSCTP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceSCTP>
                (
                    Session: this,
                    Command: "show-services-sctp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceSCTP Methods

        #region ServiceTCP Methods

        /// <summary>
        /// Deletes a TCP service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceTCP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-tcp",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all TCP services that match filter.
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
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceTCP[]> FindAllServicesTCP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceTCP>
                (
                    Session: this,
                    Type: "service-tcp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all TCP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceTCP[]> FindAllServicesTCP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceTCP>
                (
                    Session: this,
                    Command: "show-services-tcp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds TCP services that match filter.
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
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public Task<NetworkObjectsPagingResults<ServiceTCP>> FindServicesTCP
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
            return Finds.Invoke<ServiceTCP>
                (
                    Session: this,
                    Type: "service-tcp",
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
        /// Finds TCP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public Task<NetworkObjectsPagingResults<ServiceTCP>> FindServicesTCP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceTCP>
                (
                    Session: this,
                    Command: "show-services-tcp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a TCP service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceTCP object</returns>
        public Task<ServiceTCP> FindServiceTCP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceTCP>
                (
                    Session: this,
                    Command: "show-service-tcp",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceTCP Methods

        #region ServiceUDP Methods

        /// <summary>
        /// Deletes a UDP service.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task DeleteServiceUDP
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-service-udp",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all UDP services that match filter.
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
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceUDP[]> FindAllServicesUDP
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceUDP>
                (
                    Session: this,
                    Type: "service-udp",
                    Filter: filter,
                    IPOnly: ipOnly,
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all UDP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceUDP[]> FindAllServicesUDP
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ServiceUDP>
                (
                    Session: this,
                    Command: "show-services-udp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds UDP services that match filter.
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
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public Task<NetworkObjectsPagingResults<ServiceUDP>> FindServicesUDP
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
            return Finds.Invoke<ServiceUDP>
                (
                    Session: this,
                    Type: "service-udp",
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
        /// Finds UDP services.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public Task<NetworkObjectsPagingResults<ServiceUDP>> FindServicesUDP
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ServiceUDP>
                (
                    Session: this,
                    Command: "show-services-udp",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Offset: offset,
                    Order: order,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a UDP service.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceUDP object</returns>
        public Task<ServiceUDP> FindServiceUDP
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ServiceUDP>
                (
                    Session: this,
                    Command: "show-service-udp",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceUDP Methods

        #region ServiceGroup Methods

        /// <summary>
        /// Deletes a service group.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceGroup[]> FindAllServiceGroups
            (
                string filter,
                bool ipOnly = FindAll.Defaults.IPOnly,
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
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
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all service groups.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of ServiceTCP</returns>
        public Task<ServiceGroup[]> FindAllServiceGroups
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
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
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds a service group.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>ServiceGroup object</returns>
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public Task<NetworkObjectsPagingResults<ServiceGroup>> FindServiceGroups
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of ServiceTCP</returns>
        public Task<NetworkObjectsPagingResults<ServiceGroup>> FindServiceGroups
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
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
                    cancellationToken: cancellationToken
                );
        }

        #endregion ServiceGroup Methods

        #endregion Service Methods

        #region Access Control and NAT Methods

        #region Access Layer Methods

        /// <summary>
        /// Deletes an access layer.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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
        /// <returns>AccessLayer object</returns>
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
        /// <returns>NetworkObjectsPagingResults of AccessLayers</returns>
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
        /// <returns>AccessLayersPagingResults</returns>
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
        /// <returns>Array of AccessLayer</returns>
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
        /// <returns>Array of AccessLayer</returns>
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
                    cancellationToken: cancellationToken
                );
        }

        #endregion Access Layer Methods

        #region Access Rule Methods

        /// <summary>
        /// Finds the access rule by rule number.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="ruleNumber">The rule number.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>AccessRule</returns>
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
        /// <returns>AccessRulebasePagingResults</returns>
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
        /// <returns>AccessRulebasePagingResults</returns>
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

        #endregion Access Rule Methods

        #endregion Access Control and NAT Methods

        #region Misc. Methods

        #region Task Methods

        /// <summary>
        /// Find a task.
        /// </summary>
        /// <param name="taskID">The task identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task</returns>
        public async Task<CheckPointTask> FindTask
            (
                string taskID,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "task-id", taskID },
                { "details-level", DetailLevels.Full }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-task", jsonData, cancellationToken);

            var results = JsonConvert.DeserializeObject<JObject>(result);
            var array = (JArray)results.GetValue("tasks");

            var tasks = JsonConvert.DeserializeObject<CheckPointTask[]>(array.ToString(), new JsonSerializerSettings() { Converters = { new SessionConstructorConverter(this) } });

            return tasks?[0];
        }

        /// <summary>
        /// Runs the script on multiple targets.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The script arguments.</param>
        /// <param name="targets">The targets.</param>
        /// <param name="comments">Script comments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A read-only dictionary detailing the task ID for each target.</returns>
        public async Task<IReadOnlyDictionary<string, string>> RunScript
            (
                string scriptName,
                string script,
                string args,
                string[] targets,
                string comments = null,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "script-name", scriptName },
                { "script", script },
                { "args", args },
                { "targets", targets },
                { "comments", comments }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("run-script", jsonData, cancellationToken);

            var results = JsonConvert.DeserializeObject<JObject>(result);
            var array = (JArray)results.GetValue("tasks");

            var dicResults = new Dictionary<string, string>();

            foreach (var r in array)
            {
                var j = r as JObject;
                dicResults.Add(j.GetValue("target").ToString(), j.GetValue("task-id").ToString());
            }

            return new ReadOnlyDictionary<string, string>(dicResults);
        }

        /// <summary>
        /// Runs the script on a single target.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="target">The target.</param>
        /// <param name="comments">The script comments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task ID</returns>
        public async Task<string> RunScript
            (
                string scriptName,
                string script,
                string args,
                string target,
                string comments = null,
                CancellationToken cancellationToken = default
            )
        {
            var results = await RunScript(
                    scriptName,
                    script,
                    args,
                    new string[] { target },
                    comments,
                    cancellationToken
                );

            return results?.Values.First();
        }

        #endregion Task Methods

        #region Policy Methods

        /// <summary>
        /// Installs the policy to gateways.
        /// </summary>
        /// <param name="policy">The policy to install.</param>
        /// <param name="targets">The target gateways.</param>
        /// <param name="access">if set to <c>true</c> installs the access policy.</param>
        /// <param name="threatPrevention">
        /// if set to <c>true</c> installs the threat prevention policy.
        /// </param>
        /// <param name="installOnAllClusterMembersOrFail">
        /// if set to <c>true</c> will fail if it cannot install policy to all cluster members. if
        /// set to <c>false</c> can complete with partial success if not all cluster members available.
        /// </param>
        /// <param name="prepareOnly">if set to <c>true</c> will prepare only.</param>
        /// <param name="revision">The revision of the policy to install.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task ID</returns>
        public async Task<string> InstallPolicy
            (
                string policy,
                string[] targets,
                bool access,
                bool threatPrevention,
                bool installOnAllClusterMembersOrFail = true,
                bool prepareOnly = false,
                string revision = null,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy },
                { "targets", targets },
                { "access", access },
                { "threat-prevention", threatPrevention },
                { "install-on-all-cluster-members-or-fail", installOnAllClusterMembersOrFail },
                { "prepare-only", prepareOnly },
                { "revision", revision }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("install-policy", jsonData, cancellationToken);

            var taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        /// <summary>
        /// Verifies the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task ID</returns>
        public async Task<string> VerifyPolicy
            (
                string policy,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "policy-package", policy }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("verify-policy", jsonData, cancellationToken);

            var taskID = JsonConvert.DeserializeObject<JObject>(result);

            return taskID.GetValue("task-id")?.ToString();
        }

        #endregion Policy Methods

        #region WhereUsed Methods

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

        #endregion WhereUsed Methods

        #region Unused Methods

        /// <summary>
        /// Searches for unusage objects.
        /// </summary>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Array of IObjectSummary objects</returns>
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
        /// Searches for unusage objects.
        /// </summary>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>NetworkObjectsPagingResults of IObjectSummary objects</returns>
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

        #endregion Unused Methods

        #endregion Misc. Methods
    }
}