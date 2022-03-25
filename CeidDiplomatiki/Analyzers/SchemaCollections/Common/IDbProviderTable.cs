using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Exposes the required properties for a database table
    /// </summary>
    public interface IDbProviderTable : IEquatable<IDbProviderTable>
    {
        #region Properties

        /// <summary>
        /// Catalog of the table.
        /// </summary>
        string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table.
        /// </summary>
        string TableSchema { get; set; }

        /// <summary>
        /// Table name.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Type of table. Can be VIEW or BASE TABLE.
        /// </summary>
        string TableType { get; set; }

        #endregion
    }
}
