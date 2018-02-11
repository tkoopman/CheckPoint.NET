using Koopman.CheckPoint.Json;
using Newtonsoft.Json;

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    [JsonConverter(typeof(EnumConverter), StringCases.Lowercase, " ")]
    public enum TopologyBehind
    {
        NotDefined,
        NetworkDefinedByTheInterfaceIPAndNetMask,
        NetworkDefinedByRouting,
        Specific
    }
}