using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server reserved word
    /// </summary>
    public class SQLServerProviderReservedWord
    {
        #region Public Properties

        /// <summary>
        /// Provider specific reserved word.
        /// </summary>
        public string ReservedWord { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderReservedWord(DataRow row) : base()
        {
            ReservedWord = row.GetString(0);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ReservedWord;

        #endregion
    }
}
