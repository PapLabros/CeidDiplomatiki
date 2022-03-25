using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents PostgreSQL provider schemata
    /// </summary>
    public class PostgreSQLProviderSchemata
    {
        #region Public Properties

        /// <summary>
        /// The catalog name
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// The schema name
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// The schema owner
        /// </summary>
        public string SchemaOwner { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal PostgreSQLProviderSchemata(DataRow row) : base()
        {
            CatalogName = row.GetString(0);
            SchemaName = row.GetString(1);
            SchemaOwner = row.GetString(2);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => SchemaName;

        #endregion
    }
}
