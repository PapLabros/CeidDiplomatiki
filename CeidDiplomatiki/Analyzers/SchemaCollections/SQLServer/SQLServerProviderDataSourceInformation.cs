using System.Data;
using System.Data.Common;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represent a SQL Server data source information
    /// </summary>
    public class SQLServerProviderDataSourceInformation
    {
        #region Public Properties

        /// <summary>
        /// The regular expression to match the composite separators in a composite identifier. For example, "\." (for SQL Server) or "@|\." (for Oracle).
        /// A composite identifier is typically what is used for a database object name, for example: pubs.dbo.authors or pubs @dbo.authors.
        /// For SQL Server, use the regular expression "\.". For OracleClient, use "@|\.".
        /// For ODBC use the Catalog_name_seperator.
        /// For OLE DB use DBLITERAL_CATALOG_SEPARATOR or DBLITERAL_SCHEMA_SEPARATOR.
        /// </summary>
        public string CompositeIdentifierSeparatorPattern { get; set; }

        /// <summary>
        /// The name of the product accessed by the provider, such as "Oracle" or "SQLServer".
        /// </summary>
        public string DataSourceProductName { get; set; }

        /// <summary>
        /// Indicates the version of the product accessed by the provider, in the data sources native format and not in Microsoft format.
        /// In some cases DataSourceProductVersion and DataSourceProductVersionNormalized will be the same value.In the case of OLE DB and ODBC, these will always be the same as they are mapped to the same function call in the underlying native API.
        /// </summary>
        public string DataSourceProductVersion { get; set; }

        /// <summary>
        /// A normalized version for the data source, such that it can be compared with String.Compare(). The format of this is consistent for all versions of the provider to prevent version 10 from sorting between version 1 and version 2.
        /// For example, the Oracle provider uses a format of "nn.nn.nn.nn.nn" for its normalized version, which causes an Oracle 8i data source to return "08.01.07.04.01". SQL Server uses the typical Microsoft "nn.nn.nnnn" format.
        /// In some cases, DataSourceProductVersion and DataSourceProductVersionNormalized will be the same value.In the case of OLE DB and ODBC these will always be the same as they are mapped to the same function call in the underlying native API.
        /// </summary>
        public string DataSourceProductVersionNormalized { get; set; }

        /// <summary>
        /// Specifies the relationship between the columns in a GROUP BY clause and the non-aggregated columns in the select list.
        /// </summary>
        public GroupByBehavior GroupByBehavior { get; set; }

        /// <summary>
        /// A regular expression that matches an identifier and has a match value of the identifier. For example "[A-Za-z0-9_#$]".
        /// </summary>
        public string IdentifierPattern { get; set; }

        /// <summary>
        /// Indicates whether non-quoted identifiers are treated as case sensitive or not.
        /// </summary>
        public IdentifierCase IdentifierCase { get; set; }

        /// <summary>
        /// Specifies whether columns in an ORDER BY clause must be in the select list. A value of true indicates that they are required to be in the select list, a value of false indicates that they are not required to be in the select list.
        /// </summary>
        public bool OrderByColumnsInSelect { get; set; }

        /// <summary>
        /// A format string that represents how to format a parameter.
        /// If named parameters are supported by the data source, the first placeholder in this string should be where the parameter name should be formatted.
        /// For example, if the data source expects parameters to be named and prefixed with an ':' this would be ":{0}". When formatting this with a parameter name of "p1" the resulting string is ":p1".
        /// If the data source expects parameters to be prefixed with the '@', but the names already include them, this would be '{0}', and the result of formatting a parameter named "@p1" would simply be "@p1".
        /// For data sources that do not expect named parameters and expect the use of the '?' character, the format string can be specified as simply '?', which would ignore the parameter name. For OLE DB we return '?'.
        /// </summary>
        public string ParameterMarkerFormat { get; set; }

        /// <summary>
        /// A regular expression that matches a parameter marker. It will have a match value of the parameter name, if any.
        /// For example, if named parameters are supported with an '@' lead-in character that will be included in the parameter name, this would be: "(@[A-Za-z0-9_#]*)".   
        /// However, if named parameters are supported with a ':' as the lead-in character and it is not part of the parameter name, this would be: ":([A-Za-z0-9_#]*)".
        /// Of course, if the data source doesn't support named parameters, this would simply be "?".
        /// </summary>
        public string ParameterMarkerPattern { get; set; }

        /// <summary>
        /// The maximum length of a parameter name in characters. Visual Studio expects that if parameter names are supported, the minimum value for the maximum length is 30 characters.
        /// If the data source does not support named parameters, this property returns zero.
        /// </summary>
        public int ParameterNameMaxLength { get; set; }

        /// <summary>
        /// A regular expression that matches the valid parameter names. Different data sources have different rules regarding the characters that may be used for parameter names.
        /// Visual Studio expects that if parameter names are supported, the characters "\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}" are the minimum supported set of characters that are valid for parameter names.
        /// </summary>
        public string ParameterNamePattern { get; set; }

        /// <summary>
        /// A regular expression that matches a quoted identifier and has a match value of the identifier itself without the quotes. 
        /// For example, if the data source used double-quotes to identify quoted identifiers, this would be: "(([^\"]|\"\")*)".
        /// </summary>
        public string QuotedIdentifierPattern { get; set; }

        /// <summary>
        /// Indicates whether quoted identifiers are treated as case sensitive or not.
        /// </summary>
        public IdentifierCase QuotedIdentifierCase { get; set; }

        /// <summary>
        /// A regular expression that matches the statement separator.
        /// </summary>
        public string StatementSeparatorPattern { get; set; }

        /// <summary>
        /// A regular expression that matches a string literal and has a match value of the literal itself. For example, if the data source used single-quotes to identify strings, this would be: "('([^']|'')*')"'
        /// </summary>
        public string StringLiteralPattern { get; set; }

        /// <summary>
        /// A regular expression that matches a string literal and has a match value of the literal itself. For example, if the data source used single-quotes to identify strings, this would be: "('([^']|'')*')"'
        /// </summary>
        public SupportedJoinOperators SupportedJoinOperators { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderDataSourceInformation(DataRow row) : base()
        {
            CompositeIdentifierSeparatorPattern = row.GetString(0);
            DataSourceProductName = row.GetString(1);
            DataSourceProductVersion = row.GetString(2);
            DataSourceProductVersionNormalized = row.GetString(3);
            GroupByBehavior = (GroupByBehavior)row.GetInt(4);
            IdentifierPattern = row.GetString(5);
            IdentifierCase = (IdentifierCase)row.GetInt(6);
            OrderByColumnsInSelect = row.GetBool(7);
            ParameterMarkerFormat = row.GetString(8);
            ParameterMarkerPattern = row.GetString(9);
            ParameterNameMaxLength = row.GetInt(10);
            ParameterNamePattern = row.GetString(11);
            QuotedIdentifierPattern = row.GetString(12);
            QuotedIdentifierCase = (IdentifierCase)row.GetInt(13);
            StatementSeparatorPattern = row.GetString(14);
            StatementSeparatorPattern = row.GetString(15);
            SupportedJoinOperators = (SupportedJoinOperators)row.GetInt(16);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => DataSourceProductName;

        #endregion
    }
}
