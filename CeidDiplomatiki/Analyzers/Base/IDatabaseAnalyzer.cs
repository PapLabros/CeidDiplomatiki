using Atom.Core;

using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Exposes the required methods and properties for a database analyzer
    /// </summary>
    public interface IDatabaseAnalyzer
    {
        #region Properties

        /// <summary>
        /// The database provider
        /// </summary>
        SQLDatabaseProvider Provider { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a database connection using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Get the database analysis for the database with the specified <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">The database name</param>
        /// <returns></returns>
        IFailable<DatabaseAnalysisDataModel> GetDatabaseAnalysis(string databaseName);
        
        /// <summary>
        /// Get the database analysis for the database with the specified <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">The database name</param>
        /// <returns></returns>
        Task<IFailable<DatabaseAnalysisDataModel>> GetDatabaseAnalysisAsync(string databaseName);

        /// <summary>
        /// Gets the meta data collections from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        IFailable<IEnumerable<DbProviderMetaDataCollection>> GetMetaData();
        
        /// <summary>
        /// Gets the meta data collections from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        Task<IFailable<IEnumerable<DbProviderMetaDataCollection>>> GetMetaDataAsync();

        /// <summary>
        /// Gets the data types from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        IFailable<IEnumerable<DbProviderDataType>> GetDataTypes();

        /// <summary>
        /// Gets the data types from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        Task<IFailable<IEnumerable<DbProviderDataType>>> GetDataTypesAsync();

        /// <summary>
        /// Gets the restrictions from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        IFailable<IEnumerable<DbProviderRestriction>> GetRestrictions();

        /// <summary>
        /// Gets the restrictions from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        Task<IFailable<IEnumerable<DbProviderRestriction>>> GetRestrictionsAsync();

        /// <summary>
        /// Gets the databases from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        IFailable<IEnumerable<IDbProviderDatabase>> GetDatabases();

        /// <summary>
        /// Gets the databases from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        Task<IFailable<IEnumerable<IDbProviderDatabase>>> GetDatabasesAsync();

        /// <summary>
        /// Gets the tables from the server with the specified established connection
        /// and the database with the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <returns></returns>
        IFailable<IEnumerable<IDbProviderTable>> GetTables(string databaseName);

        /// <summary>
        /// Gets the tables from the server with the specified established connection
        /// and the database with the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <returns></returns>
        Task<IFailable<IEnumerable<IDbProviderTable>>> GetTablesAsync(string databaseName);

        /// <summary>
        /// Gets the columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        IFailable<IEnumerable<IDbProviderColumn>> GetColumns(string databaseName, string tableName);

        /// <summary>
        /// Gets the columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        Task<IFailable<IEnumerable<IDbProviderColumn>>> GetColumnsAsync(string databaseName, string tableName);

        /// <summary>
        /// Gets the index columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        IFailable<IEnumerable<IDbProviderIndex>> GetIndexColumns(string databaseName, string tableName);

        /// <summary>
        /// Gets the index columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        Task<IFailable<IEnumerable<IDbProviderIndex>>> GetIndexColumnsAsync(string databaseName, string tableName);

        /// <summary>
        /// Gets the foreign key columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table that contains the foreign key</param>
        /// <returns></returns>
        IFailable<IEnumerable<IDbProviderForeignKeyColumn>> GetForeignKeyColumns(string databaseName, string tableName);

        /// <summary>
        /// Gets the foreign key columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table that contains the foreign key</param>
        /// <returns></returns>
        Task<IFailable<IEnumerable<IDbProviderForeignKeyColumn>>> GetForeignKeyColumnsAsync(string databaseName, string tableName);

        #endregion
    }
}
