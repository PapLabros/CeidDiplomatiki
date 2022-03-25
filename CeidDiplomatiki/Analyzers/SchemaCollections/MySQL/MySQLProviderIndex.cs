using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL index.
    /// </summary>
    public class MySQLProviderIndex
    {
        #region Public Properties

        public string IndexCatalog { get; set; }

        public string IndexSchema { get; set; }

        public string IndexName { get; set; }

        public string TableName { get; set; }

        public bool Unique { get; set; }

        public bool Primary { get; set; }

        public string Type { get; set; }

        public string Comment { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal MySQLProviderIndex(DataRow row)
        {
            IndexCatalog = row.GetDbNullableString(0);
            IndexSchema = row.GetString(1);
            IndexName = row.GetString(2);
            TableName = row.GetString(3);
            Unique = row.GetBool(4);
            Primary = row.GetBool(5);
            Comment = row.GetDbNullableString(6);
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
