using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL foreign key.
    /// </summary>
    public class MySQLProviderForeignKey
    {
        #region Public Properties

        public string ConstraintCatalog { get; set; }

        public string ConstraintSchema { get; set; }

        public string ConstraintName { get; set; }

        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string MatchOption { get; set; }

        public string UpdateRule { get; set; }

        public string DeleteRule { get; set; }

        public string ReferencedTableCatalog { get; set; }

        public string ReferencedTableSchema { get; set; }

        public string RefenrecedTableName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal MySQLProviderForeignKey(DataRow row)
        {
            ConstraintCatalog = row.GetString(0);
            ConstraintSchema = row.GetString(1);
            ConstraintName = row.GetString(2);
            TableCatalog = row.GetString(3);
            TableSchema = row.GetString(4);
            TableName = row.GetString(5);
            MatchOption = row.GetString(6);
            UpdateRule = row.GetString(7);
            DeleteRule = row.GetString(8);
            ReferencedTableCatalog = row.GetDbNullableString(9);
            ReferencedTableSchema = row.GetDbNullableString(9);
            RefenrecedTableName = row.GetString(10);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ConstraintName;

        #endregion
    }
}
