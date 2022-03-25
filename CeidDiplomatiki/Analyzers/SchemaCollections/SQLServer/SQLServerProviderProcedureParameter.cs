using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server procedure parameter
    /// </summary>
    public class SQLServerProviderProcedureParameter
    {
        #region Public Properties

        /// <summary>
        /// Catalog name of the procedure for which this is a parameter.
        /// </summary>
        public string SpecificCatalog { get; set; }

        /// <summary>
        /// Schema that contains the procedure for which this parameter is part of.
        /// </summary>
        public string SpecificSchema { get; set; }

        /// <summary>
        /// Name of the procedure for which this parameter is a part of.
        /// </summary>
        public string SpecificName { get; set; }

        /// <summary>
        /// Ordinal position of the parameter starting at 1. 
        /// For the return value of a procedure, this is a 0.
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Returns IN if an input parameter, OUT if an output parameter, and INOUT if an input/output parameter.
        /// </summary>
        public string ParameterMode { get; set; }

        /// <summary>
        /// Returns YES if indicates result of the procedure that is a function. 
        /// Otherwise, returns NO.
        /// </summary>
        public bool IsResult { get; set; }

        /// <summary>
        /// Returns YES if declared as locator.
        /// Otherwise, returns NO.
        /// </summary>
        public bool AsLocator { get; set; }

        /// <summary>
        /// Name of the parameter. 
        /// NULL if this corresponds to the return value of a function.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// System-supplied data type.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Maximum length in characters for binary or character data types.
        /// Otherwise, returns NULL.
        /// </summary>
        public int? CharacterMaximumLength { get; set; }

        /// <summary>
        /// Maximum length, in bytes, for binary or character data types.
        /// Otherwise, returns NULL.
        /// </summary>
        public int? CharacterOctetLength { get; set; }

        /// <summary>
        /// Catalog name of the collation of the parameter. 
        /// If not one of the character types, returns NULL.
        /// </summary>
        public string CollationCatalog { get; set; }

        /// <summary>
        /// Always returns NULL.
        /// </summary>
        public string CollationSchema { get; set; }

        /// <summary>
        /// Name of the collation of the parameter.
        /// If not one of the character types, returns NULL.
        /// </summary>
        public string CollationName { get; set; }

        /// <summary>
        /// Catalog name of the character set of the parameter.
        /// If not one of the character types, returns NULL.
        /// </summary>
        public string CharacterSetCatalog { get; set; }

        /// <summary>
        /// Always returns NULL.
        /// </summary>
        public string CharacterSetSchema { get; set; }

        /// <summary>
        /// Name of the character set of the parameter. 
        /// If not one of the character types, returns NULL.
        /// </summary>
        public string CharacterSetName { get; set; }

        /// <summary>
        /// Precision of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, returns NULL.
        /// </summary>
        public byte? NumericPrecision { get; set; }

        /// <summary>
        /// Precision radix of approximate numeric data, exact numeric data, integer data, or monetary data.Otherwise, returns NULL.
        /// </summary>
        public short? NumericPrecisionRadix { get; set; }

        /// <summary>
        /// Scale of approximate numeric data, exact numeric data, integer data, or monetary data.
        /// Otherwise, returns NULL.
        /// </summary>
        public int? NumericScale { get; set; }

        /// <summary>
        /// Precision in fractional seconds if the parameter type is date-time or small date-time.
        /// Otherwise, returns NULL.
        /// </summary>
        public short? DatetimePrecision { get; set; }

        /// <summary>
        /// NULL.Reserved for future use by SQL Server.
        /// </summary>
        public string IntervalType { get; set; }

        /// <summary>
        /// NULL.Reserved for future use by SQL Server.
        /// </summary>
        public short? IntervalPrecision { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderProcedureParameter(DataRow row) : base()
        {
            SpecificCatalog = row.GetString(0);
            SpecificSchema = row.GetString(1);
            SpecificName = row.GetString(2);
            OrdinalPosition = row.GetInt(3);
            ParameterMode = row.GetString(4);
            IsResult = row.GetBool(5);
            AsLocator = row.GetBool(6);
            ParameterName = row.GetString(7);
            DataType = row.GetString(8);
            CharacterMaximumLength = row.GetDbNullableInt(9);
            CharacterOctetLength = row.GetDbNullableInt(10);
            CollationCatalog = row.GetString(11);
            CollationSchema = row.GetString(12);
            CollationName = row.GetString(13);
            CharacterSetCatalog = row.GetString(14);
            CharacterSetSchema = row.GetString(15);
            CharacterSetName = row.GetString(16);
            NumericPrecision = row.GetDbNullableByte(17);
            NumericPrecisionRadix = row.GetDbNullableShort(18);
            NumericScale = row.GetDbNullableInt(19);
            DatetimePrecision = row.GetDbNullableShort(20);
            IntervalType = row.GetString(21);
            IntervalPrecision = row.GetDbNullableShort(22);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => SpecificName;

        #endregion
    }
}
