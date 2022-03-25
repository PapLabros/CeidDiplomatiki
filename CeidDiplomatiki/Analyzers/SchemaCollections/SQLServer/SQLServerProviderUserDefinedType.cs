using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a SQL Server user defined type
    /// </summary>
    public class SQLServerProviderUserDefinedType
    {
        #region Public Properties

        /// <summary>
        /// The name of the file for the assembly.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// The class name for the assembly.
        /// </summary>
        public string UDTName { get; set; }

        /// <summary>
        /// Major Version Number.
        /// </summary>
        public object VersionMajor { get; set; }

        /// <summary>
        /// Minor Version Number.
        /// </summary>
        public object VersionMinor { get; set; }

        /// <summary>
        /// Build Number.
        /// </summary>
        public object VersionBuild { get; set; }

        /// <summary>
        /// Revision Number.
        /// </summary>
        public object VersionRevision { get; set; }

        /// <summary>
        /// The culture information associated with this UDT.
        /// </summary>
        public object CultureInfo { get; set; }

        /// <summary>
        /// The public key used by this Assembly.
        /// </summary>
        public object PublicKey { get; set; }

        /// <summary>
        /// Specifies whether length of type is always same as max_length.
        /// </summary>
        public bool IsFixedLength { get; set; }

        /// <summary>
        /// Maximum length of type in bytes.
        /// </summary>
        public short MaxLength { get; set; }

        /// <summary>
        /// The date the assembly was created/registered.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The friendly name for the permission-set/security-level for the assembly.
        /// </summary>
        public string PermissionSetDescription { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="row">The data row</param>
        internal SQLServerProviderUserDefinedType(DataRow row) : base()
        {
            AssemblyName = row.GetString(0);
            UDTName = row.GetString(1);
            VersionMajor = row[2];
            VersionMinor = row[3];
            VersionBuild = row[4];
            VersionRevision = row[5];
            CultureInfo = row[6];
            PublicKey = row[7];
            IsFixedLength = row.GetBool(8);
            MaxLength = row.GetShort(9);
            DateCreated = row.GetDateTime(10);
            PermissionSetDescription = row.GetString(11);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => AssemblyName;

        #endregion
    }
}
