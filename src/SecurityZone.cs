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

using Koopman.CheckPoint.Common;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Security Zone Object
    /// </summary>
    /// <example>
    /// Add new security zone using <see cref="SecurityZone.SecurityZone(Session)" />
    /// <code>
    /// var sz = new SecurityZone(Session) {
    ///     Name = "MySecurityZone"
    /// };
    /// sz.AcceptChanges();
    /// </code>
    /// Find security zone using <see cref="Session.FindSecurityZone(string, DetailLevels)" />
    /// <code>
    /// var sz = Session.FindSecurityZone("MySecurityZone");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase" />
    public class SecurityZone : ObjectBase
    {
        #region Constructors

        /// <summary>
        /// Create new <see cref="SecurityZone" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var sz = new SecurityZone(Session) {
        ///     Name = "MySecurityZone"
        /// };
        /// sz.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        public SecurityZone(Session session) : this(session, DetailLevels.Full)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityZone" /> class ready to be populated
        /// with current data.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level of data that will be populated.</param>
        protected internal SecurityZone(Session session, DetailLevels detailLevel) : base(session, detailLevel)
        {
        }

        #endregion Constructors

        #region Classes

        /// <summary>
        /// Valid sort orders for Security Zones
        /// </summary>
        public static class Order
        {
            #region Fields

            /// <summary>
            /// Sort by name in ascending order
            /// </summary>
            public readonly static IOrder NameAsc = new OrderAscending("name");

            /// <summary>
            /// Sort by name in descending order
            /// </summary>
            public readonly static IOrder NameDesc = new OrderDescending("name");

            #endregion Fields
        }

        #endregion Classes
    }
}