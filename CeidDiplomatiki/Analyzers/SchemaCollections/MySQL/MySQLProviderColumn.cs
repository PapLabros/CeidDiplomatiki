
using Atom.Core;

using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL column
    /// </summary>
    public class MySQLProviderColumn : IDbProviderColumn
    {
        #region Private Members

        /// <summary>
        /// The member of the <see cref="DataType"/> property
        /// </summary>
        private Type mDataType;

        /// <summary>
        /// The member of the <see cref="ColumnDefault"/> property
        /// </summary>
        private object mColumnDefault;

        /// <summary>
        /// The member of the <see cref="IsNullable"/> property
        /// </summary>
        private bool mIsNullable;

        #endregion

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
        /// Column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Column identification number.
        /// </summary>
        public uint OrdinalPosition { get; set; }

        /// <summary>
        /// The string representation of the column default value
        /// </summary>
        public string ColumnDefault { get; set; }

        /// <summary>
        /// A flag indicating whether the column can except null value. 
        /// </summary>
        public bool IsNullable
        {
            get => mIsNullable;

            set
            {
                mIsNullable = value;

                if (DataType == null)
                    return;

                var castedInstance = this.CastTo<IDbProviderColumn>();

                if (castedInstance.DataType.IsValueType && value)
                    castedInstance.DataType = TypeHelpers.GetNonNullableType(castedInstance.DataType);
            }
        }

        /// <summary>
        /// The string representation of the column data type
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Maximum length, in characters, for binary data, character data, or text and image data. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public ulong? CharacterMaximumLength { get; set; }

        /// <summary>
        /// Precision of approximate numeric data, exact numeric data, integer data, or monetary data. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public ulong? NumericPrecision { get; set; }

        /// <summary>
        /// Scale of approximate numeric data, exact numeric data, integer data, or monetary data. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public ulong? NumericScale { get; set; }

        /// <summary>
        /// Subtype code for date-time and SQL-92 interval data types. 
        /// For other data types, NULL is returned.
        /// </summary>
        public ulong? DateTimePrecision { get; set; }

        /// <summary>
        /// Returns the unique name for the character set if this column is character data or text data type. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public string CharacterSetName { get; set; }

        public string CollationName { get; set; }

        /// <summary>
        /// The SQL column type
        /// </summary>
        public string ColumnType { get; set; }

        public string ColumnKey { get; set; }

        public string Extra { get; set; }

        public string Privileges { get; set; }

        /// <summary>
        /// The comments for the column
        /// </summary>
        public string ColumnComment { get; set; }

        /// <summary>
        /// The expression that generates the column values 
        /// </summary>
        public string GenerationExpression { get; set; }

        /// <summary>
        /// Default value of the column
        /// </summary>
        object IDbProviderColumn.ColumnDefault
        {
            get
            {
                if (mColumnDefault == null)
                    return ColumnDefault;
                else
                    return mColumnDefault;
            }

            set => mColumnDefault = value;
        }

        /// <summary>
        /// The database provider SQL data type
        /// </summary>
        string IDbProviderColumn.DbProviderSQLType { get; set; }

        /// <summary>
        /// System-supplied data type.
        /// </summary>
        Type IDbProviderColumn.DataType
        {
            get => mDataType;

            set
            {
                mDataType = value;
            }
        }

        /// <summary>
        /// The SQL data type
        /// </summary>
        string IDbProviderColumn.SQLTypeName { get => ColumnType; set { ColumnType = value; } }

        /// <summary>
        /// A flag indicating whether the column is a primary key or not.
        /// </summary>
        bool IDbProviderColumn.IsPrimaryKey { get; set; }

        /// <summary>
        /// A flag indicating whether the column has unique values or not.
        /// </summary>
        bool IDbProviderColumn.IsUnique { get; set; }

        /// <summary>
        /// A flag indicating whether the value is auto increment or not.
        /// </summary>
        bool IDbProviderColumn.IsAutoIncrement { get; set; }

        /// <summary>
        /// Column identification number.
        /// </summary>
        int IDbProviderColumn.OrdinalPosition { get => (int)OrdinalPosition; set => OrdinalPosition = (uint)value; }

        /// <summary>
        /// The column value that will be used when the next row is inserted if <see cref="IDbProviderColumn.IsAutoIncrement"/> is true.
        /// </summary>
        uint? IDbProviderColumn.NextAutoIncrementValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal MySQLProviderColumn(DataRow row)
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            ColumnName = row.GetString(3);

            // If the database is MariaDb...
            if (row.Table.Columns[4].DataType == typeof(ulong))
                OrdinalPosition = (uint)row.GetUnsignedLong(4);
            // If the database is MySql...
            else
                OrdinalPosition = row.GetUnsignedInt(4);

            ColumnDefault = row.GetDbNullableString(5);
            IsNullable = row.GetBool(6);
            DataType = row.GetString(7);
            CharacterMaximumLength = row.GetDbNullableUnsignedLong(8);
            NumericPrecision = row.GetDbNullableUnsignedLong(9);
            NumericScale = row.GetDbNullableUnsignedLong(10);
            DateTimePrecision = row.GetDbNullableUnsignedLong(11);
            CharacterSetName = row.GetDbNullableString(12);
            CollationName = row.GetDbNullableString(13);
            ColumnType = row.GetString(14);
            ColumnKey = row.GetString(15);
            Extra = row.GetDbNullableString(16);
            Privileges = row.GetDbNullableString(17);
            ColumnComment = row.GetDbNullableString(18);
            GenerationExpression = row.GetDbNullableString(19);

            var castedInstance = this.CastTo<IDbProviderColumn>();

            castedInstance.IsAutoIncrement = Extra.Contains("auto_increment");
            castedInstance.IsPrimaryKey = ColumnKey == "PRI";
            castedInstance.IsUnique = castedInstance.IsPrimaryKey;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ColumnName;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDbProviderColumn providerColumn)
                return Equals(providerColumn);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(IDbProviderColumn other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((IDbProviderColumn)this).TableCatalog == other.TableCatalog
                && ((IDbProviderColumn)this).TableSchema == other.TableSchema
                && ((IDbProviderColumn)this).TableName == other.TableName
                && ((IDbProviderColumn)this).ColumnName == other.ColumnName
                && ((IDbProviderColumn)this).OrdinalPosition == other.OrdinalPosition
                && ((IDbProviderColumn)this).ColumnDefault == other.ColumnDefault
                && ((IDbProviderColumn)this).DataType == other.DataType
                && ((IDbProviderColumn)this).SQLTypeName == other.SQLTypeName
                && ((IDbProviderColumn)this).IsPrimaryKey == other.IsPrimaryKey
                && ((IDbProviderColumn)this).IsUnique == other.IsUnique
                && ((IDbProviderColumn)this).IsNullable == other.IsNullable
                && ((IDbProviderColumn)this).IsAutoIncrement == other.IsAutoIncrement;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCodeHelpers.Combine(TableCatalog, TableSchema, TableName, ColumnName, OrdinalPosition, ColumnDefault, 
                IsNullable, DataType, CharacterMaximumLength, NumericPrecision, NumericScale, DateTimePrecision, 
                CharacterSetName, CollationName, ColumnType, ColumnKey, Extra, Privileges, ColumnComment, GenerationExpression);

        #endregion
    }
}
