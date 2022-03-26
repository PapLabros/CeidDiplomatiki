
using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Contains information related to a database
    /// </summary>
    public struct DatabaseInfo : IEquatable<DatabaseInfo>
    {
        #region Public Properties

        /// <summary>
        /// The database provider
        /// </summary>
        public SQLDatabaseProvider DatabaseProvider { get; }

        /// <summary>
        /// The database name
        /// </summary>
        public string DatabaseName { get; }

        /// <summary>
        /// The connection string
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// The tables prefix
        /// </summary>
        public string TablesPrefix { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="databaseProvider">The database provider</param>
        /// <param name="databaseName">The database name</param>
        /// <param name="connectionString">The connection string</param>
        /// <param name="tablesPrefix">The tables prefix</param>
        public DatabaseInfo(SQLDatabaseProvider databaseProvider, string databaseName, string connectionString, string tablesPrefix)
        {
            DatabaseProvider = databaseProvider;
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            TablesPrefix = tablesPrefix;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Provider: {DatabaseProvider}, Database name: {DatabaseName}, Connection string: {ConnectionString}, Tables prefix: {TablesPrefix}";

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is DatabaseInfo databaseInfo)
                return Equals(databaseInfo);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(DatabaseInfo other)
            => DatabaseProvider == other.DatabaseProvider && DatabaseName == other.DatabaseName && ConnectionString == other.ConnectionString && TablesPrefix == other.TablesPrefix;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(DatabaseProvider, DatabaseName, ConnectionString, TablesPrefix);

        #endregion
    }
}
