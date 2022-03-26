using Atom.Core;

using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The database converter
    /// </summary>
    public class DatabaseConverter
    {
        #region Private Members

        /// <summary>
        /// The member of <see cref="Instance"/>
        /// </summary>
        private static readonly Lazy<DatabaseConverter> mInstance = new Lazy<DatabaseConverter>(() => new DatabaseConverter());

        #endregion

        #region Public Properties

        #region Singleton

        /// <summary>
        /// The singleton
        /// </summary>
        public static DatabaseConverter Instance => mInstance.Value;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        protected DatabaseConverter() : base()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Creates a clone of the <paramref name="sourceDatabase"/> based on the information
        /// provided by the <paramref name="destinationDatabase"/>
        /// </summary>
        /// <param name="sourceDatabase">The source database</param>
        /// <param name="destinationDatabase">The destination database</param>
        /// <returns></returns>
        public async Task<IFailable> ConvertAsync(DatabaseInfo sourceDatabase, DatabaseInfo destinationDatabase)
        {
            // Create the source database context type
            var dbContextTypeResult = MigrationHelpers.CreateDbContextType(sourceDatabase);

            // If there was an error...
            if (!dbContextTypeResult.Successful)
                // Return
                return dbContextTypeResult;

            var sourceDBContextType = dbContextTypeResult.Result.SourceDbContextType;
            var destinationDBContextType = dbContextTypeResult.Result.DestinationDBContextType;
            var tables = dbContextTypeResult.Result.Tables;
            var columns = dbContextTypeResult.Result.Columns;
            var foreignKeyColumns = dbContextTypeResult.Result.ForeignKeyColumns;

            // Create the from source database context
            var sourceDbContext = MigrationHelpers.CreateDbContextInstance(sourceDatabase, sourceDBContextType, tables, columns, foreignKeyColumns);

            // Create the destination database context
            var destinationDbContext = MigrationHelpers.CreateDbContextInstance(destinationDatabase, destinationDBContextType, tables, columns, foreignKeyColumns);

            // Copy the data
            return await MigrationHelpers.CopyDataAsync(sourceDBContextType, sourceDbContext, destinationDbContext, columns);
        }

        /// <summary>
        /// Creates a clone of the <paramref name="sourceDatabase"/> based on the information
        /// provided by the <paramref name="destinationDatabase"/>. Specifying the type of the
        /// database context results in a more accurate database conversion. For example, when trying to
        /// convert an SQLite database to a MySQL database, mapping the SQLite column types to MySQL
        /// column types is usually not completely accurate. This method should be used when ever is possible.
        /// </summary>
        /// <param name="sourceDatabase">The source database</param>
        /// <param name="destinationDatabase">The destination database</param>
        /// <returns></returns>
        public async Task<IFailable> ConvertAsync<TDbContext>(DatabaseInfo sourceDatabase, DatabaseInfo destinationDatabase)
            where TDbContext : DbContext
        {
            // Create the source database context type
            var dbContextTypeResult = MigrationHelpers.CreateDbContextType(sourceDatabase);

            // If there was an error...
            if (!dbContextTypeResult.Successful)
                // Return
                return dbContextTypeResult;

            var columns = dbContextTypeResult.Result.Columns;

            // Create the from source database context
            var sourceDbContext = MigrationHelpers.CreateDbContextInstance<TDbContext>(sourceDatabase);

            // Create the destination database context
            var destinationDbContext = MigrationHelpers.CreateDbContextInstance<TDbContext>(destinationDatabase);

            // Copy the data
            return await MigrationHelpers.CopyDataAsync(typeof(TDbContext), sourceDbContext, destinationDbContext, columns);
        }

        #endregion
    }
}
