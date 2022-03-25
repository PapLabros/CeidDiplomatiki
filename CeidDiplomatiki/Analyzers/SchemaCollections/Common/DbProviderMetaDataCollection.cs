using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// This schema collection exposes information about all of the schema collections supported by the .NET Framework 
    /// managed provider that is currently used to connect to the database.
    /// </summary>
    public class DbProviderMetaDataCollection : IEquatable<DbProviderMetaDataCollection>
    {
        #region Public Properties

        /// <summary>
        /// The name of the collection to pass to the GetSchema method to return the collection.
        /// </summary>
        public string CollectionName { get; }

        /// <summary>
        /// The number of restrictions that may be specified for the collection.
        /// </summary>
        public int NumberOfRestrictions { get; }

        /// <summary>
        /// The number of parts in the composite identifier/database object name. 
        /// For example, in SQL Server, this would be 3 for tables and 4 for columns. In Oracle, it would be 2 for tables and 3 for columns.
        /// </summary>
        public int NumberOfIdentifierParts { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal DbProviderMetaDataCollection(DataRow row)
        {
            CollectionName = row.GetString(0);
            NumberOfRestrictions = row.GetInt(1);
            NumberOfIdentifierParts = row.GetInt(2);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => CollectionName;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(DbProviderMetaDataCollection other)
        {
            if (other == null)
                return false;

            return CollectionName == other.CollectionName && NumberOfRestrictions == other.NumberOfRestrictions && NumberOfIdentifierParts == other.NumberOfIdentifierParts;
        }

        #endregion
    }
}
