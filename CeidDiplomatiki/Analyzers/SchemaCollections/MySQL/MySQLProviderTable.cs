using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a MySQL table
    /// </summary>
    public class MySQLProviderTable : IDbProviderTable
    {
        #region Public Properties

        /// <summary>
        /// Catalog of the table.
        /// </summary>
        public string TableCatalog { get; set; }

        /// <summary>
        /// Schema that contains the table.
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Type of table. Can be VIEW or BASE TABLE.
        /// </summary>
        public string TableType { get; set; }

        public string Engine { get; set; }

        public ulong Version { get; set; }

        public string RowFormat { get; set; }

        public ulong TableRows { get; set; }

        public ulong AverageRowLength { get; set; }

        public ulong DataLength { get; set; }

        public ulong MaxDataLength { get; set; }

        public ulong IndexLength { get; set; }

        public ulong DataFree { get; set; }

        public ulong? AutoIncrement { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? CheckTime { get; set; }

        public string TableCollation { get; set; }

        public string CheckSum { get; set; }

        public string CreateOption { get; set; }

        public string TableComment { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal MySQLProviderTable(DataRow row)
        {
            TableCatalog = row.GetString(0);
            TableSchema = row.GetString(1);
            TableName = row.GetString(2);
            TableType = row.GetString(3);
            Engine = row.GetString(4);

            // If the database is MariaDb...
            if (row.Table.Columns[5].DataType == typeof(ulong))
                Version = row.GetUnsignedLong(5);
            // If the database is MySql...
            else
                Version = (ulong)row.GetLong(5);

            RowFormat = row.GetString(6);
            TableRows = row.GetUnsignedLong(7);
            AverageRowLength = row.GetUnsignedLong(8);
            DataLength = row.GetUnsignedLong(9);
            MaxDataLength = row.GetUnsignedLong(12);
            IndexLength = row.GetUnsignedLong(11);
            DataFree = row.GetUnsignedLong(12);
            AutoIncrement = row.GetDbNullableUnsignedLong(13);
            CreateTime = row.GetDateTime(14);
            UpdateTime = row.GetDbNullableDateTime(15);
            CheckTime = row.GetDbNullableDateTime(16);
            TableCollation = row.GetString(17);
            CheckSum = row.GetDbNullableString(18);
            CreateOption = row.GetString(19);
            TableComment = row.GetString(20);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => TableName;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is IDbProviderTable dbProviderTable)
                return Equals(dbProviderTable);

            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns></returns>
        public bool Equals(IDbProviderTable other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return TableCatalog == other.TableCatalog &&
                   TableSchema == other.TableSchema &&
                   TableName == other.TableName &&
                   TableType == other.TableType;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCodeHelpers.Combine(TableCatalog, TableCatalog, TableSchema, TableName, TableType, Engine, Version, 
                RowFormat, TableRows, AverageRowLength, DataLength, MaxDataLength, IndexLength, DataFree, AutoIncrement, 
                CreateTime, UpdateTime, CheckTime, TableCollation, CheckSum, CreateOption, TableComment);

        #endregion
    }
}
