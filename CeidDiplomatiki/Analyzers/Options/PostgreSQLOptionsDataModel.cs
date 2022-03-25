using Npgsql;

using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Connection information for a PostgreSQL database
    /// </summary>
    public class PostgreSQLOptionsDataModel : BaseDatabaseOptionsDataModel, IEquatable<PostgreSQLOptionsDataModel>
    {
        #region Public Properties

        /// <summary>
        /// The host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The port number
        /// </summary>
        public int Port { get; set; } = RelationalConstants.DefaultPostgreServerPort;

        /// <summary>
        /// The user id
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The database provider the options represent
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.PostgreSQL;

        /// <summary>
        /// The name of the database provider
        /// </summary>
        public override string Name => RelationalConstants.PostgreSQLProviderName;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PostgreSQLOptionsDataModel() : base()
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
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
                {
                    Host = Host,
                    Database = DatabaseName,
                    Port = Port,
                    Username = UserName,
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
            if (obj is PostgreSQLOptionsDataModel options)
                return Equals(options);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(PostgreSQLOptionsDataModel other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   TablesPrefix == other.TablesPrefix &&
                   Provider == other.Provider &&
                   DatabaseName == other.DatabaseName &&
                   Host == other.Host &&
                   Port == other.Port &&
                   UserName == other.UserName &&
                   Password == other.Password;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCode.Combine(Id, Provider, DatabaseName, TablesPrefix, Host, Port, UserName, Password);

        #endregion
    }
}
