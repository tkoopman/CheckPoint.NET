using Koopman.CheckPoint.Json;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.IA
{
    /// <summary>
    /// IA Client Types
    /// </summary>
    [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, "-")]
    public enum ClientTypes
    {
        /// <summary>
        /// Any
        /// </summary>
        Any,

        /// <summary>
        /// Captive portal
        /// </summary>
        CaptivePortal,

        /// <summary>
        /// IDA agent
        /// </summary>
        IdaAgent,

        /// <summary>
        /// VPN
        /// </summary>
        VPN,

        /// <summary>
        /// AD query
        /// </summary>
        AdQuery,

        /// <summary>
        /// Multihost agent
        /// </summary>
        MultihostAgent,

        /// <summary>
        /// Radius
        /// </summary>
        Radius,

        /// <summary>
        /// IDA API
        /// </summary>
        IdaApi,

        /// <summary>
        /// Identity collector
        /// </summary>
        IdentityCollector
    }
}