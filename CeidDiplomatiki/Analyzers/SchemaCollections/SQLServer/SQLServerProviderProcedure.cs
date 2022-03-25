using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server procedure
    /// </summary>
    public class SQLServerProviderProcedure
    {
        #region Public Properties

        /// <summary>
        /// Specific name for the catalog.
        /// </summary>
        public string SpecificCatalog { get; set; }

        /// <summary>
        /// Specific name of the schema.
        /// </summary>
        public string SpecificSchema { get; set; }

        /// <summary>
        /// Specific name.
        /// </summary>
        public string SpecificName { get; set; }

        /// <summary>
        /// Catalog the stored procedure belongs to.
        /// </summary>
        public string RoutineCatalog { get; set; }

        /// <summary>
        /// Schema that contains the stored procedure.
        /// </summary>
        public string RoutineSchema { get; set; }

        /// <summary>
        /// Name of the stored procedure.
        /// </summary>
        public string RoutineName { get; set; }

        /// <summary>
        /// Returns PROCEDURE for stored procedures and FUNCTION for functions.
        /// </summary>
        public string RoutineType { get; set; }

        /// <summary>
        /// Date-time the procedure was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The last date-time the procedure was modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderProcedure(DataRow row) : base()
        {
            SpecificCatalog = row.GetString(0);
            SpecificSchema = row.GetString(1);
            SpecificName = row.GetString(2);
            RoutineCatalog = row.GetString(3);
            RoutineSchema = row.GetString(4);
            RoutineName = row.GetString(5);
            RoutineType = row.GetString(6);
            DateCreated = row.GetDateTime(7);
            DateModified = row.GetDateTime(8);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => RoutineName;

        #endregion
    }
}
