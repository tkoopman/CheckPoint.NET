// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Action to take when current detail level is insufficient
    /// </summary>
    public enum DetailLevelActions
    {
        /// <summary>
        /// Throw a DetailLevelException. Default.
        /// </summary>
        ThrowException,

        /// <summary>
        /// Just return null, or equivalent
        /// </summary>
        ReturnNull,

        /// <summary>
        /// Automatically run Reload method to get full details.
        /// </summary>
        AutoReload,

        /// <summary>
        /// Use the session setting.
        /// If assigned as Session.Options.DetailLevelAction action will be default ThrowException.
        /// </summary>
        SessionDefault
    }
}