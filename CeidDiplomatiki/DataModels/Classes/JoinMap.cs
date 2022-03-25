using System;
using System.Collections.Generic;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a join between 2 tables
    /// </summary>
    public class JoinMap : IEquatable<JoinMap>
    {
        #region Public Properties

        /// <summary>
        /// The principle table
        /// </summary>
        public IDbProviderTable Table { get; }

        /// <summary>
        /// The principle key column of the <see cref="Table"/>
        /// </summary>
        public IDbProviderColumn PrincipleKeyColumn { get; }

        /// <summary>
        /// The referenced table
        /// </summary>
        public IDbProviderTable ReferencedTable { get; }

        /// <summary>
        /// The foreign key column of the <see cref="ReferencedTable"/>
        /// </summary>
        public IDbProviderColumn ForeignKeyColumn { get; }

        /// <summary>
        /// The index level of the join.
        /// NOTE: If the index is set to zero, then the <see cref="Table"/> is the root table!
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// A flag indicating whether we have a foreign key to primary key join (right join) or not.
        /// NOTE: When this is set to <see cref="false"/> a left join is applied (the navigation property of the root type is an <see cref="IEnumerable{T}"/>)!
        /// NOTE: When this is set to <see cref="true"/> a right join is applied (the navigation property of the root type is an object)!
        /// </summary>
        public bool IsRightJoin { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="table">The main table</param>
        /// <param name="principleKeyColumn">The principle key column of the <see cref="Table"/></param>
        /// <param name="referencedTable">The referenced table</param>
        /// <param name="foreignKeyColumn">The foreign key column of the <see cref="ReferencedTable"/></param>
        /// <param name="index">
        /// The index level of the join.
        /// NOTE: If the index is set to zero, then the <see cref="Table"/> is the root table!
        /// </param>
        /// <param name="isRightJoin">
        /// A flag indicating whether we have a foreign key to primary key join (right join) or not.
        /// NOTE: When this is set to <see cref="false"/> a left join is applied (the navigation property of the root type is an <see cref="IEnumerable{T}"/>)!
        /// NOTE: When this is set to <see cref="true"/> a right join is applied (the navigation property of the root type is an object)!
        /// </param>
        public JoinMap(IDbProviderTable table, IDbProviderColumn principleKeyColumn, IDbProviderTable referencedTable, IDbProviderColumn foreignKeyColumn, int index, bool isRightJoin) : base()
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            PrincipleKeyColumn = principleKeyColumn ?? throw new ArgumentNullException(nameof(principleKeyColumn));
            ReferencedTable = referencedTable ?? throw new ArgumentNullException(nameof(referencedTable));
            ForeignKeyColumn = foreignKeyColumn ?? throw new ArgumentNullException(nameof(foreignKeyColumn));
            Index = index;
            IsRightJoin = isRightJoin;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "One " + Table.TableName + " with many " + ReferencedTable.TableName;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(JoinMap other)
        {
            if (other == null)
                return false;

            return Table == other.Table && PrincipleKeyColumn == other.PrincipleKeyColumn && ReferencedTable == other.ReferencedTable && ForeignKeyColumn == other.ForeignKeyColumn && Index == other.Index;
        }

        /// <summary>
        /// Creates and returns a <see cref="JoinMapDataModel"/> from the current <see cref="JoinMap"/>
        /// </summary>
        /// <returns></returns>
        public JoinMapDataModel ToDataModel() => new JoinMapDataModel()
        {
            TableName = Table.TableName,
            PrincipleKeyColumnName = PrincipleKeyColumn.ColumnName,
            ReferencedTableName = ReferencedTable.TableName,
            ForeignKeyColumnName = ForeignKeyColumn.ColumnName,
            Index = Index,
            IsInverted = IsRightJoin
        };

        #endregion
    }

    /// <summary>
    /// Represents a <see cref="JoinMap"/>
    /// </summary>
    public class JoinMapDataModel
    {
        #region Public Properties

        /// <summary>
        /// The main table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The principle key column of the table with the specified <see cref="TableName"/>
        /// </summary>
        public string PrincipleKeyColumnName { get; set; }

        /// <summary>
        /// The referenced table name
        /// </summary>
        public string ReferencedTableName { get; set; }

        /// <summary>
        /// The foreign key column of the table with the specified <see cref="ReferencedTableName"/>
        /// </summary>
        public string ForeignKeyColumnName { get; set; }

        /// <summary>
        /// The index level of the join.
        /// NOTE: If the index is set to zero, then the table with the <see cref="TableName"/> is the root table!
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// A flag indicating whether we have a foreign key to primary key join (right join) or not.
        /// NOTE: When this is set to <see cref="false"/> a left join is applied (the navigation property of the root type is an <see cref="IEnumerable{T}"/>)!
        /// NOTE: When this is set to <see cref="true"/> a right join is applied (the navigation property of the root type is an object)!
        /// </summary>
        public bool IsInverted { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public JoinMapDataModel() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => TableName + " -> " + ReferencedTableName;

        #endregion
    }
}
