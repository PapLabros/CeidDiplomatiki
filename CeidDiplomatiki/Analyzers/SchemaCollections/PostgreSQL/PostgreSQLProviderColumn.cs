using Atom.Core;

using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a PostgreSQL provider column
    /// </summary>
    internal class PostgreSQLProviderColumn : IDbProviderColumn
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
        /// The SQL column type
        /// </summary>
        public string SQLType { get; set; }

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
        /// If data_type identifies a character or bit string type, the declared maximum length; null for all other data types or if no maximum length was declared.
        /// </summary>
        public int? CharacterMaximumLength { get; set; }

        /// <summary>
        /// If data_type identifies a character type, the maximum possible length in octets (bytes) of a datum; null for all other data types. 
        /// The maximum octet length depends on the declared character maximum length (see above) and the server encoding.
        /// </summary>
        public int? CharacterOctetLength { get; set; }

        /// <summary>
        /// If data_type identifies a numeric type, this column contains the (declared or implicit) precision of the type for this column. The precision indicates the number of significant digits. It can be expressed in decimal (base 10) or binary (base 2) terms, as specified in the column numeric_precision_radix. 
        /// For all other data types, this column is null.
        /// </summary>
        public int? NumericPrecision { get; set; }

        /// <summary>
        /// If data_type identifies a numeric type, this column indicates in which base the values in the columns numeric_precision and numeric_scale are expressed. 
        /// The value is either 2 or 10. For all other data types, this column is null.
        /// </summary>
        public int? NumericPrecisionRadix { get; set; }

        /// <summary>
        /// If data_type identifies an exact numeric type, this column contains the (declared or implicit) scale of the type for this column. The scale indicates the number of significant digits to the right of the decimal point. It can be expressed in decimal (base 10) or binary (base 2) terms, as specified in the column numeric_precision_radix. 
        /// For all other data types, this column is null.
        /// </summary>
        public int? NumericScale { get; set; }

        /// <summary>
        /// If data_type identifies a date, time, timestamp, or interval type, this column contains the (declared or implicit) fractional seconds precision of the type for this column, that is, the number of decimal digits maintained following the decimal point in the seconds value. 
        /// For all other data types, this column is null.
        /// </summary>
        public int? DatetimePrecision { get; set; }

        /// <summary>
        /// Name of the database containing the collation of the column (always the current database), null if default or the data type of the column is not collatable
        /// </summary>
        public string CollationCatalog { get; set; }

        /// <summary>
        /// Column identification number.
        /// </summary>
        public int OrdinalPosition { get; set; }

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
        string IDbProviderColumn.SQLTypeName { get => SQLType; set => SQLType = value; }

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
        /// The column value that will be used when the next row is inserted if <see cref="IDbProviderColumn.IsAutoIncrement"/> is true.
        /// </summary>
        uint? IDbProviderColumn.NextAutoIncrementValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal PostgreSQLProviderColumn(DataRow row) : base()
        {
            var castedInstance = this.CastTo<IDbProviderColumn>();

            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            ColumnName = row.GetString(3);
            OrdinalPosition = row.GetInt(4);
            castedInstance.ColumnDefault = row.GetDbNullableString(5);
            IsNullable = row.GetBool(6);
            SQLType = row.GetString(7);
            CharacterMaximumLength = row.GetDbNullableInt(8);
            CharacterOctetLength = row.GetDbNullableInt(9);
            NumericPrecision = row.GetDbNullableInt(10);
            NumericPrecisionRadix = row.GetDbNullableInt(11);
            NumericScale = row.GetDbNullableInt(12);
            DatetimePrecision = row.GetDbNullableInt(13);
            CollationCatalog = row.GetDbNullableString(17);
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
            => HashCodeHelpers.Combine(TableCatalog, TableSchema, TableName, ColumnName, SQLType, IsNullable, CharacterMaximumLength, CharacterOctetLength, 
                NumericPrecision, NumericPrecisionRadix, NumericScale, DatetimePrecision, CollationCatalog, OrdinalPosition);

        #endregion
    }
}
