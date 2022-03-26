using Atom.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Extension methods for <see cref="DatabaseInfo"/>
    /// </summary>
    public static class DatabaseInfoExtensions
    {
        /// <summary>
        /// Returns a <see cref="IDatabaseAnalyzer"/> that can analyze the database that the
        /// specified <paramref name="databaseInfo"/> represents
        /// </summary>
        /// <param name="databaseInfo">The database context</param>
        /// <returns></returns>
        public static IDatabaseAnalyzer GetDatabaseAnalyzer(this DatabaseInfo databaseInfo)
        {
            // If the database provider is SQL Server...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.SQLServer)
                // Return a SQL Server analyzer
                return new SQLServerDatabaseAnalyzer(databaseInfo.ConnectionString);

            // If the database provider is SQlite...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.SQLite)
                // Return a SQLite analyzer
                return new SQLiteDatabaseAnalyzer(databaseInfo.ConnectionString);

            // If the database provider is PostgreSQL...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.PostgreSQL)
                // Return a PostgreSQL analyzer
                return new PostgreSQLDatabaseAnalyzer(databaseInfo.ConnectionString);

            // Return a MySql analyzer
            return new MySqlDatabaseAnalyzer(databaseInfo.ConnectionString);
        }

        /// <summary>
        /// Creates and returns configuration options for initializing a <see cref="DbContext"/>
        /// based on the specified <paramref name="databaseInfo"/>
        /// </summary>
        /// <param name="databaseInfo">The database info</param>
        /// <returns></returns>
        public static DbContextOptions<TDbContext> GetDbContextOptions<TDbContext>(this DatabaseInfo databaseInfo)
            where TDbContext : DbContext
        {
            // Create the options builder
            var options = new DbContextOptionsBuilder<TDbContext>();

            // If the database provider is SQL Server...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.SQLServer)
                // Use the SQL server provider
                options.UseSqlServer(databaseInfo.ConnectionString);

            // If the database provider is SQlite...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.SQLite)
                // Use the SQLite provider
                options.UseSqlite(databaseInfo.ConnectionString);

            // If the database provider is PostgreSQL...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.PostgreSQL)
                // Use the PostgreSQL provider
                options.UseNpgsql(databaseInfo.ConnectionString);

            // If the database provider is PostgreSQL...
            if (databaseInfo.DatabaseProvider == SQLDatabaseProvider.MySQL)
                // Use the MySQL provider
                options.UseMySql(databaseInfo.ConnectionString);

            ((IDbContextOptionsBuilderInfrastructure)options).AddOrUpdateExtension(new PrefixDbContextOptions(databaseInfo.TablesPrefix));

            // For every configurator...
            foreach (var configurator in DI.GetServices<DbContextOptionsConfigurator>())
                // Configure the options
                configurator.Configure(options, databaseInfo.DatabaseProvider);

            // Get the options
            return options.Options;
        }

        /// <summary>
        /// Creates and returns configuration options for initializing a <see cref="DbContext"/>
        /// based on the specified <paramref name="databaseInfo"/>
        /// </summary>
        /// <param name="databaseInfo">The database info</param>
        /// <returns></returns>
        public static DbContextOptions GetDbContextOptions(this DatabaseInfo databaseInfo)
            => databaseInfo.GetDbContextOptions<DbContext>();
    }
}
