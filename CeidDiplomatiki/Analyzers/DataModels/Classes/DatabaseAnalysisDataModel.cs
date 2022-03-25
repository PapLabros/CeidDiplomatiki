using System.Collections.Generic;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The data model that describes the current state of a database 
    /// </summary>
    public class DatabaseAnalysisDataModel
    {
        #region Public Properties

        /// <summary>
        /// The database
        /// </summary>
        public IDbProviderDatabase Database { get; }

        /// <summary>
        /// The foreign keys
        /// </summary>
        public IEnumerable<IDbProviderForeignKeyColumn> ForeignKeys { get; }

        /// <summary>
        /// The indexes
        /// </summary>
        public IEnumerable<IDbProviderIndex> Indexes { get; }

        /// <summary>
        /// The tables
        /// </summary>
        public IEnumerable<TableAnalysisDataModel> Tables { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="database">The database</param>
        /// <param name="foreignKeys">The foreign keys</param>
        /// <param name="indexes">The indexes</param>
        /// <param name="tables">The tables</param>
        public DatabaseAnalysisDataModel(IDbProviderDatabase database, IEnumerable<TableAnalysisDataModel> tables, IEnumerable<IDbProviderForeignKeyColumn> foreignKeys, IEnumerable<IDbProviderIndex> indexes) : base()
        {
            Tables = tables ?? throw new System.ArgumentNullException(nameof(tables));
            Indexes = indexes ?? throw new System.ArgumentNullException(nameof(indexes));
            ForeignKeys = foreignKeys ?? throw new System.ArgumentNullException(nameof(foreignKeys));
            Database = database ?? throw new System.ArgumentNullException(nameof(database));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Database.DatabaseName;

        #endregion
    }
}
