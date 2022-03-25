using MySql.Data.MySqlClient;

using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Connection information for an MySQL database
    /// </summary>
    public class MySQLOptionsDataModel : BaseDatabaseOptionsDataModel, IEquatable<MySQLOptionsDataModel>
    {
        #region Public Properties

        /// <summary>
        /// The server name/ip
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The optional port number
        /// </summary>
        public uint? Port { get; set; }

        /// <summary>
        /// The user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The database provider the options represent
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.MySQL;

        /// <summary>
        /// The name of the database provider
        /// </summary>
        public override string Name => RelationalConstants.MySQLProviderName;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MySQLOptionsDataModel() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to create the <paramref name="connectionString"/> string
        /// </summary>
        /// <param name="connectionString">The connection string result</param>
        /// <returns></returns>
        public override bool TryGetConnectionString(out string connectionString)
        {
            try
            {
                // Create a MySQL connection string builder...
                var connectionStringBuilder = new MySqlConnectionStringBuilder()
                {
                    Server = Server,
                    Database = DatabaseName,
                    Port = Port ?? 3306,
                    UserID = UserId,
                    Password = Password
                };

                // Get the connection string
                connectionString = connectionStringBuilder.ConnectionString;

                // Return true
                return true;
            }
            catch
            {
                // Return no connection string
                connectionString = null;
                // Return false
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is MySQLOptionsDataModel options)
                return Equals(options);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(MySQLOptionsDataModel other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   TablesPrefix == other.TablesPrefix &&
                   Provider == other.Provider &&
                   DatabaseName == other.DatabaseName &&
                   Server == other.Server &&
                   Port == other.Port &&
                   UserId == other.UserId &&
                   Password == other.Password;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCode.Combine(Id, Provider, DatabaseName, TablesPrefix, Server, Port, UserId, Password);

        #endregion
    }

    /// <summary>
    /// Constants related to the database providers
    /// </summary>
    public static class RelationalConstants
    {
        #region Providers

        /// <summary>
        /// The SQLite provider name
        /// </summary>
        public const string SQLiteProviderName = "SQLite";

        /// <summary>
        /// The MySQL provider name
        /// </summary>
        public const string MySQLProviderName = "MySQL";

        /// <summary>
        /// The SQLServer provider name
        /// </summary>
        public const string SQLServerProviderName = "SQLServer";

        /// <summary>
        /// The PostgreSQL provider name
        /// </summary>
        public const string PostgreSQLProviderName = "PostgreSQL";

        /// <summary>
        /// The default SQLServer port
        /// </summary>
        public const uint DefaultSQLServerPort = 1433;

        /// <summary>
        /// The default MySQL port
        /// </summary>
        public const uint DefaultMySQLServerPort = 3306;

        /// <summary>
        /// The default PostgreSQL port
        /// </summary>
        public const int DefaultPostgreServerPort = 5432;

        #endregion
    }

    /// <summary>
    /// The base for all the database provider options data model
    /// </summary>
    public abstract class BaseDatabaseOptionsDataModel : IEquatable<BaseDatabaseOptionsDataModel>
    {
        #region Public Properties

        /// <summary>
        /// Unique identifier for the model
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The database provider the options represent
        /// </summary>
        public abstract SQLDatabaseProvider Provider { get; }

        /// <summary>
        /// The name of the database provider
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The name of the database
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The prefix that should be applied to all the tables of the database
        /// </summary>
        public string TablesPrefix { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDatabaseOptionsDataModel()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        /// <summary>
        /// Attempts to create the <paramref name="connectionString"/> string
        /// </summary>
        /// <param name="connectionString">The connection string result</param>
        /// <returns></returns>
        public abstract bool TryGetConnectionString(out string connectionString);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is BaseDatabaseOptionsDataModel options)
                return Equals(options);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(BaseDatabaseOptionsDataModel other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   TablesPrefix == other.TablesPrefix &&
                   Provider == other.Provider &&
                   DatabaseName == other.DatabaseName;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(Id, Provider, DatabaseName, TablesPrefix);

        #endregion
    }
}
