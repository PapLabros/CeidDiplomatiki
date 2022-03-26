
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The extensions methods for the <see cref="DbContext"/>
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Return the <see cref="SQLDatabaseProvider"/> that the <paramref name="dbContext"/> uses or null if the provide is not supported
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static SQLDatabaseProvider? GetSQLDatabaseProviderOrDefault(this DbContext dbContext)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Return a SQL Server provider
                return SQLDatabaseProvider.SQLServer;

            // If the database provider is SQlite...
            if (database.IsSqlite())
                // Return a SQlite provider
                return SQLDatabaseProvider.SQLite;

            // If the database provider is PostgreSQL...
            if (database.IsNpgsql())
                // Return a PostgreSQL provider
                return SQLDatabaseProvider.PostgreSQL;

            // If the database provider is MySql...
            if (database.IsMySql())
                // Return a MySql provider
                return SQLDatabaseProvider.MySQL;

            // Return null
            return null;
        }

        /// <summary>
        /// Return the <see cref="SQLDatabaseProvider"/> that the <paramref name="dbContext"/> uses
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static SQLDatabaseProvider GetSQLDatabaseProvider(this DbContext dbContext)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Return a SQL Server provider
                return SQLDatabaseProvider.SQLServer;

            // If the database provider is SQlite...
            if (database.IsSqlite())
                // Return a SQlite provider
                return SQLDatabaseProvider.SQLite;

            // If the database provider is PostgreSQL...
            if (database.IsNpgsql())
                // Return a PostgreSQL provider
                return SQLDatabaseProvider.PostgreSQL;

            // If the database provider is MySql...
            if (database.IsMySql())
                // Return a MySql provider
                return SQLDatabaseProvider.MySQL;

            throw new InvalidOperationException($"The selected {database} is not supported.");
        }

        ///// <summary>
        ///// Returns a <see cref="IDatabaseAnalyzer"/> that can analyze the database that the
        ///// specified <paramref name="dbContext"/> represents
        ///// </summary>
        ///// <param name="dbContext">The database context</param>
        ///// <returns></returns>
        //public static IDatabaseAnalyzer GetDatabaseAnalyzer(this DbContext dbContext)
        //{
        //    // Get the database
        //    var database = dbContext.Database;

        //    // Get the connection string
        //    var connectionString = database.GetConnectionString();

        //    // If the database provider is SQL Server...
        //    if (database.IsSqlServer())
        //        // Return a SQL Server analyzer
        //        return new SQLServerDatabaseAnalyzer(connectionString);

        //    // If the database provider is SQlite...
        //    if (database.IsSqlite())
        //        // Return a SQLite analyzer
        //        return new SQLiteDatabaseAnalyzer(connectionString);

        //    // If the database provider is PostgreSQL...
        //    if (database.IsNpgsql())
        //        // Return a PostgreSQL analyzer
        //        return new PostgreSQLDatabaseAnalyzer(connectionString);

        //    // If the database provider is MySql...
        //    if (database.IsMySql())
        //        // Return a MySql analyzer
        //        return new MySqlDatabaseAnalyzer(connectionString);

        //    throw new InvalidOperationException($"The selected {database} is not supported.");
        //}

        /// <summary>
        /// Disables the foreign key checks for the database represented by the specified 
        /// <paramref name="dbContext"/>
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static Task DisableForeignKeyChecksAsync(this DbContext dbContext)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Execute the command
                return database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");

            // If the database provider is SQlite...
            if (database.IsSqlite())
                // Execute the command 
                return database.ExecuteSqlRawAsync("PRAGMA foreign_keys=\"0\"");

            // If the database provider is PostgreSQL...
            if (database.IsNpgsql())
                // Execute the command
                return database.ExecuteSqlRawAsync("SET session_replication_role = 'replica'");

            // If the database provider is MySql...
            if (database.IsMySql())
                // Execute the command
                return database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");

            throw new InvalidOperationException($"The selected {database} is not supported.");
        }

        /// <summary>
        /// Enables the foreign key checks for the database represented by the specified 
        /// <paramref name="dbContext"/>
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static Task EnableForeignKeyChecksAsync(this DbContext dbContext)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Execute the command
                return database.ExecuteSqlRawAsync("exec sp_MSforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");

            // If the database provider is SQlite...
            if (database.IsSqlite())
                // Execute the command 
                return database.ExecuteSqlRawAsync("PRAGMA foreign_keys=\"1\"");

            // If the database provider is PostgreSQL...
            if (database.IsNpgsql())
                // Execute the command
                return database.ExecuteSqlRawAsync("SET session_replication_role = 'origin'");

            // If the database provider is MySql...
            if (database.IsMySql())
                // Execute the command
                return database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");

            throw new InvalidOperationException($"The selected {database} is not supported.");
        }

        /// <summary>
        /// Disables the identity column inserts, for  the database represented by the specified 
        /// <paramref name="dbContext"/> if it uses the SQL Server database provider
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static Task ForSQLServerDisableIdentityInsertsAsync(this DbContext dbContext)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Execute the command
                return database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable " +
                    "@command1=\"SET IDENTITY_INSERT ? OFF\", " +
                    "@whereand = ' AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = o.id AND is_identity = 1) and o.type = ''U'''");
            else
                // Return the completed task
                return Task.CompletedTask;
        }

        /// <summary>
        /// Enables the identity column inserts, for  the database represented by the specified 
        /// <paramref name="dbContext"/> if it uses the SQL Server database provider
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static Task ForSQLServerEnableIdentityInsertAsync(this DbContext dbContext)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Execute the command
                return database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable " +
                    "@command1=\"SET IDENTITY_INSERT ? ON\", " +
                    "@whereand = ' AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = o.id AND is_identity = 1) and o.type = ''U'''");
            else
                // Return the completed task
                return Task.CompletedTask;
        }

        /// <summary>
        /// Set the starting primary key value that will be used when the next row is inserted for the specified <paramref name="dbContext"/>
        /// NOTE: The next value will be <paramref name="value"/> + 1
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="value">The starting auto increment value</param>
        /// <returns></returns>
        public static Task SetAutoIncrementPrimaryKeyValueAsync(this DbContext dbContext, string tableName, string columnName, uint value)
        {
            // Get the database
            var database = dbContext.Database;

            // If the database provider is SQL Server...
            if (database.IsSqlServer())
                // Execute the command
                return database.ExecuteSqlRawAsync($"DBCC checkident ('{tableName}', reseed, {value})");

            // If the database provider is SQlite...
            if (database.IsSqlite())
                // Execute the command 
                return database.ExecuteSqlRawAsync($"UPDATE SQLITE_SEQUENCE SET seq = {value} WHERE name = '{tableName}'");

            // If the database provider is PostgreSQL...
            if (database.IsNpgsql())
                // Execute the command
                return database.ExecuteSqlRawAsync($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{columnName}\" RESTART WITH {value}");

            // If the database provider is MySql...
            if (database.IsMySql())
                // Execute the command
                return database.ExecuteSqlRawAsync($"ALTER TABLE {tableName} AUTO_INCREMENT = {value}");

            throw new InvalidOperationException($"The selected {database} is not supported.");
        }
    }
}
