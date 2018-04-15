﻿using Koopman.CheckPoint.Json;
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
        /// Position at Top or Bottom of rulebase.
        /// </summary>
        /// <param name="locate">Top or Bottom.</param>
        /// <exception cref="InvalidOperationException">
        /// Cannot use locate values of Absolute, Above or Below in this constructor.
        /// </exception>
        public Position(Positions locate) : this()
        {
            if (locate != Positions.Top && locate != Positions.Bottom)
                throw new InvalidOperationException("Cannot use locate values of Absolute, Above or Below in this constructor.");
            Locate = locate;
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