using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server user
    /// </summary>
    public class SQLServerProviderUser
    {
        #region Public Properties

        /// <summary>
        /// User ID, unique in this database. 1 is the database owner.
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// User name or group name, unique in this database.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Date the account was added.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date the account was last changed.
        /// </summary>
        public DateTime DateModified { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderUser(DataRow row) : base()
        {
            Id = row.GetShort(0);
            Username = row.GetString(1);
            DateCreated = row.GetDateTime(2);
            DateModified = row.GetDateTime(3);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Username;

        #endregion
    }
}
