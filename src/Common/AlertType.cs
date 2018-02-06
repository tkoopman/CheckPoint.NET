using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koopman.CheckPoint.Common
{
    [JsonConverter(typeof(EnumConverter), StringCases.Lowercase, " ", ".")]
    public enum AlertType
    {
        None,
        Log,
        PopupAlert,
        MailAlert,
        SnmpTrapAlert,
        UserDefinedAlertNo1,
        UserDefinedAlertNo2,
        UserDefinedAlertNo3
    }
}