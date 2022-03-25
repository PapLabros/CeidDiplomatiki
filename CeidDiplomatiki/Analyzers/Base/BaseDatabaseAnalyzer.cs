using Atom.Core;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The base for all the database analyzers
    /// </summary>
    public abstract class BaseDatabaseAnalyzer<TDbConnection> : IDatabaseAnalyzer
        where TDbConnection : DbConnection
    {
        #region Private Members

        /// <summary>
        /// The lazy property for <see cref="ProviderDataTypes"/>
        /// </summary>
        private readonly Lazy<IEnumerable<DbProviderDataType>> mProviderDataTypes;

        #endregion

        #region Public Properties

        /// <summary>
        /// The connection string
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// The database provider
        /// </summary>
        public abstract SQLDatabaseProvider Provider { get; }

        /// <summary>
        /// The connection
        /// </summary>
        public TDbConnection Connection { get; }

        /// <summary>
        /// The provider data types
        /// </summary>
        public IEnumerable<DbProviderDataType> ProviderDataTypes => mProviderDataTypes.Value;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public BaseDatabaseAnalyzer(string connectionString) : base()
        {
            // If the connection string is null or empty...
            if (string.IsNullOrEmpty(connectionString))
                // Throw an exception
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.", nameof(connectionString));

            // Create a connection
            Connection = CreateConnection(connectionString);
            ConnectionString = connectionString;

            // Lazy initialize the provider data type
            mProviderDataTypes = new Lazy<IEnumerable<DbProviderDataType>>(() => GetDataTypes().Result);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Provider.ToString();

        /// <summary>
        /// Creates a database connection using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        public abstract TDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Creates a database connection using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        DbConnection IDatabaseAnalyzer.CreateConnection(string connectionString) => CreateConnection(connectionString);

        /// <summary>
        /// Get the database analysis for the database with the specified <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">The database name</param>
        /// <returns></returns>
        public IFailable<DatabaseAnalysisDataModel> GetDatabaseAnalysis(string databaseName)
        {
            // Get the databases
            var databaseResult = GetDatabases();

            // If not successful...
            if (!databaseResult.Successful)
                // Return the unsuccessful result
                return new Failable<DatabaseAnalysisDataModel>()
                {
                    ErrorMessage = databaseResult.ErrorMessage,
                    ErrorType = databaseResult.ErrorType
                };

            // Get the database by name
            var database = databaseResult.Result.FirstOrDefault(x => x.DatabaseName == databaseName);

            // If such a database doesn't exist...
            if (database == null)
                // Return the unsuccessful result
                return new Failable<DatabaseAnalysisDataModel>()
                {
                    ErrorMessage = $"Database by {databaseName} doesn't exist."
                };

            // Get the tables
            var tableResult = GetTables(databaseName);

            // If not successful...
            if (!tableResult.Successful)
                // Return the unsuccessful result
                return new Failable<DatabaseAnalysisDataModel>()
                {
                    ErrorMessage = tableResult.ErrorMessage,
                    ErrorType = tableResult.ErrorType
                };

            // Declare a list for the indexes
            var indexes = new List<IDbProviderIndex>();

            // Declare a list for the foreign keys
            var foreignKeys = new List<IDbProviderForeignKeyColumn>();

            // Declare a list for the tables
            var tables = new List<TableAnalysisDataModel>();

            // For every table...
            foreach (var table in tableResult.Result)
            {
                // Get the foreign keys that are defined on the table
                var tableForeignKeysResult = GetForeignKeyColumns(databaseName, table.TableName);

                // If not successful...
                if (!tableForeignKeysResult.Successful)
                    // Return the unsuccessful result
                    return new Failable<DatabaseAnalysisDataModel>()
                    {
                        ErrorMessage = tableForeignKeysResult.ErrorMessage,
                        ErrorType = tableForeignKeysResult.ErrorType
                    };

                // Add the foreign keys
                foreignKeys.AddRange(tableForeignKeysResult.Result);

                // Get the indexes that are defined on the table
                var tableIndexesResult = GetIndexColumns(databaseName, table.TableName);

                // If not successful...
                if (!tableIndexesResult.Successful)
                    // Return the unsuccessful result
                    return new Failable<DatabaseAnalysisDataModel>()
                    {
                        ErrorMessage = tableIndexesResult.ErrorMessage,
                        ErrorType = tableIndexesResult.ErrorType
                    };

                // Add the indexes
                indexes.AddRange(tableIndexesResult.Result);

                // Get the columns that are defined on the table
                var tableColumnsResult = GetColumns(databaseName, table.TableName);

                // If not successful...
                if (!tableColumnsResult.Successful)
                    // Return the unsuccessful result
                    return new Failable<DatabaseAnalysisDataModel>()
                    {
                        ErrorMessage = tableColumnsResult.ErrorMessage,
                        ErrorType = tableColumnsResult.ErrorType
                    };

                // Add the table entry
                tables.Add(new TableAnalysisDataModel(table, tableColumnsResult.Result));
            }

            // Return the successful result
            return new Failable<DatabaseAnalysisDataModel>()
            { 
                Result = new DatabaseAnalysisDataModel(database, tables, foreignKeys, indexes)
            };
        }

        /// <summary>
        /// Get the database analysis for the database with the specified <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">The database name</param>
        /// <returns></returns>
        public async Task<IFailable<DatabaseAnalysisDataModel>> GetDatabaseAnalysisAsync(string databaseName)
        {
            // Get the databases
            var databaseResult = await GetDatabasesAsync();

            // If not successful...
            if (!databaseResult.Successful)
                // Return the unsuccessful result
                return new Failable<DatabaseAnalysisDataModel>()
                {
                    ErrorMessage = databaseResult.ErrorMessage,
                    ErrorType = databaseResult.ErrorType
                };

            // Get the database by name
            var database = databaseResult.Result.FirstOrDefault(x => x.DatabaseName == databaseName);

            // If such a database doesn't exist...
            if (database == null)
                // Return the unsuccessful result
                return new Failable<DatabaseAnalysisDataModel>()
                {
                    ErrorMessage = $"Database by {databaseName} doesn't exist."
                };

            // Get the tables
            var tableResult = await GetTablesAsync(databaseName);

            // If not successful...
            if (!tableResult.Successful)
                // Return the unsuccessful result
                return new Failable<DatabaseAnalysisDataModel>()
                {
                    ErrorMessage = tableResult.ErrorMessage,
                    ErrorType = tableResult.ErrorType
                };

            // Declare a list for the indexes
            var indexes = new List<IDbProviderIndex>();

            // Declare a list for the foreign keys
            var foreignKeys = new List<IDbProviderForeignKeyColumn>();

            // Declare a list for the tables
            var tables = new List<TableAnalysisDataModel>();

            // For every table...
            foreach (var table in tableResult.Result)
            {
                // Get the foreign keys that are defined on the table
                var tableForeignKeysResult = await GetForeignKeyColumnsAsync(databaseName, table.TableName);

                // If not successful...
                if (!tableForeignKeysResult.Successful)
                    // Return the unsuccessful result
                    return new Failable<DatabaseAnalysisDataModel>()
                    {
                        ErrorMessage = tableForeignKeysResult.ErrorMessage,
                        ErrorType = tableForeignKeysResult.ErrorType
                    };

                // Add the foreign keys
                foreignKeys.AddRange(tableForeignKeysResult.Result);

                // Get the indexes that are defined on the table
                var tableIndexesResult = await GetIndexColumnsAsync(databaseName, table.TableName);

                // If not successful...
                if (!tableIndexesResult.Successful)
                    // Return the unsuccessful result
                    return new Failable<DatabaseAnalysisDataModel>()
                    {
                        ErrorMessage = tableIndexesResult.ErrorMessage,
                        ErrorType = tableIndexesResult.ErrorType
                    };

                // Add the indexes
                indexes.AddRange(tableIndexesResult.Result);

                // Get the columns that are defined on the table
                var tableColumnsResult = await GetColumnsAsync(databaseName, table.TableName);

                // If not successful...
                if (!tableColumnsResult.Successful)
                    // Return the unsuccessful result
                    return new Failable<DatabaseAnalysisDataModel>()
                    {
                        ErrorMessage = tableColumnsResult.ErrorMessage,
                        ErrorType = tableColumnsResult.ErrorType
                    };

                // Add the table entry
                tables.Add(new TableAnalysisDataModel(table, tableColumnsResult.Result));
            }

            // Return the successful result
            return new Failable<DatabaseAnalysisDataModel>()
            {
                Result = new DatabaseAnalysisDataModel(database, tables, foreignKeys, indexes)
            };
        }

        /// <summary>
        /// Gets the meta data collections from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public virtual IFailable<IEnumerable<DbProviderMetaDataCollection>> GetMetaData()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<DbProviderMetaDataCollection>>()
                {
                    Result = Connection.GetSchema("MetaDataCollections").Rows.OfType<DataRow>().Select(x => new DbProviderMetaDataCollection(x)).ToList()
                }; 
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<DbProviderMetaDataCollection>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the meta data collections from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<DbProviderMetaDataCollection>>> GetMetaDataAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<DbProviderMetaDataCollection>(() => GetMetaData());

        /// <summary>
        /// Gets the data types from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public virtual IFailable<IEnumerable<DbProviderDataType>> GetDataTypes()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<DbProviderDataType>>()
                {
                    Result = Connection.GetSchema("DataTypes").Rows.OfType<DataRow>().Select(x => new DbProviderDataType(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<DbProviderDataType>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the data types from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<DbProviderDataType>>> GetDataTypesAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<DbProviderDataType>(() => GetDataTypes());

        /// <summary>
        /// Gets the restrictions from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public virtual IFailable<IEnumerable<DbProviderRestriction>> GetRestrictions()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<DbProviderRestriction>>()
                {
                    Result = Connection.GetSchema("Restrictions").Rows.OfType<DataRow>().Select(x => new DbProviderRestriction(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<DbProviderRestriction>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the restrictions from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<DbProviderRestriction>>> GetRestrictionsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<DbProviderRestriction>(() => GetRestrictions());

        /// <summary>
        /// Gets the databases from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public abstract IFailable<IEnumerable<IDbProviderDatabase>> GetDatabases();

        /// <summary>
        /// Gets the databases from the server with the specified established <see cref="Connection"/>
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<IDbProviderDatabase>>> GetDatabasesAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<IDbProviderDatabase>(() => GetDatabases());

        /// <summary>
        /// Gets the tables from the server with the specified established <see cref="Connection"/>
        /// and the database with the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <returns></returns>
        public abstract IFailable<IEnumerable<IDbProviderTable>> GetTables(string databaseName);
        
        /// <summary>
        /// Gets the tables from the server with the specified established <see cref="Connection"/>
        /// and the database with the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<IDbProviderTable>>> GetTablesAsync(string databaseName)
            => AnalyzerHelpers.GetDatabaseCollectionAsync<IDbProviderTable>(() => GetTables(databaseName));

        /// <summary>
        /// Gets the columns from the server with the specified established <see cref="Connection"/>,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public abstract IFailable<IEnumerable<IDbProviderColumn>> GetColumns(string databaseName, string tableName);

        /// <summary>
        /// Gets the columns from the server with the specified established <see cref="Connection"/>,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<IDbProviderColumn>>> GetColumnsAsync(string databaseName, string tableName)
            => AnalyzerHelpers.GetDatabaseCollectionAsync<IDbProviderColumn>(() => GetColumns(databaseName, tableName));

        /// <summary>
        /// Gets the index columns from the server with the specified established <see cref="Connection"/>,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public abstract IFailable<IEnumerable<IDbProviderIndex>> GetIndexColumns(string databaseName, string tableName);

        /// <summary>
        /// Gets the index columns from the server with the specified established <see cref="Connection"/>,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<IDbProviderIndex>>> GetIndexColumnsAsync(string databaseName, string tableName)
            => AnalyzerHelpers.GetDatabaseCollectionAsync<IDbProviderIndex>(() => GetIndexColumns(databaseName, tableName));

        /// <summary>
        /// Gets the foreign key columns from the server with the specified established <see cref="Connection"/>,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table that contains the foreign key</param>
        /// <returns></returns>
        public abstract IFailable<IEnumerable<IDbProviderForeignKeyColumn>> GetForeignKeyColumns(string databaseName, string tableName);

        /// <summary>
        /// Gets the foreign key columns from the server with the specified established <see cref="Connection"/>,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table that contains the foreign key</param>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<IDbProviderForeignKeyColumn>>> GetForeignKeyColumnsAsync(string databaseName, string tableName)
            => AnalyzerHelpers.GetDatabaseCollectionAsync<IDbProviderForeignKeyColumn>(() => GetForeignKeyColumns(databaseName, tableName));

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns a <see cref="DbContext"/> using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected abstract DbContext CreateDbContext(string connectionString);

        #endregion
    }

    /// <summary>
    /// The available SQL database providers
    /// </summary>
    public enum SQLDatabaseProvider
    {
        SQLite = 0,
        SQLServer = 1,
        MySQL = 2,
        PostgreSQL = 3
    }
}
