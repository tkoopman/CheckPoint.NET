﻿// MIT License
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

namespace Koopman.CheckPoint.SimpleGatewaySettings
{
    /// <summary>
    /// If packets will be rejected (the Prevent option) or whether the packets will be monitored
    /// (the Detect option)
    /// </summary>
    [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase)]
    public enum AntiSpoofingAction
    {
        /// <summary>
        /// To prevent anti-spoofing by dropping the packets
        /// </summary>
        Prevent,

        /// <summary>
        /// To only detect anti-spoofing
        /// </summary>
        Detect
    }
}