using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a PostgreSQL data type
    /// </summary>
    internal class PostgreSQLProviderDataType : DbProviderDataType
    {
        #region Public Properties

        /// <summary>
        /// true – the data type is updated by the database every time the row is changed and the value of the column is different from all previous values
        /// false – the data type is note updated by the database every time the row is changed
        /// </summary>
        public bool IsConcurrencyType { get; set; }

        /// <summary>
        /// true – the data type can be expressed as a literal
        /// false – the data type can not be expressed as a literal
        /// </summary>
        public bool IsLiteralSupported { get; set; }

        /// <summary>
        /// The prefix applied to a given literal.
        /// </summary>
        public string LiteralPrefix { get; set; }

        /// <summary>
        /// The suffix applied to a given literal.
        /// </summary>
        public string LiteralSuffix { get; set; }

        /// <summary>
        /// If the type indicator is a numeric type, this is the maximum number of digits allowed to the right of the decimal point.
        /// </summary>
        public short? MaximumScale { get; set; }

        /// <summary>
        /// If the type indicator is a numeric type, this is the minimum number of digits allowed to the right of the decimal point.
        /// </summary>
        public short? MinimumScale { get; set; }

        /// <summary>
        /// The .NET data type
        /// </summary>
        public string NativeDataType { get; set; }

        /// <summary>
        /// The unique id for provider data type
        /// </summary>
        public uint OID { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal PostgreSQLProviderDataType(DataRow row) : base()
        {
            TypeName = row.GetString(0);
            ColumnSize = row.GetDbNullableLong(1);
            CreateFormat = row.GetString(2);
            CreateParameters = row.GetString(3);
            DataType = row.GetDbNullableString(4);
            IsAutoIncrementable = row.GetDbNullableBool(5);
            IsBestMatch = row.GetDbNullableBool(6);
            IsCaseSensitive = row.GetDbNullableBool(7);
            IsConcurrencyType = row.GetBool(8);
            IsFixedLength = row.GetDbNullableBool(9);
            IsFixedPrecisionScale = row.GetDbNullableBool(10);
            IsLiteralSupported = row.GetBool(11);
            IsLong = row.GetDbNullableBool(12);
            IsNullable = row.GetDbNullableBool(13);
            IsSearchable = row.GetDbNullableBool(14);
            IsSearchableWithLike = row.GetDbNullableBool(15);
            IsUnsigned = row.GetDbNullableBool(16);
            LiteralPrefix = row.GetDbNullableString(17);
            LiteralSuffix = row.GetDbNullableString(18);
            MaximumScale = row.GetDbNullableShort(19);
            MinimumScale = row.GetDbNullableShort(20);
            NativeDataType = row.GetDbNullableString(21);
            ProviderDbType = row.GetDbNullableInt(22);
            OID = row.GetUnsignedInt(23);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => TypeName;

        #endregion
    }
}
