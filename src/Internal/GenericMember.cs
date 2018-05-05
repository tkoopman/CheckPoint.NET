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

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint.Internal
{
    /// <summary>
    /// Used to represent an unknown object type when is needs to be used as a member of another object.
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.IObjectSummary" />
    /// <seealso cref="Koopman.CheckPoint.Common.IGroupMember" />
    /// <seealso cref="Koopman.CheckPoint.Common.IServiceGroupMember" />
    internal class GenericMember : IObjectSummary, IGroupMember, IServiceGroupMember, IApplicationGroupMember
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMember" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="uid">The uid.</param>
        internal GenericMember(Session session, string uid)
        {
            UID = uid;
            Session = session;
        }

        #endregion Constructors

        #region Fields

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
        public DetailLevels DetailLevel => DetailLevels.UID;

        /// <summary>
        /// Information about the domain the object belongs to.
        /// </summary>
        /// <value>The domain.</value>
        /// <exception cref="DetailLevelException"></exception>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        public Domain Domain => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

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
        public string Name => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        /// <exception cref="DetailLevelException"></exception>
        public string Type => throw new DetailLevelException(DetailLevel, DetailLevels.Standard);

        /// <summary>
        /// Object unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        public string UID { get; }

        private Session Session { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the identifier that is used when adding this object to a group.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        public string GetIdentifier() => UID;

        /// <inheritdoc />
        public async Task<IObjectSummary> Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard, CancellationToken cancellationToken = default)
        {
            if (cache == null || !OnlyIfPartial || cache.DetailLevel < detailLevel)
                cache = await Find.InvokeAsync(Session, UID, detailLevel, cancellationToken);

            return cache;
        }

        public override string ToString() => UID;

        internal IObjectSummary GetFromCache(ObjectConverter objectConverter)
        {
            if (!cached)
            {
                cache = objectConverter.GetFromCache(UID);
                cached = true;
            }

            return cache;
        }

        #endregion Methods
    }
}