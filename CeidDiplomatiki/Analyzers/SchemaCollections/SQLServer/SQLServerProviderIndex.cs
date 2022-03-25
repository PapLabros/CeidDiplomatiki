using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server index
    /// </summary>
    public class SQLServerProviderIndex
    {
        #region Public Properties

        /// <summary>
        /// Catalog that index belongs to.
        /// </summary>
        public string ConstraintCatalog { get; set; }

        /// <summary>
        /// Schema that contains the index.
        /// </summary>
        public string ConstraintSchema { get; set; }

        /// <summary>
        /// Name of the index.
        /// </summary>
        public string ConstraintName { get; set; }

        /// <summary>
        /// Table name the index is associated with.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table the index is associated with.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// Table Name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Index Name.
        /// </summary>
        public string IndexName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderIndex(DataRow row) : base()
        {
            ConstraintCatalog = row.GetString(0);
            ConstraintSchema = row.GetString(1);
            ConstraintName = row.GetString(2);
            TableCatalog = row.GetString(3);
            TableSchema = row.GetString(4);
            TableName = row.GetString(5);
            IndexName = row.GetString(6);
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
