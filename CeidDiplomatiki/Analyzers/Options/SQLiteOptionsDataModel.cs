
using Atom.Core;

using System;
using System.IO;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Connection information for an SQLite database
    /// </summary>
    public class SQLiteOptionsDataModel : BaseDatabaseOptionsDataModel, IEquatable<SQLiteOptionsDataModel>
    {
        #region Public Properties

        /// <summary>
        /// The database provider the options represent
        /// </summary>
        public override SQLDatabaseProvider Provider => SQLDatabaseProvider.SQLite;

        /// <summary>
        /// The name of the database provider
        /// </summary>
        public override string Name => RelationalConstants.SQLiteProviderName;

        /// <summary>
        /// The path to the director that contains the database
        /// </summary>
        public string DirectoryPath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SQLiteOptionsDataModel()
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
            connectionString = $"Data Source={Path.Combine(DirectoryPath, $"{(DatabaseName.IsNullOrEmpty() ? "test" : DatabaseName)}.db")}";

            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is SQLiteOptionsDataModel options)
                return Equals(options);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(SQLiteOptionsDataModel other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   TablesPrefix == other.TablesPrefix &&
                   Provider == other.Provider &&
                   DatabaseName == other.DatabaseName &&
                   DirectoryPath == other.DirectoryPath;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(Id, Provider, DatabaseName, TablesPrefix, Provider, DirectoryPath);

        #endregion
    }
}
