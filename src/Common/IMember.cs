namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Identifies objects that can be assigned as members a group. Should only be assigned to
    /// classes that inherit <see cref="ObjectSummary" />
    /// </summary>
    public interface IMember
    {
        #region Properties

        /// <summary>
        /// Gets the current detail level of retrieved objects. You will not be able to get the
        /// values of some properties if the detail level is too low. You can still set property
        /// values however to override existing values.
        /// </summary>
        /// <value>The current detail level.</value>
        DetailLevels DetailLevel { get; }

        /// <summary>
        /// Information about the domain the object belongs to.
        /// </summary>
        /// <value>The domain.</value>
        /// <remarks>Requires <see cref="ObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        Domain Domain { get; }

        /// <summary>
        /// Object name. Should be unique in the domain.
        /// </summary>
        /// <value>The object's name.</value>
        /// <remarks>Requires <see cref="ObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        string Name { get; set; }

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; }

        /// <summary>
        /// Object unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        string UID { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the identifier that is used when adding this object to a group.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        string GetMembershipID();

        #endregion Methods
    }
}