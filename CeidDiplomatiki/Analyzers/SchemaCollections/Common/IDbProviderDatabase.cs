using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Exposes the required properties for a database
    /// </summary>
    public interface IDbProviderDatabase : IEquatable<IDbProviderDatabase>
    {
        #region Properties

        /// <summary>
        /// Name of the catalog
        /// </summary>
        string CatalogName { get; set; }

        /// <summary>
        /// Name of the database.
        /// </summary>
        string DatabaseName { get; set; }

        #endregion
    }
}
