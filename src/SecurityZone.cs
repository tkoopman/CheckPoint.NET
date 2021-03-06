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
using System.Threading;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Check Point Security Zone Object
    /// </summary>
    /// <example>
    /// Add new security zone using <see cref="SecurityZone.SecurityZone(Session, bool)" />
    /// <code>
    /// var sz = new SecurityZone(Session) {
    /// Name = "MySecurityZone"
    /// };
    /// sz.AcceptChanges();
    /// </code>
    /// Find security zone using <see cref="Session.FindSecurityZone(string, DetailLevels, CancellationToken)" />
    /// <code>
    /// var sz = Session.FindSecurityZone("MySecurityZone");
    /// </code>
    /// </example>
    /// <seealso cref="Koopman.CheckPoint.Common.ObjectBase{T}" />
    public class SecurityZone : ObjectBase<SecurityZone>
    {
        #region Constructors

        /// <summary>
        /// Create new <see cref="SecurityZone" />.
        /// </summary>
        /// <example>
        /// <code>
        /// var sz = new SecurityZone(Session) {
        /// Name = "MySecurityZone"
        /// };
        /// sz.AcceptChanges();
        /// </code>
        /// </example>
        /// <param name="session">The current session.</param>
        /// <param name="setIfExists">
        /// if set to <c>true</c> if another object with the same name already exists, it will be
        /// updated. Pay attention that original object's fields will be overwritten by the fields
        /// provided in the request payload!.
        /// </param>
        public SecurityZone(Session session, bool setIfExists = false) : this(session, DetailLevels.Full)
        {
            SetIfExists = setIfExists;
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

        #region Properties

        /// <inheritdoc />
        public override ObjectType ObjectType => ObjectType.SecurityZone;

        /// <inheritdoc />
        public override string Type => "security-zone";

        #endregion Properties

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