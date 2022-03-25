namespace CeidDiplomatiki
{
    /// <summary>
    /// Contains information for all the available database providers
    /// </summary>
    public class DatabaseProviderOptionsDataModel
    {
        #region Public Properties

        /// <summary>
        /// The selected provider
        /// </summary>
        public SQLDatabaseProvider Provider { get; set; }

        /// <summary>
        /// The selected options
        /// </summary>
        public BaseDatabaseOptionsDataModel Options
        {
            get
            {
                if (Provider == SQLDatabaseProvider.MySQL)
                    return MySQL;

                if (Provider == SQLDatabaseProvider.PostgreSQL)
                    return PostgreSQL;

                if (Provider == SQLDatabaseProvider.SQLite)
                    return SQLite;

                return SQLServer;
            }
        }

        /// <summary>
        /// The SQLite related options
        /// </summary>
        public SQLiteOptionsDataModel SQLite { get; set; }

        /// <summary>
        /// The SQLServer related options
        /// </summary>
        public SQLServerOptionsDataModel SQLServer { get; set; }

        /// <summary>
        /// The MySQL related options
        /// </summary>
        public MySQLOptionsDataModel MySQL { get; set; }

        /// <summary>
        /// The PostgreSQL related options
        /// </summary>
        public PostgreSQLOptionsDataModel PostgreSQL { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseProviderOptionsDataModel()
        {
            SQLite = new SQLiteOptionsDataModel();
            SQLServer = new SQLServerOptionsDataModel();
            MySQL = new MySQLOptionsDataModel();
            PostgreSQL = new PostgreSQLOptionsDataModel();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the database name based on the selected <see cref="Provider"/>
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            if (Provider == SQLDatabaseProvider.SQLite)
                return SQLite?.DatabaseName;
            else if (Provider == SQLDatabaseProvider.MySQL)
                return MySQL?.DatabaseName;
            else if (Provider == SQLDatabaseProvider.SQLServer)
                return SQLServer?.DatabaseName;
            else
                return PostgreSQL?.DatabaseName;
        }

        /// <summary>
        /// Gets the connection string based on the selected <see cref="Provider"/>
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            var result = string.Empty;

            if (Provider == SQLDatabaseProvider.SQLite)
            {
                if (SQLite == null)
                    return result;

                SQLite.TryGetConnectionString(out result);

                return result;
            }
            else if (Provider == SQLDatabaseProvider.MySQL)
            {
                if (MySQL == null)
                    return result;

                MySQL.TryGetConnectionString(out result);

                return result;
            }
            else if (Provider == SQLDatabaseProvider.SQLServer)
            {
                if (SQLServer == null)
                    return result;

                SQLServer.TryGetConnectionString(out result);

                return result;
            }

            if (PostgreSQL == null)
                return result;

            PostgreSQL.TryGetConnectionString(out result);

            return result;
        }

        #endregion
    }
}
