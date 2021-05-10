namespace CeidDiplomatiki
{
    /// <summary>
    /// Provides abstractions for the CeidDiplomatiki main page builder
    /// </summary>
    public interface ICeidDiplomatikiMainPageBuilder
    {
        #region Methods

        /// <summary>
        /// Recreates the dynamically created menu buttons
        /// </summary>
        void Refresh();

        #endregion
    }
}
