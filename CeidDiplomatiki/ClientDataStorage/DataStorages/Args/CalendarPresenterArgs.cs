
using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Args used by the <see cref="CalendarPresenterDataStorage"/>
    /// </summary>
    public class CalendarPresenterArgs
    {
        #region Public Properties

        /// <summary>
        /// Limit response to resources published after a given date.
        /// </summary>
        public DateTimeOffset After { get; set; }

        /// <summary>
        /// Limit response to resources published before a given date.
        /// </summary>
        public DateTimeOffset Before { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CalendarPresenterArgs() : base()
        {

        }

        #endregion
    }
}
