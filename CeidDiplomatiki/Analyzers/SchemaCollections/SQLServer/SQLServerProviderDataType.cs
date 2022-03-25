using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server data type
    /// </summary>
    internal class SQLServerProviderDataType : DbProviderDataType
    {
        #region Public Properties

        /// <summary>
        /// If the type indicator is a numeric type, this is the maximum number of digits allowed to the right of the decimal point. 
        /// Otherwise, this is DBNull.Value.
        /// </summary>
        public short? MaximumScale { get; set; }

        /// <summary>
        /// If the type indicator is a numeric type, this is the minimum number of digits allowed to the right of the decimal point. 
        /// Otherwise, this is DBNull.Value.
        /// </summary>
        public short? MinimumScale { get; set; }

        /// <summary>
        /// true – the data type is updated by the database every time the row is changed and the value of the column is different from all previous values
        /// false – the data type is note updated by the database every time the row is changed
        /// DBNull.Value – the database does not support this type of data type
        /// </summary>
        public bool? IsConcurrencyType { get; set; }

        /// <summary>
        /// true – the data type can be expressed as a literal
        /// false – the data type can not be expressed as a literal
        /// </summary>
        public bool? IsLiteralSupported { get; set; }

        /// <summary>
        /// The prefix applied to a given literal.
        /// </summary>
        public string LiteralPrefix { get; set; }

        /// <summary>
        /// The suffix applied to a given literal.
        /// </summary>
        public string LiteralSuffix { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderDataType(DataRow row) : base(row)
        {
            MaximumScale = row.GetDbNullableShort(16);
            MinimumScale = row.GetDbNullableShort(17);
            IsConcurrencyType = row.GetDbNullableBool(18);
            IsLiteralSupported = row.GetDbNullableBool(19);
            LiteralPrefix = row.GetDbNullableString(20);
            LiteralSuffix = row.GetDbNullableString(21);
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
