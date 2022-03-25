using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL view column
    /// </summary>
    public class MySQLProviderViewColumn
    {
        #region Public Properties

        public string ViewCatalog { get; set; }

        public string ViewSchema { get; set; }

        public string ViewName { get; set; }

        public string ColumnName { get; set; }

        public ulong OrdinalPosition { get; set; }

        public string ColumnDefault { get; set; }

        public bool IsNullable { get; set; }

        public string DataType { get; set; }

        public ulong? CharacterMaximumLength { get; set; }

        public ulong? CharacterOctetLength { get; set; }

        public ulong? NumericPrecision { get; set; }

        public ulong? NumericScale { get; set; }

        public ulong? DateTimePrecision { get; set; }

        public string CharacterSetName { get; set; }

        public string CollationName { get; set; }

        public string ColumnType { get; set; }

        public string ColumnKey { get; set; }

        public string Extra { get; set; }

        public string Privileges { get; set; }

        public string ColumnComment { get; set; }

        public string GenerationExpression { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal MySQLProviderViewColumn(DataRow row)
        {
            ViewCatalog = row.GetString(0);
            ViewSchema = row.GetString(1);
            ViewName = row.GetString(2);
            ColumnName = row.GetString(3);
            OrdinalPosition = row.GetUnsignedInt(4);
            ColumnDefault = row.GetDbNullableString(5);
            IsNullable = row.GetBool(6);
            DataType = row.GetString(7);
            CharacterMaximumLength = row.GetDbNullableUnsignedLong(8);
            CharacterOctetLength = row.GetDbNullableUnsignedLong(9);
            NumericPrecision = row.GetDbNullableUnsignedLong(10);
            NumericScale = row.GetDbNullableUnsignedLong(11);
            DateTimePrecision = row.GetDbNullableUnsignedLong(12);
            CharacterSetName = row.GetDbNullableString(13);
            CollationName = row.GetDbNullableString(14);
            ColumnType = row.GetString(15);
            ColumnKey = row.GetDbNullableString(16);
            Extra = row.GetDbNullableString(17);
            Privileges = row.GetString(18);
            ColumnComment = row.GetDbNullableString(19);
            GenerationExpression = row.GetDbNullableString(20);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ColumnName;

        #endregion
    }
}
