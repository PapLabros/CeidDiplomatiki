namespace CeidDiplomatiki
{
    /// <summary>
    /// The values for the referenced key for a specific foreign key
    /// </summary>
    internal struct ReferencedKeyValues
    {
        #region Public Properties

        /// <summary>
        /// Schema that contains the referenced table.
        /// </summary>
        public string ReferencedSchemaName { get; }

        /// <summary>
        /// The table that contains the referenced column.
        /// </summary>
        public string ReferencedTableName { get; }

        /// <summary>
        /// Referenced column name
        /// NOTE: The referenced column is usually a primary key column!
        /// </summary>
        public string ReferencedColumnName { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="referencedColumnName">Referenced column name</param>
        /// <param name="referencedSchemaName">Schema that contains the referenced table</param>
        /// <param name="referencedTableName">The table that contains the referenced column</param>
        public ReferencedKeyValues(string referencedSchemaName, string referencedTableName, string referencedColumnName)
        {
            ReferencedSchemaName = referencedSchemaName;
            ReferencedTableName = referencedTableName;
            ReferencedColumnName = referencedColumnName;
        }

        #endregion
    }
}
