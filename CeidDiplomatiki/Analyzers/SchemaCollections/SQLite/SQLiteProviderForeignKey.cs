using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQLite foreign key column.
    /// </summary>
    public class SQLiteProviderForeignKey : IDbProviderForeignKeyColumn
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
        public string ColumnName { get => ForeignKeyFromColumn; set { ForeignKeyFromColumn = value; } }

        /// <summary>
        /// Schema that contains the referenced table.
        /// </summary>
        public string ReferencedTableSchema { get; set; }

        /// <summary>
        /// The table that contains the referenced column.
        /// </summary>
        public string ReferencedTableName { get => ForeignKeyToTable; set { ForeignKeyToTable = value; } }

        /// <summary>
        /// Referenced column.
        /// NOTE: The referenced column is usually a primary key column!
        /// </summary>
        public string ReferencedColumnName { get => ForeignKeyToColumn; set { ForeignKeyToColumn = value; } }

        public string ConstraintType { get; set; }

        public bool IsDeferable { get; set; }

        public bool InitiallyDeferred { get; set; }

        public int ForeignKeyId { get; set; }

        public string ForeignKeyFromColumn { get; set; }

        public int ForeignKeyFromOrdinalPosition { get; set; }

        public string ForeignKeyToCaltalog { get; set; }

        public string ForeignKeyToSchema { get; set; }

        public string ForeignKeyToTable { get; set; }

        public string ForeignKeyToColumn { get; set; }

        public string ForeignKeyOnUpdate { get; set; }

        public string ForeignKeyOnDelete { get; set; }

        public string ForeignKeyMatch { get; set; }

        /// <summary>
        /// The delete behavior
        /// </summary>
        DatabaseForeignKeyDeleteBehavior IDbProviderForeignKeyColumn.OnDelete
        {
            get
            {
                if (ForeignKeyOnDelete == "RESTRICT")
                    return DatabaseForeignKeyDeleteBehavior.Restrict;

                if (ForeignKeyOnDelete == "NO ACTION")
                    return DatabaseForeignKeyDeleteBehavior.NoAction;

                if (ForeignKeyOnDelete == "SET NULL")
                    return DatabaseForeignKeyDeleteBehavior.SetNull;

                return DatabaseForeignKeyDeleteBehavior.Cascade;
            }

            set
            {
                if (value == DatabaseForeignKeyDeleteBehavior.Restrict)
                    ForeignKeyOnDelete = "RESTRICT";

                if (value == DatabaseForeignKeyDeleteBehavior.NoAction)
                    ForeignKeyOnDelete = "NO ACTION";

                if (value == DatabaseForeignKeyDeleteBehavior.SetNull)
                    ForeignKeyOnDelete = "SET NULL";

                ForeignKeyOnDelete = "CASCADE";
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLiteProviderForeignKey(DataRow row) : base()
        {
            ConstraintCatalog = row.GetString(0);
            ConstraintSchema = row.GetDbNullableString(1);
            ConstraintName = row.GetString(2);
            TableCatalog = row.GetString(3);
            TableSchema = row.GetDbNullableString(4);
            TableName = row.GetString(5);
            ConstraintType = row.GetString(6);
            IsDeferable = row.GetBool(7);
            InitiallyDeferred = row.GetBool(8);
            ForeignKeyId = row.GetInt(9);
            ForeignKeyFromColumn = row.GetString(10);
            ForeignKeyFromOrdinalPosition = row.GetInt(11);
            ForeignKeyToCaltalog = row.GetString(12);
            ForeignKeyToSchema = row.GetDbNullableString(13);
            ForeignKeyToTable = row.GetString(14);
            ForeignKeyToColumn = row.GetString(15);
            ForeignKeyOnUpdate = row.GetString(16);
            ForeignKeyOnDelete = row.GetString(17);
            ForeignKeyMatch = row.GetString(18);
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
            => HashCodeHelpers.Combine(ConstraintCatalog, ConstraintSchema, ConstraintName, TableCatalog, TableSchema, TableName, ColumnName, 
                ReferencedTableSchema, ReferencedTableName, ReferencedColumnName, ConstraintType, IsDeferable, InitiallyDeferred, 
                ForeignKeyId, ForeignKeyFromColumn, ForeignKeyFromOrdinalPosition, ForeignKeyToCaltalog, ForeignKeyToSchema, ForeignKeyToTable, 
                ForeignKeyToColumn, ForeignKeyOnUpdate, ForeignKeyOnDelete, ForeignKeyMatch);

        #endregion
    }
}
