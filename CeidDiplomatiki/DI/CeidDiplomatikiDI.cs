using Atom.Core;

using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The CeidDiplomatiki services that should be available everywhere in the application
    /// </summary>
    public static class CeidDiplomatikiDI
    {
        /// <summary>
        /// Gets the CeidDiplomatiki manager
        /// </summary>
        public static CeidDiplomatikiManager GetCeidDiplomatikiManager => DI.GetService<CeidDiplomatikiManager>();

        /// <summary>
        /// Gets the CeidDiplomatiki main page builder
        /// </summary>
        public static ICeidDiplomatikiMainPageBuilder GetCeidDiplomatikiMainPageBuilder { get; internal set; }

        /// <summary>
        /// Gets the options file name
        /// </summary>
        public static string GetOptionsFileName => DI.GetConfiguration["OptionsFileName"];

        /// <summary>
        /// Gets the database provider of the requested type
        /// </summary>
        /// <param name="provider">The type of the provider</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        public static IDatabaseAnalyzer GetDatabaseAnalyzer(SQLDatabaseProvider provider, string connectionString)
        {
            if (provider == SQLDatabaseProvider.MySQL)
                return new MySqlDatabaseAnalyzer(connectionString);
            else if (provider == SQLDatabaseProvider.SQLite)
                return new SQLiteDatabaseAnalyzer(connectionString);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the MySQL database analyzer
        /// </summary>
        public static MySqlDatabaseAnalyzer GetMySQLDatabaseAnalyzer => DI.GetService<MySqlDatabaseAnalyzer>();

        /// <summary>
        /// Gets the SQLite database analyzer
        /// </summary>
        public static SQLiteDatabaseAnalyzer GetSQLiteDatabaseAnalyzer => DI.GetService<SQLiteDatabaseAnalyzer>();
    }
}
