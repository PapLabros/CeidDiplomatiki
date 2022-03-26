
using Atom.Core;
using Atom.Windows.Controls;

using System.IO;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Options component for an SQLite database
    /// </summary>
    public class SQLiteOptionsComponent : BaseDatabaseOptionsComponent<SQLiteOptionsDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SQLiteOptionsComponent(SQLiteOptionsDataModel options) : base(options)
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseDatabaseOptionsComponent{TOptions}.OptionsForm"/>
        /// </summary>
        /// <returns></returns>
        protected override DataForm<SQLiteOptionsDataModel> CreateForm()
        {
            return new DataForm<SQLiteOptionsDataModel>()
                .ShowCustomFormInput(x => x.DirectoryPath, (dataForm, propertyInfo) => new DirectoryInputFormInput(dataForm, propertyInfo), false, "Directory path", true)
                .ShowInput(x => x.DatabaseName, "Database name", true, ".db extension is added as a suffix!");
        }

        /// <summary>
        /// Checks whether the specified <paramref name="connectionString"/> contained in the specified <paramref name="options"/> is valid or not
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected async override Task<IFailable> IsConnectionValidAsync(SQLiteOptionsDataModel options, string connectionString)
        {
            // Remove the data source part
            connectionString = connectionString.Replace("Data Source=", "");

            // If the directory exists...
            if (Directory.Exists(connectionString))
                // Return success
                return await Task.FromResult(new Failable());
            // Else
            else
                // Return fail
                return await Task.FromResult(new Failable() { ErrorMessage = "The directory doesn't exist!" });
        }

        /// <summary>
        /// Removes the database part from the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override string RemoveDatabaseFromConnectionString(SQLiteOptionsDataModel options, string connectionString)
            => connectionString;

        #endregion
    }
}
