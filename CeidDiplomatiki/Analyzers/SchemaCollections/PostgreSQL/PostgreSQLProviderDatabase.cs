using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a PostgreSQL database
    /// </summary>
    internal class PostgreSQLProviderDatabase : IDbProviderDatabase
    {
        #region Public Properties

        /// <summary>
        /// Name of the catalog
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// Name of the database.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The owner
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The encoding used in the database
        /// </summary>
        public string Encoding { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal PostgreSQLProviderDatabase(DataRow row) : base()
        {
            DatabaseName = row.GetString(0);
            Owner = row.GetString(1);
            Encoding = row.GetString(2);
            CatalogName = DatabaseName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => DatabaseName;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDbProviderDatabase dbProviderDatabase)
                return Equals(dbProviderDatabase);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(IDbProviderDatabase other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return ((IDbProviderDatabase)this).CatalogName == other.CatalogName
                && ((IDbProviderDatabase)this).DatabaseName == other.DatabaseName;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(CatalogName, DatabaseName, Owner, Encoding);

        #endregion
    }
}
