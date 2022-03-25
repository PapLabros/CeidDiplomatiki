using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server view column
    /// </summary>
    public class SQLServerProviderViewColumn
    {
        #region Public Properties

        /// <summary>
        /// Catalog of the view.
        /// </summary>
        public string ViewCatalog { get; set; }

        /// <summary>
        /// Schema that contains the view.
        /// </summary>
        public string ViewSchema { get; set; }

        /// <summary>
        /// View name.
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Catalog of the table that is associated with this view.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table that is associated with this view.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// Name of the table that is associated with the view. Base Table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Column name.
        /// </summary>
        public string ColumnName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderViewColumn(DataRow row) : base()
        {
            ViewCatalog = row.GetString(0);
            ViewSchema = row.GetString(1);
            ViewName = row.GetString(2);
            TableCatalog = row.GetString(3);
            TableSchema = row.GetString(4);
            TableName = row.GetString(5);
            ColumnName = row.GetString(6);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => TableName;

        #endregion
    }
}
