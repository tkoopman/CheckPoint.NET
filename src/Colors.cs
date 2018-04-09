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

using Koopman.CheckPoint.Json;
using Newtonsoft.Json;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// List of valid Check Point object colors
    /// </summary>
    [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase, " ")]
    public enum Colors
    {
        /// <summary>
        /// Aquamarine
        /// </summary>
        Aquamarine,

        /// <summary>
        /// Black
        /// </summary>
        Black,

        /// <summary>
        /// Blue
        /// </summary>
        Blue,

        /// <summary>
        /// Brown
        /// </summary>
        Brown,

        /// <summary>
        /// Burlywood
        /// </summary>
        Burlywood,

        /// <summary>
        /// Coral
        /// </summary>
        Coral,

        /// <summary>
        /// Crete Blue
        /// </summary>
        CreteBlue,

        /// <summary>
        /// Cyan
        /// </summary>
        Cyan,

        /// <summary>
        /// Dark Blue
        /// </summary>
        DarkBlue,

        /// <summary>
        /// Dark Gold
        /// </summary>
        DarkGold,

        /// <summary>
        /// Dark Gray
        /// </summary>
        DarkGray,

        /// <summary>
        /// Dark Green
        /// </summary>
        DarkGreen,

        /// <summary>
        /// Dark Orange
        /// </summary>
        DarkOrange,

        /// <summary>
        /// Dark Sea Green
        /// </summary>
        DarkSeaGreen,

        /// <summary>
        /// Firebrick
        /// </summary>
        Firebrick,

        /// <summary>
        /// Forest Green
        /// </summary>
        ForestGreen,

        /// <summary>
        /// Gold
        /// </summary>
        Gold,

        /// <summary>
        /// Gray
        /// </summary>
        Gray,

        /// <summary>
        /// Khaki
        /// </summary>
        Khaki,

        /// <summary>
        /// Lemon Chiffon
        /// </summary>
        LemonChiffon,

        /// <summary>
        /// Light Green
        /// </summary>
        LightGreen,

        /// <summary>
        /// Magenta
        /// </summary>
        Magenta,

        /// <summary>
        /// Navy Blue
        /// </summary>
        NavyBlue,

        /// <summary>
        /// Olive
        /// </summary>
        Olive,

        /// <summary>
        /// Orange
        /// </summary>
        Orange,

        /// <summary>
        /// Orchid
        /// </summary>
        Orchid,

        /// <summary>
        /// Pink
        /// </summary>
        Pink,

        /// <summary>
        /// Purple
        /// </summary>
        Purple,

        /// <summary>
        /// Red
        /// </summary>
        Red,

        /// <summary>
        /// Sea Green
        /// </summary>
        SeaGreen,

        /// <summary>
        /// Sienna
        /// </summary>
        Sienna,

        /// <summary>
        /// Sky Blue
        /// </summary>
        SkyBlue,

        /// <summary>
        /// Slate Blue
        /// </summary>
        SlateBlue,

        /// <summary>
        /// Turquoise
        /// </summary>
        Turquoise,

        /// <summary>
        /// Violet Red
        /// </summary>
        VioletRed,

        /// <summary>
        /// Yellow
        /// </summary>
        Yellow,

        /// <summary>
        /// None returned by some check point inbuilt objects. If used during update color will not
        /// be changed.
        /// </summary>
        [JsonIgnore]
        None
    }
}