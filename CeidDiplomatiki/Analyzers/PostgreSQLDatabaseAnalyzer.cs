using Atom.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The PostgreSQL implementation of the <see cref="BaseDatabaseAnalyzer{TDbConnection}"/>
    /// </summary>
    public class PostgreSQLDatabaseAnalyzer : BaseDatabaseAnalyzer<NpgsqlConnection>
    {
        #region Public Properties

        /// <summary>
        /// The database provider
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.PostgreSQL;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public PostgreSQLDatabaseAnalyzer(string connectionString) : base(connectionString)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a database connection using the specified connection
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        public override NpgsqlConnection CreateConnection(string connectionString) => new NpgsqlConnection(connectionString);

        #region Common

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
                    Result = Connection.GetSchema("columns", new string[4] { databaseName, null, tableName, null }).Rows.OfType<DataRow>().Select(x =>
                    {
                        // Initialize the column
                        var column = new PostgreSQLProviderColumn(x);

                        // Get the CLR type name
                        var clrTypeName = ProviderDataTypes.FirstOrDefault(y => y.TypeName == column.SQLType)?.DataType;

                        if (clrTypeName == null)
                        {                            
                            clrTypeName = ProviderDataTypes.FirstOrDefault(y => y.TypeName.Contains(column.SQLType))?.DataType;

                            if (column.SQLType.Contains("character varying"))
                                clrTypeName = "string";
                        }

                        // Cast the instance
                        var castedInstance = column.CastTo<IDbProviderColumn>();

                        // Set the column data type
                        castedInstance.DataType = Type.GetType(clrTypeName, false, true);

                        // Set the column data type
                        castedInstance.DataType = relationalTypeMappingService.FindMapping(castedInstance.DataType, castedInstance.SQLTypeName).ClrType;

                        if (castedInstance.IsNullable && castedInstance.DataType.IsValueType)
                        {
                            castedInstance.DataType = typeof(Nullable<>).MakeGenericType(castedInstance.DataType);
                        }

                        // Get the column name
                        var columnName = column.ColumnName;

                        // Set the nullable column flag
                        castedInstance.IsNullable = IsNullableColumn(tableName, columnName);

                        // Set the unique column flag
                        castedInstance.IsUnique = IsUniqueKeyColumn(tableName, columnName);

                        // Set the primary key flag
                        castedInstance.IsPrimaryKey = IsPrimaryKeyColumn(tableName, columnName);

                        // Set the auto increment flag
                        castedInstance.IsAutoIncrement = IsAutoIncrementColumn(tableName, columnName);

                        // If the column is the primary key and auto increment...
                        if (castedInstance.IsPrimaryKey && castedInstance.IsAutoIncrement)
                            // Set the next auto increment value
                            castedInstance.NextAutoIncrementValue = GetNextAutoIncrementValue(tableName, columnName);

                        // Set the database SQL provider data type
                        castedInstance.DbProviderSQLType = GetColumnDbProviderDataType(tableName, columnName);

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
                    Result = Connection.GetSchema("databases").Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderDatabase(x)).ToList()
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
                    Result = Connection.GetSchema("constraintcolumns", new string[4] { databaseName, null, tableName, null }).Rows.OfType<DataRow>().Where(x => x.GetString(8) == "FOREIGN KEY").Select(x =>
                    {
                        // Initialize the column
                        var foreignKey = new PostgreSQLProviderForeignKeyColumn(x);

                        // Get the referenced values
                        var referencedValues = GetReferencedKeyValues(foreignKey.TableName, foreignKey.ConstraintName, foreignKey.ColumnName);

                        // Close the connection
                        Connection.Close();

                        // Set the referenced table schema
                        foreignKey.ReferencedTableSchema = referencedValues.ReferencedSchemaName;

                        // Set the referenced table name
                        foreignKey.ReferencedTableName = referencedValues.ReferencedTableName;

                        // Set the referenced column name
                        foreignKey.ReferencedColumnName = referencedValues.ReferencedColumnName;

                        // Set the on delete behavior
                        foreignKey.OnDelete = GetDeleteBehavior(foreignKey.ConstraintName);

                        // Return column
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
                // Get the columns
                var columns = GetColumns(databaseName, tableName);

                // If not successful...
                if(!columns.Successful)
                    return new Failable<IEnumerable<IDbProviderIndex>>()
                    {
                        ErrorMessage = columns.ErrorMessage
                    };

                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<IDbProviderIndex>>()
                {
                    Result = Connection.GetSchema("indexcolumns", new string[5] { databaseName, null, tableName, null, null }).Rows.OfType<DataRow>().Select(x =>
                    {
                        // Initialize the index column
                        var indexColumn = new PostgreSQLProviderIndexColumn(x);

                        // Get the ordinal position
                        indexColumn.OrdinalPosition = columns.Result.FirstOrDefault(y => y.ColumnName == indexColumn.ColumnName).OrdinalPosition;

                        // Return the index column
                        return indexColumn;
                    }).ToList()
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
                    Result = Connection.GetSchema("tables", new string[4] { databaseName, null, null, null }).Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderTable(x)).ToList()
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

        #endregion

        #region PostgreSQL

        /// <summary>
        /// Gets the views from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<PostgreSQLProviderView>> GetViews()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<PostgreSQLProviderView>>()
                {
                    Result = Connection.GetSchema("views").Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderView(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<PostgreSQLProviderView>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the views from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<PostgreSQLProviderView>>> GetViewsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<PostgreSQLProviderView>(() => GetViews());

        /// <summary>
        /// Gets the users from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<PostgreSQLProviderUser>> GetUsers()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<PostgreSQLProviderUser>>()
                {
                    Result = Connection.GetSchema("users").Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderUser(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<PostgreSQLProviderUser>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the users from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<PostgreSQLProviderUser>>> GetUsersAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<PostgreSQLProviderUser>(() => GetUsers());

        /// <summary>
        /// Gets the indexes from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<PostgreSQLProviderIndex>> GetIndexes()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<PostgreSQLProviderIndex>>()
                {
                    Result = Connection.GetSchema("indexes").Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderIndex(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<PostgreSQLProviderIndex>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the indexes from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<PostgreSQLProviderIndex>>> GetIndexesAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<PostgreSQLProviderIndex>(() => GetIndexes());

        /// <summary>
        /// Gets the schemata from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<PostgreSQLProviderSchemata>> GetSchemata()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the meta data
                return new Failable<IEnumerable<PostgreSQLProviderSchemata>>()
                {
                    Result = Connection.GetSchema("schemata").Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderSchemata(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<PostgreSQLProviderSchemata>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the schemata from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<PostgreSQLProviderSchemata>>> GetSchemataAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<PostgreSQLProviderSchemata>(() => GetSchemata());

        /// <summary>
        /// Gets the data types from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public override IFailable<IEnumerable<DbProviderDataType>> GetDataTypes()
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
                    Result = Connection.GetSchema("datatypes").Rows.OfType<DataRow>().Select(x => new PostgreSQLProviderDataType(x)).ToList()
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
        /// Check if the column is auto incremented
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private bool IsAutoIncrementColumn(string tableName, string columnName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT is_identity FROM information_schema.columns WHERE table_name = '{tableName}' AND column_name = '{columnName}' AND is_identity = 'YES'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Return the status
                return reader.HasRows;
            }
        }

        /// <summary>
        /// Check if the column is primary key
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private bool IsPrimaryKeyColumn(string tableName, string columnName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT c.column_name FROM information_schema.table_constraints tc " +
                $"JOIN information_schema.constraint_column_usage AS ccu USING(constraint_schema, constraint_name) " +
                $"JOIN information_schema.columns AS c ON c.table_schema = tc.constraint_schema AND tc.table_name = c.table_name AND ccu.column_name = c.column_name " +
                $"WHERE constraint_type = 'PRIMARY KEY' and tc.table_name = '{tableName}' AND c.column_name = '{columnName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Return the status
                return reader.HasRows;
            }
        }

        /// <summary>
        /// Check if the column is unique
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private bool IsUniqueKeyColumn(string tableName, string columnName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT tc.constraint_name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc " +
                $"inner join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE cu on cu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME " +
                $"where tc.CONSTRAINT_TYPE = 'UNIQUE' and tc.TABLE_NAME = '{tableName}' and cu.COLUMN_NAME = '{columnName}' limit 1";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Return the status
                return reader.HasRows;
            }
        }

        /// <summary>
        /// Check if the column is nullable
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private bool IsNullableColumn(string tableName, string columnName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT is_nullable FROM information_schema.columns WHERE table_name = '{tableName}' AND column_name = '{columnName}' AND is_nullable = 'YES'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Return the status
                return reader.HasRows;
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
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT pg_catalog.format_type(pg_attribute.atttypid, pg_attribute.atttypmod) AS data_type " +
                $"FROM pg_catalog.pg_attribute INNER JOIN pg_catalog.pg_class ON pg_class.oid = pg_attribute.attrelid " +
                $"INNER JOIN pg_catalog.pg_namespace ON pg_namespace.oid = pg_class.relnamespace " +
                $"WHERE pg_attribute.attnum > 0 AND NOT pg_attribute.attisdropped AND pg_class.relname = '{tableName}' AND pg_attribute.attname = '{columnName}' ORDER BY attnum ASC;";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                //Get the only record
                reader.Read();

                // Return the column type
                return reader.GetString(0);
            }
        }

        /// <summary>
        /// Gets a referenced values for the foreign key 
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private ReferencedKeyValues GetReferencedKeyValues(string tableName, string foreignKeyName, string columnName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT ccu.table_schema AS foreign_table_schema, ccu.table_name AS foreign_table_name, ccu.column_name AS foreign_column_name" +
                $" FROM information_schema.table_constraints " +
                $"AS tc JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name " +
                $"AND tc.table_schema = kcu.table_schema JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name " +
                $"AND ccu.table_schema = tc.table_schema WHERE tc.constraint_type = 'FOREIGN KEY' AND tc.table_name = '{tableName}' AND kcu.column_name='{columnName}' AND tc.constraint_name='{foreignKeyName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                //Get the only record
                reader.Read();

                // Return the struct with the values
                return new ReferencedKeyValues(reader.GetString(0), reader.GetString(1), reader.GetString(2));
            }
        }

        /// <summary>
        /// Gets the next auto increment value for the primary key in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private uint? GetNextAutoIncrementValue(string tableName, string columnName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT \"{columnName}\" from \"{tableName}\" order by \"{columnName}\" desc limit 1";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // While there are rows...
                while (reader.Read())
                {
                    // Read value
                    return (uint)reader.GetInt32(0);
                }

                // Return null
                return null;
            }
        }

        /// <summary>
        /// Gets the delete behavior for the foreign key with the specifed <paramref name="foreignKeyName"/>
        /// </summary>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <returns></returns>
        private DatabaseForeignKeyDeleteBehavior GetDeleteBehavior(string foreignKeyName)
        {
            // If the connection is closed...
            if (Connection.State == ConnectionState.Closed)
                // Open the connection
                Connection.Open();

            // Declare a command
            var command = Connection.CastTo<DbConnection>().CreateCommand();

            // Set the command text
            command.CommandText = $"select delete_rule " +
                $"from information_schema.table_constraints tco " +
                $"join information_schema.referential_constraints rco on tco.constraint_schema = rco.constraint_schema and tco.constraint_name = rco.constraint_name " +
                $"where tco.constraint_type = 'FOREIGN KEY' and rco.constraint_name = '{foreignKeyName}'; ";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Read value
                reader.Read();

                // Get the behavior
                var behavior = reader.GetString(0);

                if (behavior == "NO ACTION")
                    return DatabaseForeignKeyDeleteBehavior.NoAction;

                if (behavior == "RESTRICT")
                    return DatabaseForeignKeyDeleteBehavior.Restrict;

                if (behavior == "SET NULL")
                    return DatabaseForeignKeyDeleteBehavior.SetNull;

                return DatabaseForeignKeyDeleteBehavior.Cascade;
            }
        }

        #endregion
    }
}
