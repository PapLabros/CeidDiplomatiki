using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a PostgreSQL index column
    /// </summary>
    internal class PostgreSQLProviderIndexColumn : IDbProviderIndex
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
        /// The catalog that contains the index constraint
        /// </summary>
        public string ConstraintCatalog { get; set; }

        /// <summary>
        /// The schema that contains the index constraint
        /// </summary>
        public string ConstraintSchema { get; set; }

        /// <summary>
        /// The name of the constraint
        /// </summary>
        public string ConstraintName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal PostgreSQLProviderIndexColumn(DataRow row) : base()
        {
            ConstraintCatalog = row.GetString(0);
            ConstraintSchema = row.GetString(1);
            IndexCatalog = ConstraintCatalog;
            IndexSchema = ConstraintSchema;
            TableName = row.GetString(5);
            IndexName = row.GetString(7);
            ConstraintName = IndexName;
            ColumnName = row.GetString(6);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ColumnName}, Index{IndexName}";

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
            => HashCodeHelpers.Combine(IndexCatalog, IndexSchema, IndexName, TableName, ColumnName, OrdinalPosition, ConstraintCatalog, ConstraintSchema, ConstraintName);

        #endregion
    }
}
