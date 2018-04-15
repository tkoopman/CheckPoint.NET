using Koopman.CheckPoint.Common;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Object Summary Interface
    /// </summary>
    public interface IObjectSummary
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
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        Domain Domain { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        bool IsNew { get; }

        /// <summary>
        /// Object name. Should be unique in the domain.
        /// </summary>
        /// <value>The object's name.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        /// <exception cref="System.NotImplementedException"></exception>
        string Name { get; }

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
        /// Gets the identifier.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        string GetIdentifier();

        /// <summary>
        /// Reloads the current object. Used to either reset changes made without saving, or to
        /// increased the <paramref name="detailLevel" /> to <see cref="DetailLevels.Full" />
        /// </summary>
        /// <param name="OnlyIfPartial">
        /// Only perform reload if <paramref name="detailLevel" /> is not already <see cref="DetailLevels.Full" />
        /// </param>
        /// <param name="detailLevel">The detail level of child objects to retrieve.</param>
        /// <returns>IObjectSummary of reloaded object</returns>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        IObjectSummary Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard);

        #endregion Methods
    }
}