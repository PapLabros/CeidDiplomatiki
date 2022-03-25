using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Exposes the required properties for a database index column
    /// </summary>
    public interface IDbProviderIndex : IEquatable<IDbProviderIndex>
    {
        #region Public Properties

        /// <summary>
        /// Catalog that index belongs to.
        /// </summary>
        string IndexCatalog { get; set; }

        /// <summary>
        /// Schema that contains the index.
        /// </summary>
        string IndexSchema { get; set; }

        /// <summary>
        /// Name of the index.
        /// </summary>
        string IndexName { get; set; }

        /// <summary>
        /// Table Name.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Column name the index is associated with.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        /// Column ordinal position.
        /// </summary>
        int OrdinalPosition { get; set; }

        #endregion
    }
}
