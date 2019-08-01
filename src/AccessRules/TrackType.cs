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
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Track type
    /// </summary>
    public class TrackType : IObjectSummary
    {
        #region Fields

        /// <summary>
        /// Track with detailed logging
        /// </summary>
        public static readonly TrackType DetailedLog = new TrackType(Domain.DataDomain, "Detailed Log", DetailedLogUID);

        /// <summary>
        /// Track with extended logging
        /// </summary>
        public static readonly TrackType ExtendedLog = new TrackType(Domain.DataDomain, "Extended Log", ExtendedLogUID);

        /// <summary>
        /// Track with standard logging
        /// </summary>
        public static readonly TrackType Log = new TrackType(Domain.DataDomain, "Log", LogUID);

        /// <summary>
        /// No tracking
        /// </summary>
        public static readonly TrackType None = new TrackType(Domain.DataDomain, "None", NoneUID);

        internal static readonly TrackType[] Types = new TrackType[] { None, Log, DetailedLog, ExtendedLog };

        private const string DetailedLogUID = "d395a3bb-c96c-4781-8017-b9cd0418e982";
        private const string ExtendedLogUID = "78566494-7e96-4513-ada9-ded83f4ee9ea";
        private const string LogUID = "598ead32-aa42-4615-90ed-f51a5928d41d";
        private const string NoneUID = "29e53e3d-23bf-48fe-b6b1-d59bd88036f9";

        #endregion Fields

        #region Constructors

        private TrackType(Domain domain, string name, string uID, string type = "Track")
        {
            Domain = domain;
            Name = name;
            UID = uID;
            Type = type;
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public DetailLevels DetailLevel => DetailLevels.Full;

        /// <inheritdoc />
        public Domain Domain { get; }

        /// <inheritdoc />
        public bool IsNew => false;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public ObjectType ObjectType => ObjectType.Unknown;

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public string UID { get; }

        #endregion Properties

        #region Methods

        public static implicit operator string(TrackType a) => a.GetIdentifier();

        public static implicit operator TrackType(string a)
        {
            switch (a.ToLower())
            {
                case "none":
                    return None;

                case "log":
                    return Log;

                case "detailedlog":
                case "detailed log":
                case "detailed":
                    return DetailedLog;

                case "extendedlog":
                case "extended log":
                case "extended":
                    return ExtendedLog;

                default:
                    throw new System.InvalidCastException("Invalid Track Type");
            }
        }

        public static implicit operator TrackType(TrackTypes a)
        {
            switch (a)
            {
                case TrackTypes.None:
                    return None;

                case TrackTypes.Log:
                    return Log;

                case TrackTypes.DetailedLog:
                    return DetailedLog;

                case TrackTypes.ExtendedLog:
                    return ExtendedLog;

                default:
                    throw new System.InvalidCastException("Invalid Action");
            }
        }

        public static implicit operator TrackTypes(TrackType a)
        {
            switch (a.UID)
            {
                case NoneUID:
                    return TrackTypes.None;

                case LogUID:
                    return TrackTypes.Log;

                case DetailedLogUID:
                    return TrackTypes.DetailedLog;

                case ExtendedLogUID:
                    return TrackTypes.ExtendedLog;

                default:
                    throw new System.InvalidCastException("Invalid Track Type");
            }
        }

        /// <inheritdoc />
        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        /// <inheritdoc />
        public Task<IObjectSummary> Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard, CancellationToken cancellationToken = default) => Task.FromResult((IObjectSummary)this);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => GetIdentifier();

        #endregion Methods
    }
}