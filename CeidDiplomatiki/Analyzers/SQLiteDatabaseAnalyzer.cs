using Atom.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The SQLite implementation of the <see cref="BaseDatabaseAnalyzer{TDbConnection}"/>
    /// </summary>
    public class SQLiteDatabaseAnalyzer : BaseDatabaseAnalyzer<SQLiteConnection>
    {
        #region Constants

        public const string SQLiteSequenceTableName = "sqlite_sequence";

        #endregion

        #region Public Properties

        /// <summary>
        /// The database provider
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.SQLite;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public SQLiteDatabaseAnalyzer(string connectionString) : base(connectionString)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a database connection using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        public override SQLiteConnection CreateConnection(string connectionString) => new SQLiteConnection(connectionString);

        #region Common

        /// <summary>
        /// Gets the databases from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public override IFailable<IEnumerable<IDbProviderDatabase>> GetDatabases() => new Failable<List<IDbProviderDatabase>>()
        {
            Result = new List<IDbProviderDatabase>()
            { 
                new SQLiteProviderDatabase()
                {
                    DatabaseName = Connection.Database,
                    CatalogName = Connection.Database
                } 
            }
        };

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
                    Result = Connection.GetSchema("Tables", new string[4] { null, databaseName, null, null })
                    .Rows
                    .OfType<DataRow>()
                    .Where(x => x.GetString(2).ToLower() != SQLiteSequenceTableName)
                    .Select(x => new SQLiteProviderTable(x))
                    .ToList()
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
                    Result = Connection.GetSchema("Columns", new string[4] { null, null, tableName, null }).Rows.OfType<DataRow>().Where(x => x.GetString(2).ToLower() != SQLiteSequenceTableName).Select(x =>
                    {
                        // Initialize the column
                        var column = new SQLiteProviderColumn(x);

                        // Get the ClrType name
                        var clrTypeName = ProviderDataTypes.FirstOrDefault(y => y.TypeName == column.DataType)?.DataType;

                        // Cast the column
                        var castedInstance = column.CastTo<IDbProviderColumn>();

                        // Set the column data type
                        castedInstance.DataType = Type.GetType(clrTypeName, false, true);


                        // Set the column data type
                        castedInstance.DataType = relationalTypeMappingService.FindMapping(castedInstance.DataType, castedInstance.SQLTypeName).ClrType;

                        if (castedInstance.IsNullable && castedInstance.DataType.IsValueType)
                        {
                            castedInstance.DataType = typeof(Nullable<>).MakeGenericType(castedInstance.DataType);
                        }

                        // Set the primary key flag
                        castedInstance.IsPrimaryKey = IsPrimaryKeyColumn(tableName, column.ColumnName);

                        // If the column is the primary key and auto increment...
                        if (castedInstance.IsPrimaryKey && column.IsAutoIncrement)
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
                    Result = Connection.GetSchema("IndexColumns", new string[4] { null, null, tableName, null }).Rows.OfType<DataRow>().Select(x => new SQLiteProviderIndexColumn(x)).ToList()
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
                    Result = Connection.GetSchema("ForeignKeys", new string[4] { null, null, tableName, null }).Rows.OfType<DataRow>().Select(x => new SQLiteProviderForeignKey(x)).ToList()
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

        /// <summary>
        /// Gets the restrictions from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public override IFailable<IEnumerable<DbProviderRestriction>> GetRestrictions() => new Failable<IEnumerable<DbProviderRestriction>>() 
        { 
            Result = Enumerable.Empty<DbProviderRestriction>()
        };

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns a <see cref="DbContext"/> using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override DbContext CreateDbContext(string connectionString)
        {
            return new DbContext(new DbContextOptionsBuilder<DbContext>().UseSqlite(connectionString).Options);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if the column is primary key
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private bool IsPrimaryKeyColumn(string tableName, string columnName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"PRAGMA table_info(`{tableName}`)";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // While there are rows...
                while (reader.Read())
                {
                    // Read the column name
                    var name = (string)reader[1];

                    // If the column name is found...
                    if (name == columnName)
                        // Return the primary key flag
                        return (long)reader[5] == 1;
                }

                // Return false
                return false;
            }
        }

        /// <summary>
        /// Gets the database provider data type for the column <paramref name="columnName"/> in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private string GetColumnDbProviderDataType(string tableName, string columnName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"PRAGMA table_info(`{tableName}`)";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // While there are rows...
                while (reader.Read())
                {
                    // Read the column name
                    var name = (string)reader[1];

                    // If the column name is found...
                    if (name == columnName)
                        // Return the column type
                        return (string)reader[2];
                }

                // Return null
                return null;
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
            command.CommandText = $"SELECT seq FROM SQLITE_SEQUENCE WHERE name = '{tableName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // While there are rows...
                while (reader.Read())
                {
                    // Read value
                    return reader[0].ToString().ToUnsignedInt();
                }

                // Return null
                return null;
            }
        }

        #endregion
    }
}
