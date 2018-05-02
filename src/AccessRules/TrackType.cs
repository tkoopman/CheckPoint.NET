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
        public static readonly TrackType DetailedLog = new TrackType(Domain.DataDomain, "Detailed Log", "d395a3bb-c96c-4781-8017-b9cd0418e982");

        /// <summary>
        /// Track with extended logging
        /// </summary>
        public static readonly TrackType ExtendedLog = new TrackType(Domain.DataDomain, "Extended Log", "78566494-7e96-4513-ada9-ded83f4ee9ea");

        /// <summary>
        /// Track with standard logging
        /// </summary>
        public static readonly TrackType Log = new TrackType(Domain.DataDomain, "Log", "598ead32-aa42-4615-90ed-f51a5928d41d");

        /// <summary>
        /// No tracking
        /// </summary>
        public static readonly TrackType None = new TrackType(Domain.DataDomain, "None", "29e53e3d-23bf-48fe-b6b1-d59bd88036f9");

        internal static readonly TrackType[] Types = new TrackType[] { None, Log, DetailedLog, ExtendedLog };

        #endregion Fields

        #region Constructors

        private TrackType(Domain domain, string name, string uID, string type = "RulebaseAction")
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
        public string Type { get; }

        /// <inheritdoc />
        public string UID { get; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public string GetIdentifier() => (string.IsNullOrWhiteSpace(Name)) ? UID : Name;

        /// <inheritdoc />
        public IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard) => this;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => GetIdentifier();

        #endregion Methods
    }
}