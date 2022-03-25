using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents SQL Server view
    /// </summary>
    public class SQLServerProviderView
    {
        #region Public Properties

        /// <summary>
        /// Catalog of the view.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the view.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// View name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Type of WITH CHECK OPTION.Is CASCADE if the original view was created using the WITH CHECK OPTION.Otherwise, NONE is returned.
        /// </summary>
        public string CheckOption { get; set; }

        /// <summary>
        /// Specifies whether the view is updatable.Always returns NO.
        /// </summary>
        public bool IsUpdatable { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderView(DataRow row) : base()
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            CheckOption = row.GetString(3);
            IsUpdatable = row.GetBool(4);
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
