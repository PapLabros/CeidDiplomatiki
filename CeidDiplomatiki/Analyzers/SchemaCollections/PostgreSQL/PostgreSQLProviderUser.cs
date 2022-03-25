using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a PostgreSQL provider user
    /// </summary>
    public class PostgreSQLProviderUser
    {
        #region Public Properties

        /// <summary>
        /// The user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The unique user system id
        /// </summary>
        public int UserSystemId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal PostgreSQLProviderUser(DataRow row) : base()
        {
            Username = row.GetString(0);
            UserSystemId = row.GetInt(1);
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
