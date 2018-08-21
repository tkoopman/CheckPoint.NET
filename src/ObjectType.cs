namespace Koopman.CheckPoint
{
    public enum ObjectType
    {
        Unknown = 0x000000,

        #region Network Objects

        Host = 0x000001,
        Network = 0x000002,
        Group = 0x000003,
        AddressRange = 0x000004,
        MulticastAddressRange = 0x000005,
        GroupWithExclusion = 0x000006,
        SimpleGateway = 0x000007,
        SecurityZone = 0x000008,
        Time = 0x000009,
        TimeGroup = 0x00000A,
        AccessRole = 0x00000B,
        DynamicObject = 0x00000E,
        TrustedClient = 0x00000F,
        Tag = 0x000010,
        DNSDomain = 0x000011,
        OPSECApplication = 0x000012,

        #endregion Network Objects

        #region Services

        ServiceTCP = 0x000101,
        ServiceUDP = 0x000102,
        ServiceICMP = 0x000103,
        ServiceICMP6 = 0x000104,
        ServiceSCTP = 0x000105,
        ServiceOther = 0x000106,
        ServiceGroup = 0x000107,
        ServiceDceRpc = 0x000108,
        ServiceRPC = 0x000109,

        #endregion Services

        #region Applications

        ApplicationSite = 0x000201,
        ApplicationSiteCategory = 0x000202,
        ApplicationSiteGroup = 0x000203,

        #endregion Applications

        #region Access Control and NAT

        AccessRule = 0x000301,
        AccessSection = 0x000302,
        AccessLayer = 0x000303,
        NATRule = 0x000304,
        NATSection = 0x000305

        #endregion Access Control and NAT
    }
}