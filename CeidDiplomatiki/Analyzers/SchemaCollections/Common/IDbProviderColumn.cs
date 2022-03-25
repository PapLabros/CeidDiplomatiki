using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Exposes the required properties for a database column
    /// </summary>
    public interface IDbProviderColumn : IEquatable<IDbProviderColumn>
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
        /// Column name.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        /// Column identification number.
        /// </summary>
        int OrdinalPosition { get; set; }

        /// <summary>
        /// Default value of the column
        /// </summary>
        object ColumnDefault { get; set; }

        /// <summary>
        /// System-supplied data type.
        /// </summary>
        Type DataType { get; set; }

        /// <summary>
        /// The SQL data name
        /// </summary>
        string SQLTypeName { get; set; }

        /// <summary>
        /// The database provider SQL data type
        /// </summary>
        string DbProviderSQLType { get; set; }

        /// <summary>
        /// A flag indicating whether the column is a primary key or not.
        /// </summary>
        bool IsPrimaryKey { get; set; }

        /// <summary>
        /// A flag indicating whether the column has unique values or not.
        /// </summary>
        bool IsUnique { get; set; }

        /// <summary>
        /// A flag indicating whether the column can except null value. 
        /// </summary>
        bool IsNullable { get; set; }

        /// <summary>
        /// A flag indicating whether the value is auto increment or not.
        /// </summary>
        bool IsAutoIncrement { get; set; }

        /// <summary>
        /// The column value that will be used when the next row is inserted if <see cref="IsAutoIncrement"/> is true.
        /// </summary>
        uint? NextAutoIncrementValue { get; set; }

        #endregion
    }
}
