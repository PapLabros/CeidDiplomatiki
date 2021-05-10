
using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Args used by the <see cref="DataGridPresenterDataStorage"/>
    /// </summary>
    public class DataGridPresenterArgs
    {
        #region Public Properties

        /// <summary>
        /// The current page index starting from 0
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Maximum number of items to be returned in result set.
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Limit results to those matching a string.
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Limit response to resources published after a given date.
        /// </summary>
        public DateTimeOffset? After { get; set; }

        /// <summary>
        /// Limit response to resources published before a given date.
        /// </summary>
        public DateTimeOffset? Before { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataGridPresenterArgs() : base()
        {

        }

        #endregion
    }
}
