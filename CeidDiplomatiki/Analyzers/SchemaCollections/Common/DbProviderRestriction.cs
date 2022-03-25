using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// This schema collection exposed information about the restrictions that are supported by the .NET Framework managed provider 
    /// that is currently used to connect to the database.
    /// </summary>
    public class DbProviderRestriction : IEquatable<DbProviderRestriction>
    {
        #region Public Properties

        /// <summary>
        /// The name of the collection that these restrictions apply to.
        /// </summary>
        public string CollectionName { get; }

        /// <summary>
        /// The name of the restriction in the collection.
        /// </summary>
        public string RestrictionName { get; }

        /// <summary>
        /// Ignored.
        /// </summary>
        public string RestrictionDefault { get; }

        /// <summary>
        /// The actual location in the collections restrictions that this particular restriction falls in.
        /// </summary>
        public int RestrictionNumber { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal DbProviderRestriction(DataRow row)
        {
            CollectionName = row.GetString(0);
            RestrictionName = row.GetString(1);
            RestrictionDefault = row.GetString(2);
            RestrictionNumber = row.GetInt(3);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => RestrictionName;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(DbProviderRestriction other)
        {
            if (other == null)
                return false;

            return CollectionName == other.CollectionName && RestrictionName == other.RestrictionName && RestrictionDefault == other.RestrictionDefault && RestrictionNumber == other.RestrictionNumber;
        }

        #endregion
    }
}
