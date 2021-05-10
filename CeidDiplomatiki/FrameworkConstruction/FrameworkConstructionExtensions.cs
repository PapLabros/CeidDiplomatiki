using Atom.Core;
using Atom.Relational.Analyzers;
using Microsoft.Extensions.DependencyInjection;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Extension methods for <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Adds the <see cref="CeidDiplomatikiManager"/> in the framework construction
        /// </summary>
        /// <param name="construction">The construction</param>
        /// <returns></returns>
        public static FrameworkConstruction AddCeidDiplomatikiManager(this FrameworkConstruction construction)
        {
            // Add the manager
            construction.Services.AddSingleton<CeidDiplomatikiManager>(provider => new CeidDiplomatikiManager(CeidDiplomatikiDI.GetOptionsFileName));

            // Return for chaining
            return construction;
        }

        /// <summary>
        /// Adds the database analyzers in the framework construction
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddDatabaseAnalyzers(this FrameworkConstruction construction)
        {
            construction.Services.AddSingleton<MySqlDatabaseAnalyzer>();
            construction.Services.AddSingleton<SQLiteDatabaseAnalyzer>();

            // Return for chaining
            return construction;
        }
    }
}
