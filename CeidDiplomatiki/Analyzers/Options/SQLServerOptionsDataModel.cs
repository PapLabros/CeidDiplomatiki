
using Atom.Core;

using Microsoft.Data.SqlClient;

using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Connection information for an SQLServer database
    /// </summary>
    public class SQLServerOptionsDataModel : BaseDatabaseOptionsDataModel, IEquatable<SQLServerOptionsDataModel>
    {
        #region Public Properties

        /// <summary>
        /// The database provider the options represent
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.SQLServer;

        /// <summary>
        /// The name of the database provider
        /// </summary>
        public override string Name => RelationalConstants.SQLServerProviderName;

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
        /// A flag indicating whether integrated should be used or not
        /// </summary>
        public bool IntegratedSecurity { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SQLServerOptionsDataModel()
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
                // Create a SQLServer connection string builder...
                var connectionStringBuilder = new SqlConnectionStringBuilder()
                {
                    IntegratedSecurity = IntegratedSecurity,
                    DataSource = GetServer(),
                };

                if (!UserId.IsNullOrEmpty())
                    connectionStringBuilder.UserID = UserId;

                if (!Password.IsNullOrEmpty())
                    connectionStringBuilder.Password = Password;

                if (!DatabaseName.IsNullOrEmpty())
                    connectionStringBuilder.InitialCatalog = DatabaseName;

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
        /// Gets the server including the port when needed
        /// </summary>
        /// <returns></returns>
        public string GetServer()
        {
            if (Port == null || Port == RelationalConstants.DefaultSQLServerPort)
                return Server;

            return $"Server, {Port}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is SQLServerOptionsDataModel options)
                return Equals(options);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(SQLServerOptionsDataModel other)
        {
            if (other == null)
                return false;

            return Provider == other.Provider &&
                   DatabaseName == other.DatabaseName &&
                   Server == other.Server &&
                   Port == other.Port &&
                   UserId == other.UserId &&
                   Password == other.Password &&
                   IntegratedSecurity == other.IntegratedSecurity;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCodeHelpers.Combine(Id, Provider, DatabaseName, TablesPrefix, Provider, Server, Port, UserId, Password, IntegratedSecurity);

        #endregion
    }
}
