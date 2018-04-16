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

using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Used to represent an unknown object type.
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.IObjectSummary" />
    public class GenericObjectSummary : IObjectSummary
    {
        #region Fields

        /// <summary>
        /// The All gateway to gateway VPNs.
        /// </summary>
        public static readonly GenericObjectSummary AllGwToGw = new GenericObjectSummary(null, DetailLevels.Full, "CpmiAnyObject")
        {
            UID = "97aeb36a-9aed-11d5-bd16-0090272ccb30",
            Name = "All_GwToGw",
            Domain = Domain.DataDomain
        };

        /// <summary>
        /// The Any object.
        /// </summary>
        public static readonly GenericObjectSummary Any = new GenericObjectSummary(null, DetailLevels.Full, "CpmiAnyObject")
        {
            UID = "97aeb369-9aea-11d5-bd16-0090272ccb30",
            Name = "Any",
            Domain = Domain.DataDomain
        };

        /// <summary>
        /// The Policy Targets object.
        /// </summary>
        public static readonly GenericObjectSummary PolicyTargets = new GenericObjectSummary(null, DetailLevels.Full, "Global")
        {
            UID = "6c488338-8eec-4103-ad21-cd461ac2c476",
            Name = "Policy Targets",
            Domain = Domain.DataDomain
        };

        /// <summary>
        /// The restrict common protocols action
        /// </summary>
        public static readonly GenericObjectSummary RestrictCommonProtocolsAction = new GenericObjectSummary(null, DetailLevels.Full, "", true)
        {
            UID = "ea3a425f-56b3-46de-98e7-bd88ce27a801",
            Name = "Restrict_Common_Protocols_Action",
            Domain = Domain.Default
        };

        /// <summary>
        /// The Trust_all_action object.
        /// </summary>
        public static readonly GenericObjectSummary TrustAllAction = new GenericObjectSummary(null, DetailLevels.Full, "", true)
        {
            UID = "226b5ee1-69ce-4bdb-a53f-3a01e68885b4",
            Name = "Trust_all_action",
            Domain = Domain.Default
        };

        internal static readonly GenericObjectSummary[] InBuilt = new GenericObjectSummary[] { Any, PolicyTargets, RestrictCommonProtocolsAction, TrustAllAction, AllGwToGw };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObjectSummary" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="type">The type.</param>
        /// <param name="allowNullResult">if set to <c>true</c> allow null result.</param>
        internal GenericObjectSummary(Session session, DetailLevels detailLevel, string type, bool allowNullResult = false)
        {
            Session = session;
            DetailLevel = detailLevel;
            Type = type;
            AllowNullResult = allowNullResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObjectSummary" /> class with just UID
        /// detail level.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="allowNullResult">if set to <c>true</c> allow null result.</param>
        internal GenericObjectSummary(Session session, string uid, bool allowNullResult = false)
        {
            Session = session;
            DetailLevel = DetailLevels.UID;
            UID = uid;
            AllowNullResult = allowNullResult;
        }

        #endregion Constructors



        #region Fields

        private Domain _domain;
        private string _name;
        private string _type;
        private IObjectSummary cache = null;
        private bool cached = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the current detail level of retrieved objects. You will not be able to get the
        /// values of some properties if the detail level is too low. You can still set property
        /// values however to override existing values.
        /// </summary>
        /// <value>The current detail level.</value>
        public DetailLevels DetailLevel { get; }

        /// <summary>
        /// Information about the domain the object belongs to.
        /// </summary>
        /// <value>The domain.</value>
        /// <exception cref="DetailLevelException"></exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "domain", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Domain Domain
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _domain : null;
            private set => _domain = value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew => false;

        /// <summary>
        /// Object name. Should be unique in the domain.
        /// </summary>
        /// <value>The object's name.</value>
        /// <exception cref="DetailLevelException"></exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Name
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _name : null;
            internal set => _name = value;
        }

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        /// <exception cref="DetailLevelException"></exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "type")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Type
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _type : null;
            private set => _type = value;
        }

        /// <summary>
        /// Object unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        public string UID { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether null can be returned if class cast error.
        /// </summary>
        /// <value><c>true</c> if allow null result; otherwise, <c>false</c>.</value>
        internal bool AllowNullResult { get; }

        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the identifier that is used when adding this object to a group.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        /// <summary>
        /// Reloads the current object. This should return the full IOBjectSummary that matches this UID
        /// </summary>
        /// <param name="OnlyIfPartial">
        /// Only perform reload if <paramref name="detailLevel" /> is not already <see cref="DetailLevels.Full" />
        /// </param>
        /// <param name="detailLevel">The detail level of child objects to retrieve.</param>
        /// <returns>IObjectSummary of reloaded object</returns>
        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = Find.Defaults.DetailLevel)
        {
            if (cache == null || !OnlyIfPartial || cache.DetailLevel < detailLevel)
                cache = Find.Invoke(Session, UID, detailLevel);

            return cache;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => GetIdentifier();

        internal IObjectSummary GetFromCache(ObjectConverter objectConverter)
        {
            if (!cached)
            {
                cache = objectConverter.GetFromCache(UID);
                cached = true;
            }

            return cache;
        }

        /// <summary>
        /// Tests the current detail level and takes action if too low.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="detailLevelAction">The action to take if too low.</param>
        /// <returns>
        /// <c>true</c> if detail level matches required value or if auto reload was successful;
        /// otherwise <c>false</c>
        /// </returns>
        /// <exception cref="DetailLevelException">
        /// Thrown if detail level too low and action <see cref="DetailLevelActions.ThrowException" />
        /// </exception>
        protected bool TestDetailLevel(DetailLevels minValue, DetailLevelActions detailLevelAction = DetailLevelActions.SessionDefault)
        {
            if (DetailLevel < minValue)
            {
                var action =
                    (detailLevelAction == DetailLevelActions.SessionDefault) ?
                        Session.DetailLevelAction :
                        detailLevelAction;

                switch (action)
                {
                    case DetailLevelActions.ReturnNull:
                        return false;

                    case DetailLevelActions.AutoReload:
                        Reload();
                        return (DetailLevel == DetailLevels.Full);

                    default:
                        throw new DetailLevelException(DetailLevel, minValue);
                }
            }
            else return true;
        }

        #endregion Methods
    }
}