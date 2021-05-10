using Atom.Core;
using Atom.Relational;
using Atom.Relational.Providers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The CeidDiplomatiki manager
    /// </summary>
    public class CeidDiplomatikiManager
    {
        #region Private Members

        /// <summary>
        /// The registered databases
        /// </summary>
        private readonly List<BaseDatabaseOptionsDataModel> mDatabases = new List<BaseDatabaseOptionsDataModel>();

        /// <summary>
        /// The registered query maps
        /// </summary>
        private readonly List<QueryMap> mQueryMaps = new List<QueryMap>();

        /// <summary>
        /// The registered pages
        /// </summary>
        private readonly List<PageMap> mPages = new List<PageMap>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The file name of the file where all the changes are stored
        /// </summary>
        public string OptionsFileName { get; }

        #region Databases

        /// <summary>
        /// Gets all the registered databases
        /// </summary>
        public IEnumerable<BaseDatabaseOptionsDataModel> Databases => mDatabases.ToList();

        /// <summary>
        /// The registered SQLite databases
        /// </summary>
        public IEnumerable<SQLiteOptionsDataModel> SQLiteDatabases => mDatabases.OfType<SQLiteOptionsDataModel>().ToList();

        /// <summary>
        /// The registered MySQL databases
        /// </summary>
        public IEnumerable<MySQLOptionsDataModel> MySQLDatabases => mDatabases.OfType<MySQLOptionsDataModel>().ToList();

        /// <summary>
        /// The registered SQLServer databases
        /// </summary>
        public IEnumerable<SQLServerOptionsDataModel> SQLServerDatabases => mDatabases.OfType<SQLServerOptionsDataModel>().ToList();

        /// <summary>
        /// The registered PostgreSQL databases
        /// </summary>
        public IEnumerable<PostgreSQLOptionsDataModel> PostgreSQLDatabases => mDatabases.OfType<PostgreSQLOptionsDataModel>().ToList();

        #endregion

        #region Queries

        /// <summary>
        /// The query maps
        /// </summary>
        public IEnumerable<QueryMap> QueryMaps => mQueryMaps.ToList();

        #endregion

        #region Presenters

        /// <summary>
        /// The presenters
        /// </summary>
        public IEnumerable<BasePresenterMap> Presenters => QueryMaps.SelectMany(x => x.PresenterMaps);

        /// <summary>
        /// The data grid presenters
        /// </summary>
        public IEnumerable<DataGridPresenterMap> DataGridPresenters => QueryMaps.SelectMany(x => x.DataGridPresenterMaps).ToList();

        #endregion

        #region Pages

        /// <summary>
        /// The root pages
        /// </summary>
        public IEnumerable<PageMap> RootPages => mPages.ToList();

        /// <summary>
        /// All the pages
        /// </summary>
        public IEnumerable<PageMap> Pages
        {
            get
            {
                // Create the result
                var result = new List<PageMap>();

                // For every root page...
                foreach(var rootPage in RootPages)
                    // Add the pages
                    result.AddRange(GetPages(rootPage));

                // Return the result
                return result;
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CeidDiplomatikiManager(string optionsFileName) : base()
        {
            OptionsFileName = optionsFileName.NotNullOrEmpty();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and returns a <see cref="CeidDiplomatikiOptionsDataModel"/> from the current <see cref="CeidDiplomatikiManager"/>
        /// </summary>
        /// <returns></returns>
        public CeidDiplomatikiOptionsDataModel ToOptions() => new CeidDiplomatikiOptionsDataModel()
        {
            // Set the databases
            MySQLDatabases = MySQLDatabases.ToArray(),
            PostgreSQLDatabases = PostgreSQLDatabases.ToArray(),
            SQLiteDatabases = SQLiteDatabases.ToArray(),
            SQLServerDatabases = SQLServerDatabases.ToArray(),

            // Set the query maps
            QueryMaps = QueryMaps.Select(x => x.ToDataModel()).ToArray(),

            // Set the page maps
            PageMaps = RootPages.Select(x => x.ToDataModel()).ToArray()
        };

        /// <summary>
        /// Initializes the manager.
        /// NOTE: This method should be called before the usage of the manager,
        ///       usually at the entry point of the application!
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            // If there isn't an options file...
            if (!File.Exists(OptionsFileName))
                // There is nothing to initialize from, so return
                return;

            // Get the options
            var options = XMLHelpers.FromXml<CeidDiplomatikiOptionsDataModel>(OptionsFileName);

            // For every database...
            foreach (var database in options.Databases)
                // Add it
                Register(database);

            // For every database options...
            foreach(var databaseOptions in Databases)
            {
                // Get the connection string
                databaseOptions.TryGetConnectionString(out var connectionString);

                // Get the analyzer
                var analyzer = CeidDiplomatikiDI.GetDatabaseAnalyzer(databaseOptions.Provider);

                // Get the database
                var database = analyzer.GetDatabases().First(x => x.DatabaseName == databaseOptions.DatabaseName);

                // Get the tables
                var tables = analyzer.GetTables(databaseOptions.DatabaseName);

                // Get the columns
                var columns = analyzer.GetColumns(database.DatabaseName, null);

                // For every query map related to that database...
                foreach (var queryMapDataModel in options.QueryMaps.Where(x => x.DatabaseId == databaseOptions.Id))
                {
                    // The joins collection
                    var joins = new List<JoinMap>();

                    // For every pair...
                    foreach (var joinDataModel in queryMapDataModel.Joins)
                    {
                        // Get the principle column
                        var principleColumn = columns.First(x => x.ColumnName == joinDataModel.PrincipleKeyColumnName);

                        // Get the foreign key column
                        var referencedColumn = columns.First(x => x.ColumnName == joinDataModel.ForeignKeyColumnName);

                        // Create the join map
                        var joinMap = new JoinMap(tables.First(x => x.TableName == joinDataModel.TableName), principleColumn, tables.First(x => x.TableName == joinDataModel.ReferencedTableName), referencedColumn, joinDataModel.Index, joinDataModel.IsInverted);

                        // Add it to the joins
                        joins.Add(joinMap);
                    }

                    // Create the map
                    var queryMap = QueryMap.FromDataModel(databaseOptions, database, tables.Where(x => queryMapDataModel.TableNames.Contains(x.TableName)).ToList(), columns.Where(x => queryMapDataModel.TableNames.Contains(x.TableName)).ToList(), joins, queryMapDataModel);

                    // Register it
                    await RegisterAsync(queryMap);
                }
            }

            // For every page...
            foreach (var page in options.PageMaps)
                // Add it
                Register(PageMap.FromDataModel(page, null));

            // Await a task
            await Task.CompletedTask;
        }

        /// <summary>
        /// Saves the changes
        /// </summary>
        /// <returns></returns>
        public async Task<IFailable> SaveChangesAsync()
        {
            // Create the result
            var result = new Failable();

            try
            {
                // Get the directory path
                var directoryPath = Path.GetDirectoryName(OptionsFileName);

                // If the directory doesn't exist...
                if (!Directory.Exists(directoryPath))
                    // Create it
                    Directory.CreateDirectory(directoryPath);

                // Get the options
                var options = ToOptions();

                // Save the options
                XMLHelpers.ToXmlFile(options, OptionsFileName, new XmlWriterSettings());

                // Get the page builder
                var pageBuilder = CeidDiplomatikiDI.GetCeidDiplomatikiMainPageBuilder;

                // Refresh the main page
                pageBuilder.Refresh();
            }
            catch(Exception ex)
            {
                // Set the error
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return await Task.FromResult(result);
        }

        #region Databases

        /// <summary>
        /// Registers the specified <paramref name="database"/>
        /// </summary>
        /// <param name="database">The database</param>
        public void Register(BaseDatabaseOptionsDataModel database)
        {
            // Add the database
            mDatabases.Add(database);

            // If the database doesn't have an id, meaning that's a new one...
            if (database.Id.IsNullOrEmpty())
                // Set a new id
                database.Id = Guid.NewGuid().ToString();

            // Fire the event
            DatabaseRegistered(this, database);
        }

        /// <summary>
        /// Unregisters the specified <paramref name="database"/>
        /// </summary>
        /// <param name="database">The database</param>
        public void Unregister(BaseDatabaseOptionsDataModel database)
        {
            // Remove the database
            mDatabases.Remove(database);

            // Fire the event
            DatabaseUnregistered(this, database);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Registers the specified <paramref name="queryMap"/>
        /// </summary>
        /// <param name="queryMap">The query map</param>
        public Task<IFailable> RegisterAsync(QueryMap queryMap)
        {
            mQueryMaps.Add(queryMap);

            return Task.FromResult<IFailable>(new Failable());
        }

        /// <summary>
        /// Unregisters the specified <paramref name="queryMap"/>
        /// </summary>
        /// <param name="queryMap">The query map</param>
        public async Task<IFailable> Unregister(QueryMap queryMap)
        {
            // Create the result
            var result = new Failable();

            try
            {
                await Task.Delay(1);

                mQueryMaps.Remove(queryMap);
            }
            catch(Exception ex)
            {
                // Set the error message
                result.ErrorMessage = ex.Message;
            }

            // Return the result
            return result;
        }

        #endregion

        #region Pages

        /// <summary>
        /// Registers the specified <paramref name="pageMap"/>
        /// </summary>
        /// <param name="pageMap">The page map</param>
        public void Register(PageMap pageMap)
        {
            mPages.Add(pageMap);
        }

        /// <summary>
        /// Unregisters the specified <paramref name="pageMap"/>
        /// </summary>
        /// <param name="pageMap">The page map</param>
        public void Unregister(PageMap pageMap)
        {
            mPages.Remove(pageMap);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Traverses the pages root from the specified <paramref name="pageMap"/>
        /// to the end
        /// </summary>
        /// <param name="pageMap">The page map</param>
        /// <returns></returns>
        private IEnumerable<PageMap> GetPages(PageMap pageMap)
        {
            var result = new List<PageMap>() { pageMap };

            foreach(var page in pageMap.Pages)
                result.AddRange(GetPages(page));

            return result;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Event that fires every time a database gets registered
        /// </summary>
        public event EventHandler<BaseDatabaseOptionsDataModel> DatabaseRegistered = (sender, e) => { };

        /// <summary>
        /// Event that fires every time a database gets unregistered
        /// </summary>
        public event EventHandler<BaseDatabaseOptionsDataModel> DatabaseUnregistered = (sender, e) => { };

        #endregion
    }

    /// <summary>
    /// Represents the whole CeidDiplomatiki application options
    /// </summary>
    public struct CeidDiplomatikiOptionsDataModel
    {
        #region Public Properties

        /// <summary>
        /// Gets all the databases
        /// </summary>
        public IEnumerable<BaseDatabaseOptionsDataModel> Databases => SQLiteDatabases.Concat<BaseDatabaseOptionsDataModel>(MySQLDatabases).Concat(SQLServerDatabases).Concat(PostgreSQLDatabases);

        /// <summary>
        /// The registered SQLite databases
        /// </summary>
        public SQLiteOptionsDataModel[] SQLiteDatabases { get; set; }

        /// <summary>
        /// The registered MySQL databases
        /// </summary>
        public MySQLOptionsDataModel[] MySQLDatabases { get; set; }

        /// <summary>
        /// The registered SQLServer databases
        /// </summary>
        public SQLServerOptionsDataModel[] SQLServerDatabases { get; set; }

        /// <summary>
        /// The registered PostgreSQL databases
        /// </summary>
        public PostgreSQLOptionsDataModel[] PostgreSQLDatabases { get; set; }

        /// <summary>
        /// The query maps
        /// </summary>
        public QueryMapDataModel[] QueryMaps { get; set; }

        /// <summary>
        /// The page maps
        /// </summary>
        public PageMapDataModel[] PageMaps { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Databases.Count() + " database/s";

        #endregion
    }
}
