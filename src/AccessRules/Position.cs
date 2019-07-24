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
using System;

namespace Koopman.CheckPoint.AccessRules
{
    /// <summary>
    /// Absolute and Reletive positions
    /// </summary>
    [JsonConverter(typeof(EnumConverter), EnumConverter.StringCases.Lowercase)]
    public enum Positions
    {
        /// <summary>
        /// Position is a fixed rule number
        /// </summary>
        Absolute,

        /// <summary>
        /// The top of rulebase or section
        /// </summary>
        Top,

        /// <summary>
        /// Above rule or section
        /// </summary>
        Above,

        /// <summary>
        /// Below rule or section
        /// </summary>
        Below,

        /// <summary>
        /// The bottom of rulebase or section
        /// </summary>
        Bottom
    }

    public enum PositionsRelativeToRule
    {
        /// <summary>
        /// Above rule or section
        /// </summary>
        Above = Positions.Above,

        /// <summary>
        /// Below rule or section
        /// </summary>
        Below = Positions.Below
    }

    public enum PositionsRelativeToRulebase
    {
        /// <summary>
        /// The top of rulebase
        /// </summary>
        Top = Positions.Top,

        /// <summary>
        /// The bottom of rulebase
        /// </summary>
        Bottom = Positions.Bottom
    }

    public enum PositionsRelativeToSection
    {
        /// <summary>
        /// The top of section
        /// </summary>
        Top = Positions.Top,

        /// <summary>
        /// Above section
        /// </summary>
        Above = Positions.Above,

        /// <summary>
        /// Below section
        /// </summary>
        Below = Positions.Below,

        /// <summary>
        /// The bottom of section
        /// </summary>
        Bottom = Positions.Bottom
    }

    /// <summary>
    /// Defines the position of new rulebase lines.
    /// </summary>
    [JsonConverter(typeof(PositionConverter))]
    public struct Position
    {
        #region Constructors

        /// <summary>
        /// Position above or below a section or rule or Position at the top or bottom of a section.
        /// </summary>
        /// <param name="locate">Top, Bottom, Above or Below.</param>
        /// <param name="point">Name or UID of section or rule.</param>
        /// <exception cref="InvalidOperationException">
        /// Cannot use locate value of Absolute in this constructor.
        /// </exception>
        /// <exception cref="ArgumentNullException">point</exception>
        public Position(Positions locate, string point) : this()
        {
            if (locate == Positions.Absolute)
                throw new InvalidOperationException("Cannot use locate value of Absolute in this constructor.");
            Locate = locate;
            Point = point ?? throw new ArgumentNullException(nameof(point));
        }

        /// <summary>
        /// Position above or below a rule.
        /// </summary>
        /// <param name="locate">Above or Below.</param>
        /// <param name="point">Name or UID of rule.</param>
        /// <exception cref="ArgumentNullException">point</exception>
        public Position(PositionsRelativeToRule locate, string point) : this((Positions)locate, point) { }

        /// <summary>
        /// Position top, bottom, above or below a section.
        /// </summary>
        /// <param name="locate">Top, Bottom, Above or Below.</param>
        /// <param name="point">Name or UID of section.</param>
        /// <exception cref="ArgumentNullException">point</exception>
        public Position(PositionsRelativeToSection locate, string point) : this((Positions)locate, point) { }

        /// <summary>
        /// Position at Top or Bottom of rulebase.
        /// </summary>
        /// <param name="locate">Top or Bottom.</param>
        public Position(PositionsRelativeToRulebase locate) : this()
        {
            Locate = (Positions)locate;
        }

        /// <summary>
        /// Position at an absolute rule number.
        /// </summary>
        /// <param name="ruleNumber">The rule number.</param>
        public Position(int ruleNumber) : this()
        {
            Locate = Positions.Absolute;
            Point = ruleNumber.ToString();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets how locations is set. Either absolute or relitive to rulebase, section or existing rule.
        /// </summary>
        /// <value>The locate.</value>
        public Positions Locate { get; }

        /// <summary>
        /// Gets the reference point for Locate.
        /// </summary>
        /// <value>The point.</value>
        public string Point { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (Locate == Positions.Absolute)
                return Point;
            else if (Point == null)
                return Locate.ToString();
            else
            {
                string s = Locate.ToString() +
                    ((Locate == Positions.Top || Locate == Positions.Bottom) ? " of " : " ") +
                    Point;
                return s;
            }
        }

        #endregion Methods
    }
}