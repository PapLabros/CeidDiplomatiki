using Atom.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The MySQL implementation of the <see cref="BaseDatabaseAnalyzer{TDbConnection}"/>
    /// </summary>
    public class MySqlDatabaseAnalyzer : BaseDatabaseAnalyzer<MySqlConnection>
    {
        #region Public Properties

        /// <summary>
        /// The database provider
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.MySQL;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public MySqlDatabaseAnalyzer(string connectionString) : base(connectionString)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a database connection using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        public override MySqlConnection CreateConnection(string connectionString) => new MySqlConnection(connectionString);

        #region Common

        /// <summary>
        /// Gets the databases from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public override IFailable<IEnumerable<IDbProviderDatabase>> GetDatabases()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<IDbProviderDatabase>>()
                {
                    Result = Connection.GetSchema("Databases").Rows.OfType<DataRow>().Select(x => new MySQLProviderDatabase(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<IDbProviderDatabase>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the tables from the server with the specified established connection
        /// and the database with the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <returns></returns>
        public override IFailable<IEnumerable<IDbProviderTable>> GetTables(string databaseName)
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<IDbProviderTable>>()
                {
                    Result = Connection.GetSchema("Tables", new string[4] { null, databaseName, null, null }).Rows.OfType<DataRow>().Select(x => new MySQLProviderTable(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<IDbProviderTable>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public override IFailable<IEnumerable<IDbProviderColumn>> GetColumns(string databaseName, string tableName)
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                var relationalTypeMappingService = CreateDbContext(ConnectionString).GetInfrastructure().GetRequiredService<IRelationalTypeMappingSource>();

                // Return the meta data
                return new Failable<IEnumerable<IDbProviderColumn>>()
                {
                    Result = Connection.GetSchema("Columns", new string[4] { null, databaseName, tableName, null }).Rows.OfType<DataRow>().Select(x =>
                    {
                        // Initialize the column
                        var column = new MySQLProviderColumn(x);

                        // Get the ClrType name
                        var clrTypeName = ProviderDataTypes.FirstOrDefault(y => y.TypeName == column.DataType.ToUpper())?.DataType;

                        // Cast the instance
                        var castedInstance = column.CastTo<IDbProviderColumn>();
                        // Set the column data type
                        castedInstance.DataType = Type.GetType(clrTypeName, false, true);

                        // Set the column data type
                        castedInstance.DataType = relationalTypeMappingService.FindMapping(castedInstance.DataType, column.ColumnType).ClrType;

                        if (castedInstance.IsNullable && castedInstance.DataType.IsValueType)
                        {
                            castedInstance.DataType = typeof(Nullable<>).MakeGenericType(castedInstance.DataType);
                        }

                        // If the column is the primary key and auto increment...
                        if (castedInstance.IsPrimaryKey && castedInstance.IsAutoIncrement)
                            // Set the next auto increment value
                            castedInstance.NextAutoIncrementValue = GetNextAutoIncrementValue(tableName);

                        // Set the database SQL provider data type
                        castedInstance.DbProviderSQLType = GetColumnDbProviderDataType(tableName, column.ColumnName);

                        // Return the column
                        return column;
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<IDbProviderColumn>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the index columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public override IFailable<IEnumerable<IDbProviderIndex>> GetIndexColumns(string databaseName, string tableName)
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<IDbProviderIndex>>()
                {
                    Result = Connection.GetSchema("IndexColumns", new string[4] { null, databaseName, tableName, null })
                    .Rows
                    .OfType<DataRow>()
                    .Select(x => new MySQLProviderIndexColumn(x))
                    .ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<IDbProviderIndex>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the foreign key columns from the server with the specified established connection,
        /// the database with the specified <paramref name="databaseName"/> and the table with the specified <paramref name="tableName"/>.
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="tableName">The name of the table that contains the foreign key</param>
        /// <returns></returns>
        public override IFailable<IEnumerable<IDbProviderForeignKeyColumn>> GetForeignKeyColumns(string databaseName, string tableName)
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<IDbProviderForeignKeyColumn>>()
                {
                    Result = Connection.GetSchema("Foreign Key Columns", new string[4] { null, databaseName, tableName, null }).Rows.OfType<DataRow>().Select(x =>
                    {
                        var foreignKey = new MySQLProviderForeignKeyColumn(x);

                        foreignKey.OnDelete = GetForeignKeyDeleteBehavior(foreignKey.TableName, foreignKey.ConstraintName);

                        return foreignKey;
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<IDbProviderForeignKeyColumn>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        #endregion

        #region MySQL

        /// <summary>
        /// Gets the foreign keys from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<MySQLProviderForeignKey>> GetForeignKeys()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<MySQLProviderForeignKey>>()
                {
                    Result = Connection.GetSchema("Foreign Keys").Rows.OfType<DataRow>().Select(x => new MySQLProviderForeignKey(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<MySQLProviderForeignKey>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }

        }
        /// <summary>
        /// Gets the foreign keys from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<MySQLProviderForeignKey>>> GetForeignKeysAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<MySQLProviderForeignKey>(() => GetForeignKeys());

        /// <summary>
        /// Gets the indexes from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<MySQLProviderIndex>> GetIndexes()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<MySQLProviderIndex>>()
                {
                    Result = Connection.GetSchema("Indexes").Rows.OfType<DataRow>().Select(x => new MySQLProviderIndex(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<MySQLProviderIndex>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        
        /// <summary>
        /// Gets the indexes from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<MySQLProviderIndex>>> GetIndexesAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<MySQLProviderIndex>(() => GetIndexes());

        /// <summary>
        /// Gets the views from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<MySQLProviderView>> GetViews()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<MySQLProviderView>>()
                {
                    Result = Connection.GetSchema("Views").Rows.OfType<DataRow>().Select(x => new MySQLProviderView(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<MySQLProviderView>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        
        /// <summary>
        /// Gets the views from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<MySQLProviderView>>> GetViewsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<MySQLProviderView>(() => GetViews());

        /// <summary>
        /// Gets the view columns from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<MySQLProviderViewColumn>> GetViewColumns()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<MySQLProviderViewColumn>>()
                {
                    Result = Connection.GetSchema("ViewColumns").Rows.OfType<DataRow>().Select(x => new MySQLProviderViewColumn(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<MySQLProviderViewColumn>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        
        /// <summary>
        /// Gets the view columns from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<MySQLProviderViewColumn>>> GetViewColumnsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<MySQLProviderViewColumn>(() => GetViewColumns());

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns a <see cref="DbContext"/> using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override DbContext CreateDbContext(string connectionString)
        {
            return new DbContext(new DbContextOptionsBuilder<DbContext>().UseMySql(connectionString).Options);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the database provider data type for the column <paramref name="columnName"/> in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private string GetColumnDbProviderDataType(string tableName, string columnName)
        {
            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"describe `{tableName}` `{columnName}`";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                //Get the only record
                reader.Read();

                // Return the column type
                return reader.GetString(1);
            }
        }

        /// <summary>
        /// Gets the foreign key on delete behavior for the foreign key <paramref name="foreignKeyName"/> in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <returns></returns>
        private DatabaseForeignKeyDeleteBehavior GetForeignKeyDeleteBehavior(string tableName, string foreignKeyName)
        {
            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SHOW CREATE TABLE `{tableName}`;";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // If the query failed...
                if (!reader.HasRows)
                    // Return cascade
                    return DatabaseForeignKeyDeleteBehavior.Cascade;

                // While there are rows...
                reader.Read();

                // Get the create table command
                var createTableCommand = reader.GetString(1);

                // Get the constraints
                var constrants = createTableCommand.ToUpper().Split("CONSTRAINT").Skip(1);

                // Get the respective foreign key
                var foreignKey = constrants.First(x => x.Contains(foreignKeyName.ToUpper()));

                // Get the on delete behavior
                var onDelete = foreignKey.Split("ON DELETE").Last().Split('\r', '\n', ',').Where(x => !x.IsNullOrEmpty()).First().Trim();

                if (onDelete == "RESTRICT")
                    return DatabaseForeignKeyDeleteBehavior.Restrict;

                if (onDelete == "SET NULL")
                    return DatabaseForeignKeyDeleteBehavior.SetNull;

                if (onDelete == "NO ACTION")
                    return DatabaseForeignKeyDeleteBehavior.NoAction;

                return DatabaseForeignKeyDeleteBehavior.Cascade;
            }
        }

        /// <summary>
        /// Gets the next auto increment value for the primary key in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns></returns>
        private uint? GetNextAutoIncrementValue(string tableName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"SHOW CREATE TABLE `{tableName}`";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // While there are rows...
                while (reader.Read())
                {
                    // Get the create table command
                    var createTableCommand = reader.GetString(1);

                    // If the table does not contain an auto increment primary key...
                    if (!createTableCommand.Contains("AUTO_INCREMENT="))
                        // Return null
                        return null;

                    // Get the current auto increment value
                    var splitedValues = createTableCommand.Split("AUTO_INCREMENT=").ElementAt(1).Split(" ").ElementAt(0);

                    // Return the value
                    return splitedValues.ToUnsignedInt();
                }

                // Return null
                return null;
            }
        }

        /// <summary>
        /// Counts the same number of characters that appear at the <paramref name="pattern"/> and the <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="pattern">The pattern</param>
        /// <returns></returns>
        private int CountSameCharacters(string value, string pattern)
        {
            // Declare the count
            var count = 0;

            // Get the smaller length
            var length = value.Length > pattern.Length ? pattern.Length : value.Length;

            // For every index..
            for (int index = 0; index < length; index++)
                // If the characters are the same
                if (value[index] == pattern[index])
                    // Increase the count
                    count++;

            // Return the count
            return count;
        }

        #endregion
    }
}
