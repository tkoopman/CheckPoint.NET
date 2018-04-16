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