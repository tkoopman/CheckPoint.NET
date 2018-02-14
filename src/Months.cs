using System.Runtime.Serialization;

namespace Koopman.CheckPoint
{
    public enum Months
    {
        [EnumMember(Value = "1")]
        January = 1,

        [EnumMember(Value = "2")]
        February = 2,

        [EnumMember(Value = "3")]
        March = 4,

        [EnumMember(Value = "4")]
        April = 8,

        [EnumMember(Value = "5")]
        May = 16,

        [EnumMember(Value = "6")]
        June = 32,

        [EnumMember(Value = "7")]
        July = 64,

        [EnumMember(Value = "8")]
        August = 128,

        [EnumMember(Value = "9")]
        September = 256,

        [EnumMember(Value = "10")]
        October = 512,

        [EnumMember(Value = "11")]
        November = 1024,

        [EnumMember(Value = "12")]
        December = 2048,

        [EnumMember(Value = "Any")]
        Any = 4095
    }
}