
using System;
using System.Collections.Generic;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Contains information related to the creation of the <see cref="SourceDbContextType"/>
    /// </summary>
    public class DynamicDbContextCreationResult
    {
        #region Public Properties

        /// <summary>
        /// The type of the source database context
        /// </summary>
        public Type SourceDbContextType { get; }

        /// <summary>
        /// The type of the destination database context
        /// </summary>
        public Type DestinationDBContextType { get; }

        /// <summary>
        /// The type of the destination database context
        /// </summary>
        public Type DestinationDbContextType { get; }

        /// <summary>
        /// The tables
        /// </summary>
        public IEnumerable<IDbProviderTable> Tables { get; }

        /// <summary>
        /// The columns
        /// </summary>
        public IEnumerable<IDbProviderColumn> Columns { get; }

        /// <summary>
        /// The foreign key columns
        /// </summary>
        public IEnumerable<IDbProviderForeignKeyColumn> ForeignKeyColumns { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DynamicDbContextCreationResult(Type sourceDBContextType, Type destinationDBContextType, IEnumerable<IDbProviderTable> tables, IEnumerable<IDbProviderColumn> columns, IEnumerable<IDbProviderForeignKeyColumn> foreignKeyColumns) : base()
        {
            SourceDbContextType = sourceDBContextType ?? throw new ArgumentNullException(nameof(sourceDBContextType));
            DestinationDBContextType = destinationDBContextType ?? throw new ArgumentNullException(nameof(destinationDBContextType));
            Tables = tables ?? throw new ArgumentNullException(nameof(tables));
            Columns = columns ?? throw new ArgumentNullException(nameof(columns));
            ForeignKeyColumns = foreignKeyColumns ?? throw new ArgumentNullException(nameof(foreignKeyColumns));
        }

        #endregion
    }
}
