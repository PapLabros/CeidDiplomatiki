using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server table
    /// </summary>
    internal class SQLServerProviderTable : IDbProviderTable
    {
        #region Public Properties

        /// <summary>
        /// Catalog of the table.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Type of table. Can be VIEW or BASE TABLE.
        /// </summary>
        public string TableType { get; set; }

        #endregion

        #region Constructors 

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderTable(DataRow row) : base()
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            TableType = row.GetString(3);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => TableName;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDbProviderTable dbProviderTable)
                return Equals(dbProviderTable);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(IDbProviderTable other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return TableCatalog == other.TableCatalog &&
                   TableSchema == other.TableSchema &&
                   TableName == other.TableName &&
                   TableType == other.TableType;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(TableCatalog, TableSchema, TableName, TableType);

        #endregion
    }
}
