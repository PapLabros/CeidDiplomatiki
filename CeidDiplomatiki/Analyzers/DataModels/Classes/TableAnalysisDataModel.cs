using System.Collections.Generic;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The data model that describes the current state of a table 
    /// </summary>
    public class TableAnalysisDataModel
    {
        #region Public Properties

        /// <summary>
        /// The table
        /// </summary>
        public IDbProviderTable Table { get; }

        /// <summary>
        /// The table's columns
        /// </summary>
        public IEnumerable<IDbProviderColumn> Columns { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="columns">The table's columns</param>
        /// <param name="table">The table</param>
        public TableAnalysisDataModel(IDbProviderTable table, IEnumerable<IDbProviderColumn> columns) : base()
        {
            Table = table ?? throw new System.ArgumentNullException(nameof(table));
            Columns = columns ?? throw new System.ArgumentNullException(nameof(columns));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Table.TableName;

        #endregion
    }
}
