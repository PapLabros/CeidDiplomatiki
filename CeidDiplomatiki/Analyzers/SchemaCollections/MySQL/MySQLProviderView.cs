using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL view
    /// </summary>
    public class MySQLProviderView
    {
        #region Public Properties

        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ViewDefinition { get; set; }

        public string CheckOption { get; set; }

        public bool IsUpdatable { get; set; }

        public string Definer { get; set; }

        public string SecurityType { get; set; }

        public string CharacterSetClient { get; set; }

        public string CollationConnection { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal MySQLProviderView(DataRow row)
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            ViewDefinition = row.GetString(3);
            CheckOption = row.GetString(4);
            IsUpdatable = row.GetBool(5);
            Definer = row.GetString(6);
            SecurityType = row.GetString(7);
            CharacterSetClient = row.GetString(8);
            CollationConnection = row.GetString(9);
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
