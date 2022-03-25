using Atom.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The SQL Server implementation of the <see cref="BaseDatabaseAnalyzer{TDbConnection}"/>
    /// </summary>
    public class SQLServerDatabaseAnalyzer : BaseDatabaseAnalyzer<SqlConnection>
    {
        #region Public Properties

        /// <summary>
        /// The database provider
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.SQLServer;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public SQLServerDatabaseAnalyzer(string connectionString) : base(connectionString)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a database connection using the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        public override SqlConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);

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
                    Result = Connection.GetSchema("Columns", new string[4] { databaseName, null, tableName, null }).Rows.OfType<DataRow>().Select(x =>
                    {
                        // Initialize the column
                        var column = new SQLServerProviderColumn(x);

                        // Cast the instance
                        var castedInstance = column.CastTo<IDbProviderColumn>();

                        // Get the ClrType name
                        var clrTypeName = ProviderDataTypes.FirstOrDefault(y => y.TypeName == column.SQLType)?.DataType;

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

                        // Set the unique column flag
                        castedInstance.IsUnique = IsUniqueKeyColumn(tableName, columnName);

                        // Set the primary key flag
                        castedInstance.IsPrimaryKey = IsPrimaryKeyColumn(tableName, columnName);

                        // Set the auto increment flag
                        castedInstance.IsAutoIncrement = IsAutoIncrementColumn(tableName, columnName);

                        // If the column is the primary key and auto increment...
                        if (castedInstance.IsPrimaryKey && castedInstance.IsAutoIncrement)
                            // Set the next auto increment value
                            castedInstance.NextAutoIncrementValue = GetNextAutoIncrementValue(tableName);

                        // Set the SQL data type
                        castedInstance.SQLTypeName = GetSQLDataType(tableName, columnName);

                        // Set the column default
                        castedInstance.ColumnDefault = GetColumnDefault(tableName, columnName);

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

                // Return the database
                return new Failable<IEnumerable<IDbProviderDatabase>>()
                {
                    Result = Connection.GetSchema("Databases").Rows.OfType<DataRow>().Select(x => new SQLServerProviderDatabase(x)).ToList()
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

                // Return the foreign keys
                return new Failable<IEnumerable<IDbProviderForeignKeyColumn>>()
                {
                    Result = Connection.GetSchema("ForeignKeys", new string[4] { databaseName, null, tableName, null }).Rows.OfType<DataRow>().Select(x =>
                    {
                        // Initialize the column
                        var foreignKey = new SQLServerProviderForeignKeyColumn(x);

                        // Get the referenced values
                        var referencedValues = GetReferencedKeyValues(foreignKey.TableName, foreignKey.ConstraintName, out var columnName);

                        // Set the column name
                        foreignKey.ColumnName = columnName;

                        // Set the referenced table schema
                        foreignKey.ReferencedTableSchema = referencedValues.ReferencedSchemaName;

                        // Set the referenced table name
                        foreignKey.ReferencedTableName = referencedValues.ReferencedTableName;

                        // Set the referenced column name
                        foreignKey.ReferencedColumnName = referencedValues.ReferencedColumnName;

                        // Set the delete behavior
                        foreignKey.OnDelete = GetOnDeleteBehavior(foreignKey.ConstraintName);

                        // Return the column
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
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the indexes
                return new Failable<IEnumerable<IDbProviderIndex>>()
                {
                    Result = Connection.GetSchema("IndexColumns", new string[5] { databaseName, null, tableName, null, null })
                    .Rows
                    .OfType<DataRow>()
                    .Select(x => new SQLServerProviderIndexColumn(x))
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

                // Return the tables
                return new Failable<IEnumerable<IDbProviderTable>>()
                {
                    Result = Connection.GetSchema("Tables", new string[4] { databaseName, null, null, null }).Rows.OfType<DataRow>().Select(x => new SQLServerProviderTable(x)).ToList()
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

        #region SQL Server

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

                // Return the data types
                return new Failable<IEnumerable<DbProviderDataType>>()
                {
                    Result = Connection.GetSchema("DataTypes").Rows.OfType<DataRow>().Select(x => new SQLServerProviderDataType(x)).ToList()
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
        /// Gets the reserved words from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderReservedWord>> GetReservedWords()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the reserved words
                return new Failable<IEnumerable<SQLServerProviderReservedWord>>()
                {
                    Result = Connection.GetSchema("ReservedWords").Rows.OfType<DataRow>().Select(x => new SQLServerProviderReservedWord(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderReservedWord>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the reserved words from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderReservedWord>>> GetReservedWordsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderReservedWord>(() => GetReservedWords());

        /// <summary>
        /// Gets the user defined types from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderUserDefinedType>> GetUserDefinedTypes()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the user defined types
                return new Failable<IEnumerable<SQLServerProviderUserDefinedType>>()
                {
                    Result = Connection.GetSchema("UserDefinedTypes").Rows.OfType<DataRow>().Select(x => new SQLServerProviderUserDefinedType(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderUserDefinedType>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        
        /// <summary>
        /// Gets the user defined types from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderUserDefinedType>>> GetUserDefinedTypesAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderUserDefinedType>(() => GetUserDefinedTypes());

        /// <summary>
        /// Gets the view columns from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderViewColumn>> GetViewColumns()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the view columns
                return new Failable<IEnumerable<SQLServerProviderViewColumn>>()
                {
                    Result = Connection.GetSchema("ViewColumns").Rows.OfType<DataRow>().Select(x => new SQLServerProviderViewColumn(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderViewColumn>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        
        /// <summary>
        /// Gets the view columns from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderViewColumn>>> GetViewColumnsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderViewColumn>(() => GetViewColumns());

        /// <summary>
        /// Gets the views from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderView>> GetViews()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the views
                return new Failable<IEnumerable<SQLServerProviderView>>()
                {
                    Result = Connection.GetSchema("Views").Rows.OfType<DataRow>().Select(x => new SQLServerProviderView(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderView>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the views from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderView>>> GetViewsAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderView>(() => GetViews());

        /// <summary>
        /// Gets the users from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderUser>> GetUsers()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the users
                return new Failable<IEnumerable<SQLServerProviderUser>>()
                {
                    Result = Connection.GetSchema("Users").Rows.OfType<DataRow>().Select(x => new SQLServerProviderUser(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderUser>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the users from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderUser>>> GetUsersAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderUser>(() => GetUsers());

        /// <summary>
        /// Gets the procedures from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderProcedure>> GetProcedures()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the procedures
                return new Failable<IEnumerable<SQLServerProviderProcedure>>()
                {
                    Result = Connection.GetSchema("Procedures").Rows.OfType<DataRow>().Select(x => new SQLServerProviderProcedure(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderProcedure>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        
        /// <summary>
        /// Gets the procedures from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderProcedure>>> GetProceduresAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderProcedure>(() => GetProcedures());

        /// <summary>
        /// Gets the procedure parameters from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderProcedureParameter>> GetProcedureParameters()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the procedure parameters
                return new Failable<IEnumerable<SQLServerProviderProcedureParameter>>()
                {
                    Result = Connection.GetSchema("ProcedureParameters").Rows.OfType<DataRow>().Select(x => new SQLServerProviderProcedureParameter(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderProcedureParameter>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        

        /// <summary>
        /// Gets the procedure parameters from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderProcedureParameter>>> GetProcedureParametersAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderProcedureParameter>(() => GetProcedureParameters());

        /// <summary>
        /// Gets the indexes from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderIndex>> GetIndexes()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the indexes
                return new Failable<IEnumerable<SQLServerProviderIndex>>()
                {
                    Result = Connection.GetSchema("Indexes").Rows.OfType<DataRow>().Select(x => new SQLServerProviderIndex(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderIndex>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }
        /// <summary>
        /// Gets the indexes from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderIndex>>> GetIndexesAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderIndex>(() => GetIndexes());

        /// <summary>
        /// Gets the data source information from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public IFailable<IEnumerable<SQLServerProviderDataSourceInformation>> GetDataSourceInformation()
        {
            try
            {
                // If the connection is closed...
                if (Connection.State == ConnectionState.Closed)
                    // Open the connection
                    Connection.Open();

                // Return the data source information
                return new Failable<IEnumerable<SQLServerProviderDataSourceInformation>>()
                {
                    Result = Connection.GetSchema("DataSourceInformation").Rows.OfType<DataRow>().Select(x => new SQLServerProviderDataSourceInformation(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Failable<IEnumerable<SQLServerProviderDataSourceInformation>>()
                {
                    ErrorMessage = ex.AggregateExceptionMessages()
                };
            }
        }

        /// <summary>
        /// Gets the data source information from the server with the specified established connection
        /// </summary>
        /// <returns></returns>
        public Task<IFailable<IEnumerable<SQLServerProviderDataSourceInformation>>> GetDataSourceInformationAsync()
            => AnalyzerHelpers.GetDatabaseCollectionAsync<SQLServerProviderDataSourceInformation>(() => GetDataSourceInformation());

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
            return new DbContext(new DbContextOptionsBuilder<DbContext>().UseSqlServer(connectionString).Options);
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
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"Select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = 'dbo' and TABLE_NAME = '{tableName}' and COLUMN_NAME = '{columnName}' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 order by TABLE_NAME";

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
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT OBJECT_SCHEMA_NAME(i.object_id) AS SchemaName, OBJECT_NAME(i.object_id) AS TableName, " +
                $"c.name AS ColumnName, ic.key_ordinal AS KeyOrdinal FROM sys.key_constraints AS kc JOIN sys.indexes AS i ON i.object_id = kc.parent_object_id AND kc.name = i.name " +
                $"JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id " +
                $"WHERE kc.type_desc = N'PRIMARY_KEY_CONSTRAINT' and c.name = '{columnName}' and OBJECT_NAME(i.object_id) = '{tableName}' ORDER BY SchemaName, TableName, KeyOrdinal";

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
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"Declare @T Table (SCOPE smallint, COLUMN_NAME sysname, DATA_TYPE smallint, RT sysname, RY int, GH int, SCALE smallint, PSEUDO_COLUMN smallint)" +
                $"Insert @T Exec sp_special_columns {tableName} " +
                $"Select COLUMN_NAME from @T where COLUMN_NAME = '{columnName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Return the status
                return reader.HasRows;
            }
        }

        /// <summary>
        /// Gets a referenced values for the foreign key 
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private ReferencedKeyValues GetReferencedKeyValues(string tableName, string foreignKeyName, out string columnName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"Declare @T Table (PKTABLE_QUALIFER sysname, PKTABLE_OWNER sysname, PKTABLE_NAME sysname, PKCOLUMN_NAME sysname, FKTABLE_QUALIFER sysname, FKTABLE_OWNER sysname, FKTABLE_NAME sysname, FKCOLUMN_NAME sysname, KEY_SEQ smallint, UPDATE_RULE smallint, DELETE_RULE smallint, FK_NAME sysname, PK_NAME sysname, DEFERRABILITY smallint) " +
                $"Insert @T EXEC sp_fkeys @fktable_name = '{tableName}' " +
                $"SELECT PKTABLE_QUALIFER, PKTABLE_NAME, PKCOLUMN_NAME, FKCOLUMN_NAME, DELETE_RULE from @T where FK_NAME = '{foreignKeyName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Get the only record
                reader.Read();

                // Set the column name
                columnName = reader.GetString(3);

                // Return the referenced 
                return new ReferencedKeyValues(reader.GetString(0), reader.GetString(1), reader.GetString(2));
            }
        }

        /// <summary>
        /// Gets the on delete behavior of the foreign key with the specified <paramref name="foreignKeyName"/>
        /// </summary>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <returns></returns>
        private DatabaseForeignKeyDeleteBehavior GetOnDeleteBehavior(string foreignKeyName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT delete_referential_action_desc FROM sys.foreign_keys where name = '{foreignKeyName}';";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Get the only record
                reader.Read();

                // Get the behavior
                var behavior = reader.GetString(0);

                if (behavior == "NO_ACTION")
                    return DatabaseForeignKeyDeleteBehavior.NoAction;

                if (behavior == "SET_NULL")
                    return DatabaseForeignKeyDeleteBehavior.SetNull;

                if (behavior == "RESTRICT")
                    return DatabaseForeignKeyDeleteBehavior.Restrict;

                return DatabaseForeignKeyDeleteBehavior.Cascade;
            }
        }

        /// <summary>
        /// Returns the SQL data type for the column <paramref name="columnName"/>, that is contained in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private string GetSQLDataType(string tableName, string columnName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Get the only record
                reader.Read();

                // Return the status
                return reader.GetString(0);
            }
        }

        /// <summary>
        /// Returns the default value for the column <paramref name="columnName"/>, that is contained in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns></returns>
        private string GetColumnDefault(string tableName, string columnName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // Get the only record
                reader.Read();

                // Return the status
                return reader.GetDbNullableString("COLUMN_DEFAULT");
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
            command.CommandText = $"SELECT t.name + '(' + cast(c.max_length as varchar(50)) + ')' As 'DataType' FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id WHERE c.object_id = Object_id('{tableName}') AND c.name = '{columnName}'";

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
        /// Gets the next auto increment value for the primary key in the table <paramref name="tableName"/>
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns></returns>
        private uint? GetNextAutoIncrementValue(string tableName)
        {
            // Declare a command
            var command = Connection.CreateCommand();

            // Set the command text
            command.CommandText = $"SELECT IDENT_CURRENT('{tableName}')";

            // Get the command reader
            using (var reader = command.ExecuteReader())
            {
                // While there are rows...
                while (reader.Read())
                {
                    // Read value
                    return (uint)reader.GetDecimal(0);
                }

                // Return null
                return null;
            }
        }

        #endregion
    }
}
