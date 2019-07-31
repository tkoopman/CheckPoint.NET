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
        /// Deletes an application site.
        /// </summary>
        /// <param name="value">The name or UID to delete.</param>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public Task DeleteApplicationSite
            (
                string value,
                Ignore ignore = Delete.Defaults.ignore,
                CancellationToken cancellationToken = default
            )
        {
            return Delete.Invoke
                (
                    Session: this,
                    Command: "delete-application-site",
                    Value: value,
                    Ignore: ignore,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds all application sites that match filter.
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
        /// A task that represents the asynchronous operation. The task result contains the Array of ApplicationSite
        /// </returns>
        public Task<ApplicationSite[]> FindAllApplicationSites
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
            return FindAll.Invoke<ApplicationSite>
                (
                    Session: this,
                    Type: "application-site",
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
        /// Finds all application sites.
        /// </summary>
        /// <param name="detailLevel">The detail level to return.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="order">The order.</param>
        /// <param name="showMembership">
        /// Indicates whether to calculate and populate "groups" field for every object in reply.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the Array of ApplicationSite
        /// </returns>
        public Task<ApplicationSite[]> FindAllApplicationSites
            (
                DetailLevels detailLevel = FindAll.Defaults.DetailLevel,
                int limit = FindAll.Defaults.Limit,
                IOrder order = FindAll.Defaults.Order,
                bool showMembership = FindAll.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return FindAll.Invoke<ApplicationSite>
                (
                    Session: this,
                    Command: "show-application-sites",
                    DetailLevel: detailLevel,
                    Limit: limit,
                    Order: order,
                    ShowMembership: showMembership,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds an application site.
        /// </summary>
        /// <param name="value">The name or UID to find.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// ApplicationSite object
        /// </returns>
        public Task<ApplicationSite> FindApplicationSite
            (
                string value,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            return Find.Invoke<ApplicationSite>
                (
                    Session: this,
                    Command: "show-application-site",
                    Value: value,
                    DetailLevel: detailLevel,
                    cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Finds an application site.
        /// </summary>
        /// <param name="applicationID">The application identifier.</param>
        /// <param name="detailLevel">The detail level of child objects to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// ApplicationSite object
        /// </returns>
        public async Task<ApplicationSite> FindApplicationSite
            (
                int applicationID,
                DetailLevels detailLevel = Find.Defaults.DetailLevel,
                CancellationToken cancellationToken = default
            )
        {
            var data = new Dictionary<string, dynamic>
            {
                { "application-id", applicationID },
                { "details-level", detailLevel.ToString() }
            };

            string jsonData = JsonConvert.SerializeObject(data, JsonFormatting);

            string result = await PostAsync("show-application-site", jsonData, cancellationToken);

            return JsonConvert.DeserializeObject<ApplicationSite>(result, new JsonSerializerSettings() { Converters = { new ObjectConverter(this, DetailLevels.Full, detailLevel) } });
        }

        /// <summary>
        /// Finds application sites that match filter.
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
        /// NetworkObjectsPagingResults of ApplicationSite
        /// </returns>
        public Task<NetworkObjectsPagingResults<ApplicationSite>> FindApplicationSites
            (
                string filter,
                bool ipOnly = Finds.Defaults.IPOnly,
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                bool showMembership = FindAll.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ApplicationSite>
                (
                    Session: this,
                    Type: "application-site",
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
        /// Finds application sites.
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
        /// NetworkObjectsPagingResults of ApplicationSite
        /// </returns>
        public Task<NetworkObjectsPagingResults<ApplicationSite>> FindApplicationSites
            (
                DetailLevels detailLevel = Finds.Defaults.DetailLevel,
                int limit = Finds.Defaults.Limit,
                int offset = Finds.Defaults.Offset,
                IOrder order = Finds.Defaults.Order,
                bool showMembership = Finds.Defaults.ShowMembership,
                CancellationToken cancellationToken = default
            )
        {
            return Finds.Invoke<ApplicationSite>
                (
                    Session: this,
                    Command: "show-application-sites",
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