using Atom.Core;
using Atom.Relational;
using Atom.Relational.Analyzers;

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
        public static CeidDiplomatikiManager GetCeidDiplomatikiManager => CoreDI.GetService<CeidDiplomatikiManager>();

        /// <summary>
        /// Gets the CeidDiplomatiki main page builder
        /// </summary>
        public static ICeidDiplomatikiMainPageBuilder GetCeidDiplomatikiMainPageBuilder { get; internal set; }

        /// <summary>
        /// Gets the options file name
        /// </summary>
        public static string GetOptionsFileName => CoreDI.GetConfiguration["OptionsFileName"];

        /// <summary>
        /// Gets the database provider of the requested type
        /// </summary>
        /// <param name="provider">The type of the provider</param>
        /// <returns></returns>
        public static IDatabaseAnalyzer GetDatabaseAnalyzer(SQLDatabaseProvider provider)
        {
            if (provider == SQLDatabaseProvider.MySQL)
                return GetMySQLDatabaseAnalyzer;
            else if (provider == SQLDatabaseProvider.SQLite)
                return GetSQLiteDatabaseAnalyzer;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the MySQL database analyzer
        /// </summary>
        public static MySqlDatabaseAnalyzer GetMySQLDatabaseAnalyzer => CoreDI.GetService<MySqlDatabaseAnalyzer>();

        /// <summary>
        /// Gets the SQLite database analyzer
        /// </summary>
        public static SQLiteDatabaseAnalyzer GetSQLiteDatabaseAnalyzer => CoreDI.GetService<SQLiteDatabaseAnalyzer>();
    }
}
