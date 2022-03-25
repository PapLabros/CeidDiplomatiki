using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Exposes the required properties for a database foreign key 
    /// </summary>
    public interface IDbProviderForeignKeyColumn : IEquatable<IDbProviderForeignKeyColumn>
    {
        #region Properties

        /// <summary>
        /// Catalog the constraint belongs to.
        /// </summary>
        string ConstraintCatalog { get; set; }

        /// <summary>
        /// Schema that contains the constraint.
        /// </summary>
        string ConstraintSchema { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        string ConstraintName { get; set; }

        /// <summary>
        /// Table name constraint is part of.
        /// </summary>
        string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table that contains the foreign key column.
        /// </summary>
        string TableSchema { get; set; }

        /// <summary>
        /// The table that contains the foreign key column.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        /// Schema that contains the referenced table.
        /// The referenced table usually contains the primary key column.
        /// </summary>
        string ReferencedTableSchema { get; set; }

        /// <summary>
        /// The table that contains the referenced column.
        /// Usually the table that contains the primary key column.
        /// </summary>
        string ReferencedTableName { get; set; }

        /// <summary>
        /// Referenced column.
        /// Usually a primary key column.
        /// </summary>
        string ReferencedColumnName { get; set; }

        /// <summary>
        /// The delete behavior
        /// </summary>
        DatabaseForeignKeyDeleteBehavior OnDelete { get; set; }

        #endregion
    }
}
