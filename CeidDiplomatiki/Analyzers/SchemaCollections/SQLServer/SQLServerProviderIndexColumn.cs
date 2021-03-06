using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server index column
    /// </summary>
    internal class SQLServerProviderIndexColumn : IDbProviderIndex
    {
        #region Public Properties

        /// <summary>
        /// Catalog that index belongs to.
        /// </summary>
        public string IndexCatalog { get; set; }

        /// <summary>
        /// Schema that contains the index.
        /// </summary>
        public string IndexSchema { get; set; }

        /// <summary>
        /// Name of the index.
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Table Name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Column name the index is associated with.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Column ordinal position.
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Table name the index is associated with.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table the index is associated with.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// The type of object.
        /// </summary>
        public byte KeyType { get; set; }

        /// <summary>
        /// The index name for the column the index is associated with.
        /// </summary>
        public string ColumnIndexName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        public SQLServerProviderIndexColumn(DataRow row) : base()
        {
            IndexCatalog = row.GetString(0);
            IndexSchema = row.GetString(1);
            IndexName = row.GetString(2);
            TableCatalog = row.GetString(3);
            TableSchema = row.GetString(4);
            TableName = row.GetString(5);
            ColumnName = row.GetString(6);
            OrdinalPosition = row.GetInt(7);
            KeyType = row.GetByte(8);
            ColumnIndexName = row.GetString(9);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ColumnName}, Index: {IndexName}";

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDbProviderIndex dbProviderDatabase)
                return Equals(dbProviderDatabase);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(IDbProviderIndex other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((IDbProviderIndex)this).IndexCatalog == other.IndexCatalog
                && ((IDbProviderIndex)this).IndexSchema == other.IndexSchema
                && ((IDbProviderIndex)this).IndexName == other.IndexName
                && ((IDbProviderIndex)this).TableName == other.TableName
                && ((IDbProviderIndex)this).ColumnName == other.ColumnName
                && ((IDbProviderIndex)this).OrdinalPosition == other.OrdinalPosition;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCodeHelpers.Combine(IndexCatalog, IndexSchema, IndexName, TableName, 
                ColumnName, OrdinalPosition, TableCatalog, TableSchema, KeyType, ColumnIndexName);

        #endregion
    }
}
