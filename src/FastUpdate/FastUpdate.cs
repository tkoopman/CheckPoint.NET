using Koopman.CheckPoint.Internal;

namespace Koopman.CheckPoint.FastUpdate
{
    /// <summary>
    /// Allows for editing existing objects without finding them first.
    /// </summary>
    public static class FastUpdate
    {
        #region Methods

        /// <summary>
        /// Updates the Access Layer without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Access Layer to update.</param>
        /// <returns>
        /// AccessLayer object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static AccessLayer UpdateAccessLayer(this Session session, string identifier) => AddIdentifier(identifier, new AccessLayer(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Address Range without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Address Range to update.</param>
        /// <returns>
        /// AddressRange object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static AddressRange UpdateAddressRange(this Session session, string identifier) => AddIdentifier(identifier, new AddressRange(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Application Category without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">
        /// The identifier, can by name or UID of Application Category to update.
        /// </param>
        /// <returns>
        /// ApplicationCategory object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ApplicationCategory UpdateApplicationCategory(this Session session, string identifier) => AddIdentifier(identifier, new ApplicationCategory(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Application Group without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">
        /// The identifier, can by name or UID of Application Group to update.
        /// </param>
        /// <returns>
        /// ApplicationGroup object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ApplicationGroup UpdateApplicationGroup(this Session session, string identifier) => AddIdentifier(identifier, new ApplicationGroup(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Application Site without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">
        /// The identifier, can by name or UID of Application Site to update.
        /// </param>
        /// <returns>
        /// ApplicationSite object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ApplicationSite UpdateApplicationSite(this Session session, string identifier) => AddIdentifier(identifier, new ApplicationSite(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Group without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Group to update.</param>
        /// <returns>
        /// Group object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static Group UpdateGroup(this Session session, string identifier) => AddIdentifier(identifier, new Group(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Group With Exclusion without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">
        /// The identifier, can by name or UID of Group With Exclusion to update.
        /// </param>
        /// <returns>
        /// GroupWithExclusion object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static GroupWithExclusion UpdateGroupWithExclusion(this Session session, string identifier) => AddIdentifier(identifier, new GroupWithExclusion(session, DetailLevels.Full));

        /// <summary>
        /// Updates the host without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of host to update.</param>
        /// <returns>
        /// Host object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static Host UpdateHost(this Session session, string identifier) => AddIdentifier(identifier, new Host(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Multicast Address Range without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">
        /// The identifier, can by name or UID of Multicast Address Range to update.
        /// </param>
        /// <returns>
        /// MulticastAddressRange object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static MulticastAddressRange UpdateMulticastAddressRange(this Session session, string identifier) => AddIdentifier(identifier, new MulticastAddressRange(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Network without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Network to update.</param>
        /// <returns>
        /// Network object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static Network UpdateNetwork(this Session session, string identifier) => AddIdentifier(identifier, new Network(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Security Zone without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Security Zone to update.</param>
        /// <returns>
        /// SecurityZone object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static SecurityZone UpdateSecurityZone(this Session session, string identifier) => AddIdentifier(identifier, new SecurityZone(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service DCE-RPC without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service DCE-RPC to update.</param>
        /// <returns>
        /// ServiceDceRpc object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceDceRpc UpdateServiceDceRpc(this Session session, string identifier) => AddIdentifier(identifier, new ServiceDceRpc(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service Group without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service Group to update.</param>
        /// <returns>
        /// ServiceGroup object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceGroup UpdateServiceGroup(this Session session, string identifier) => AddIdentifier(identifier, new ServiceGroup(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service ICMP without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service ICMP to update.</param>
        /// <returns>
        /// ServiceICMP object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceICMP UpdateServiceICMP(this Session session, string identifier) => AddIdentifier(identifier, new ServiceICMP(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service ICMP6 without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service ICMP6 to update.</param>
        /// <returns>
        /// ServiceICMP6 object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceICMP6 UpdateServiceICMP6(this Session session, string identifier) => AddIdentifier(identifier, new ServiceICMP6(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service Other without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service Other to update.</param>
        /// <returns>
        /// ServiceOther object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceOther UpdateServiceOther(this Session session, string identifier) => AddIdentifier(identifier, new ServiceOther(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service RPC without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service RPC to update.</param>
        /// <returns>
        /// ServiceRPC object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceRPC UpdateServiceRPC(this Session session, string identifier) => AddIdentifier(identifier, new ServiceRPC(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service SCTP without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service SCTP to update.</param>
        /// <returns>
        /// ServiceSCTP object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceSCTP UpdateServiceSCTP(this Session session, string identifier) => AddIdentifier(identifier, new ServiceSCTP(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service TCP without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service TCP to update.</param>
        /// <returns>
        /// ServiceTCP object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceTCP UpdateServiceTCP(this Session session, string identifier) => AddIdentifier(identifier, new ServiceTCP(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Service UDP without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Service UDP to update.</param>
        /// <returns>
        /// ServiceUDP object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static ServiceUDP UpdateServiceUDP(this Session session, string identifier) => AddIdentifier(identifier, new ServiceUDP(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Simple Gateway without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Simple Gateway to update.</param>
        /// <returns>
        /// SimpleGateway object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static SimpleGateway UpdateSimpleGateway(this Session session, string identifier) => AddIdentifier(identifier, new SimpleGateway(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Tag without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Tag to update.</param>
        /// <returns>
        /// Tag object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static Tag UpdateTag(this Session session, string identifier) => AddIdentifier(identifier, new Tag(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Time without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Time to update.</param>
        /// <returns>
        /// Time object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static Time UpdateTime(this Session session, string identifier) => AddIdentifier(identifier, new Time(session, DetailLevels.Full));

        /// <summary>
        /// Updates the Time Group without finding it first.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="identifier">The identifier, can by name or UID of Time Group to update.</param>
        /// <returns>
        /// TimeGroup object ready for you to set properties to change and then use
        /// <see cref="ObjectSummary{T}.AcceptChanges()" /> to send set request.
        /// </returns>
        public static TimeGroup UpdateTimeGroup(this Session session, string identifier) => AddIdentifier(identifier, new TimeGroup(session, DetailLevels.Full));

        private static T AddIdentifier<T>(string identifier, T obj) where T : ObjectSummary<T>
        {
            obj.OnDeserializingMethod(default);
            if (identifier.IsUID())
                obj.UID = identifier;
            else
                obj.Name = identifier;
            obj.OnDeserializedMethod(default);

            return obj;
        }

        #endregion Methods
    }
}