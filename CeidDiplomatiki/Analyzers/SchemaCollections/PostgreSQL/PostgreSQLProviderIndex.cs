using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a PostgreSQL provider index
    /// </summary>
    public class PostgreSQLProviderIndex
    {
        #region Public Properties

        /// <summary>
        /// The table catalog that contains the index
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// The table schema that contains the index
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// The table name that contains the index
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The index name
        /// </summary>
        public string IndexName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal PostgreSQLProviderIndex(DataRow row) : base()
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            IndexName = row.GetString(3);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => IndexName;

        #endregion
    }
}
