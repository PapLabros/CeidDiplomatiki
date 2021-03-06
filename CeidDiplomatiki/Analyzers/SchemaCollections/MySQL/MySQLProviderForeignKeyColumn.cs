using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL foreign key column.
    /// </summary>
    public class MySQLProviderForeignKeyColumn : IDbProviderForeignKeyColumn
    {
        #region Public Properties

        /// <summary>
        /// Catalog the constraint belongs to.
        /// </summary>
        public string ConstraintCatalog { get; set; }

        /// <summary>
        /// Schema that contains the constraint.
        /// </summary>
        public string ConstraintSchema { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string ConstraintName { get; set; }

        /// <summary>
        /// Table name constraint is part of.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table that contains the foreign key column.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// The table that contains the foreign key column.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Schema that contains the referenced table.
        /// </summary>
        public string ReferencedTableSchema { get; set; }

        /// <summary>
        /// The table that contains the referenced column.
        /// </summary>
        public string ReferencedTableName { get; set; }

        /// <summary>
        /// Referenced column.
        /// NOTE: The referenced column is usually a primary key column!
        /// </summary>
        public string ReferencedColumnName { get; set; }

        public uint OrdinalPosition { get; set; }

        public uint PositionInUniqueConstraint { get; set; }

        /// <summary>
        /// The delete behavior
        /// </summary>
        public DatabaseForeignKeyDeleteBehavior OnDelete { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal MySQLProviderForeignKeyColumn(DataRow row)
        {
            ConstraintCatalog = row.GetString(0);
            ConstraintSchema = row.GetString(1);
            ConstraintName = row.GetString(2);
            TableCatalog = row.GetString(3);
            TableSchema = row.GetString(4);
            TableName = row.GetString(5);
            ColumnName = row.GetString(6);

            // If the database is MariaDb...
            if (row.Table.Columns[7].DataType == typeof(long))
                OrdinalPosition = (uint)row.GetLong(7);
            // If the database is MySql...
            else
                OrdinalPosition = row.GetUnsignedInt(7);

            // If the database is MariaDb...
            if (row.Table.Columns[8].DataType == typeof(long))
                PositionInUniqueConstraint = (uint)row.GetLong(8);
            // If the database is MySql...
            else
                PositionInUniqueConstraint = row.GetUnsignedInt(8);

            ReferencedTableSchema = row.GetString(9);
            ReferencedTableName = row.GetString(10);
            ReferencedColumnName = row.GetString(11);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ColumnName}, Constraint: {ConstraintName}";

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDbProviderDatabase dbProviderDatabase)
                return Equals(dbProviderDatabase);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(IDbProviderForeignKeyColumn other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((IDbProviderForeignKeyColumn)this).ConstraintCatalog == other.ConstraintCatalog
                && ((IDbProviderForeignKeyColumn)this).ConstraintSchema == other.ConstraintSchema
                && ((IDbProviderForeignKeyColumn)this).ConstraintName == other.ConstraintName
                && ((IDbProviderForeignKeyColumn)this).TableCatalog == other.TableCatalog
                && ((IDbProviderForeignKeyColumn)this).TableSchema == other.TableSchema
                && ((IDbProviderForeignKeyColumn)this).TableName == other.TableName
                && ((IDbProviderForeignKeyColumn)this).ColumnName == other.ColumnName
                && ((IDbProviderForeignKeyColumn)this).ReferencedTableSchema == other.ReferencedTableSchema
                && ((IDbProviderForeignKeyColumn)this).ReferencedTableName == other.ReferencedTableName
                && ((IDbProviderForeignKeyColumn)this).ReferencedColumnName == other.ReferencedColumnName
                && ((IDbProviderForeignKeyColumn)this).OnDelete == other.OnDelete;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCodeHelpers.Combine(ConstraintCatalog, ConstraintSchema, ConstraintName, TableCatalog, TableSchema, TableName, 
                ColumnName, ReferencedTableSchema, ReferencedTableName, ReferencedColumnName, OrdinalPosition, PositionInUniqueConstraint, OnDelete);

        #endregion
    }
}
