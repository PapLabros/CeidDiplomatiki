
using Atom.Core;

using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents an SQLite column
    /// </summary>
    public class SQLiteProviderColumn : IDbProviderColumn
    {
        #region Private Members

        /// <summary>
        /// The member of the <see cref="IDbProviderColumn.DataType"/> property
        /// </summary>
        private Type mDataType;

        /// <summary>
        /// The member of the <see cref="IDbProviderColumn.IsNullable"/> property
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
        public int OrdinalPosition { get; set; }

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

                var castedInstance = this.CastTo<IDbProviderColumn>();

                if (castedInstance.DataType == null)
                    return;

                if (castedInstance.DataType.IsValueType && value)
                    castedInstance.DataType = TypeHelpers.GetNonNullableType(castedInstance.DataType);
            }
        }

        /// <summary>
        /// The string representation of the column data type
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The unique column id
        /// </summary>
        public string ColumnGUID { get; set; }

        public long? ColumnPropID { get; set; }

        /// <summary>
        /// A flag indicating whether the column has a default value. 
        /// </summary>
        public bool ColumnHasDefault { get; set; }

        public string ColumnFlags { get; set; }

        /// <summary>
        /// The unique data type id
        /// </summary>
        public string TypeGUID { get; set; }

        /// <summary>
        /// Maximum length, in characters, for binary data, character data, or text and image data.
        /// </summary>
        public int CharacterMaximumLength { get; set; }

        /// <summary>
        /// Maximum length, in characters, for binary data, character data, or text and image data. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public int? CharacterOctetLength { get; set; }

        /// <summary>
        /// Precision of approximate numeric data, exact numeric data, integer data, or monetary data. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public long? NumericPrecision { get; set; }

        /// <summary>
        /// Scale of approximate numeric data, exact numeric data, integer data, or monetary data. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public long? NumericScale { get; set; }

        /// <summary>
        /// Subtype code for date-time and SQL-92 interval data types. 
        /// For other data types, NULL is returned.
        /// </summary>
        public long? DateTimePrecision { get; set; }

        /// <summary>
        /// Returns master, indicating the database in which the character set is located, if the column is character data or text data type. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public string CharacterSetCatalog { get; set; }

        public string CharacterSetSchema { get; set; }

        /// <summary>
        /// Returns the unique name for the character set if this column is character data or text data type. 
        /// Otherwise, NULL is returned.
        /// </summary>
        public string CharacterSetName { get; set; }

        /// <summary>
        /// Returns master, indicating the database in which the collation is defined, if the column is character data or text data type. 
        /// Otherwise, this column is NULL.
        /// </summary>
        public string CollationCatalog { get; set; }

        public string CollationSchema { get; set; }

        public string CollationName { get; set; }

        public string DomainCatalog { get; set; }

        public string DomainName { get; set; }

        public string Description { get; set; }

        public string EDMType { get; set; }

        /// <summary>
        /// Default value of the column
        /// </summary>
        object IDbProviderColumn.ColumnDefault { get; set; }

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

            set => mDataType = value;
        }

        /// <summary>
        /// The SQL data type
        /// </summary>
        string IDbProviderColumn.SQLTypeName { get => DataType; set => DataType = value; }

        /// <summary>
        /// A flag indicating whether the column is a primary key or not.
        /// </summary>
        bool IDbProviderColumn.IsPrimaryKey { get; set; }

        /// <summary>
        /// A flag indicating whether the column has unique values or not.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// A flag indicating whether the value is auto increment or not.
        /// </summary>
        public bool IsAutoIncrement { get; set; }

        /// <summary>
        /// The column value that will be used when the next row is inserted if <see cref="IsAutoIncrement"/> is true.
        /// </summary>
        uint? IDbProviderColumn.NextAutoIncrementValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLiteProviderColumn(DataRow row) : base()
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            ColumnName = row.GetString(3);
            ColumnGUID = row.GetDbNullableString(4);
            ColumnPropID = row.GetDbNullableLong(5);
            OrdinalPosition = row.GetInt(6);
            ColumnHasDefault = row.GetBool(7);
            ColumnDefault = row.GetDbNullableString(8);
            ColumnFlags = row.GetDbNullableString(9);
            DataType = row.GetString(11);
            IsNullable = row.GetBool(10);
            TypeGUID = row.GetDbNullableString(12);
            CharacterMaximumLength = row.GetInt(13);
            CharacterOctetLength = row.GetDbNullableInt(14);
            NumericPrecision = row.GetDbNullableLong(15);
            NumericScale = row.GetDbNullableLong(16);
            DateTimePrecision = row.GetDbNullableLong(17);
            CharacterSetCatalog = row.GetDbNullableString(18);
            CharacterSetSchema = row.GetDbNullableString(19);
            CharacterSetName = row.GetDbNullableString(20);
            CollationCatalog = row.GetDbNullableString(21);
            CollationSchema = row.GetDbNullableString(22);
            CollationName = row.GetString(23);
            DomainCatalog = row.GetDbNullableString(24);
            DomainName = row.GetDbNullableString(25);
            Description = row.GetDbNullableString(26);
            EDMType = row.GetString(28);
            IsAutoIncrement = row.GetBool(29);
            IsUnique = row.GetBool(30);
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
            => HashCodeHelpers.Combine(TableCatalog, TableSchema, TableName, ColumnName, OrdinalPosition, ColumnDefault, IsNullable, DataType, ColumnGUID,
                ColumnPropID, ColumnHasDefault, ColumnFlags, TypeGUID, CharacterMaximumLength, CharacterOctetLength, NumericPrecision, NumericScale, DateTimePrecision,
                CharacterSetCatalog, CharacterSetSchema, CharacterSetName, CollationCatalog, CollationSchema,
                CollationName, DomainCatalog, DomainName, Description, EDMType, IsUnique, IsAutoIncrement);

        #endregion
    }
}
