using System.Runtime.Serialization;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// List of months
    /// </summary>
    public enum Months
    {
        /// <summary>
        /// January
        /// </summary>
        [EnumMember(Value = "1")]
        January = 1,

        /// <summary>
        /// February
        /// </summary>
        [EnumMember(Value = "2")]
        February = 2,

        /// <summary>
        /// March
        /// </summary>
        [EnumMember(Value = "3")]
        March = 4,

        /// <summary>
        /// April
        /// </summary>
        [EnumMember(Value = "4")]
        April = 8,

        /// <summary>
        /// May
        /// </summary>
        [EnumMember(Value = "5")]
        May = 16,

        /// <summary>
        /// June
        /// </summary>
        [EnumMember(Value = "6")]
        June = 32,

        /// <summary>
        /// July
        /// </summary>
        [EnumMember(Value = "7")]
        July = 64,

        /// <summary>
        /// August
        /// </summary>
        [EnumMember(Value = "8")]
        August = 128,

        /// <summary>
        /// September
        /// </summary>
        [EnumMember(Value = "9")]
        September = 256,

        /// <summary>
        /// October
        /// </summary>
        [EnumMember(Value = "10")]
        October = 512,

        /// <summary>
        /// November
        /// </summary>
        [EnumMember(Value = "11")]
        November = 1024,

        /// <summary>
        /// December
        /// </summary>
        [EnumMember(Value = "12")]
        December = 2048,

        /// <summary>
        /// Run every month
        /// </summary>
        [EnumMember(Value = "Any")]
        Any = 4095
    }
}