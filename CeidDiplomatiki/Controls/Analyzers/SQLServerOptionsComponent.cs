
using Atom.Core;
using Atom.Windows.Controls;

using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Options component for SQLServer database
    /// </summary>
    public class SQLServerOptionsComponent : BaseDatabaseOptionsComponent<SQLServerOptionsDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SQLServerOptionsComponent(SQLServerOptionsDataModel options) : base(options)
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseDatabaseOptionsComponent{TOptions}.OptionsForm"/>
        /// </summary>
        /// <returns></returns>
        protected override DataForm<SQLServerOptionsDataModel> CreateForm()
        {
            return new DataForm<SQLServerOptionsDataModel>()
                .ShowInput(x => x.Server, "Server/Ip", true)
                .ShowInput(x => x.Port, "Port", true)
                .ShowInput(x => x.UserId, "Username")
                .ShowInput(x => x.Password, "Password")
                .ShowInput(x => x.IntegratedSecurity, "Integrated security")
                .ShowInput(x => x.DatabaseName, "Database name", true);
        }

        /// <summary>
        /// Checks whether the specified <paramref name="connectionString"/> contained in the specified <paramref name="options"/> is valid or not
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected async override Task<IFailable> IsConnectionValidAsync(SQLServerOptionsDataModel options, string connectionString)
        {
            // Create the result
            var result = new Failable();

            // Create a connection
            var connection = new SqlConnection(connectionString);

            try
            {
                // Attempt to open the connection
                await connection.OpenAsync();
                // Return success
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                // Set the error
                result.ErrorMessage = ex.Message;

                // Return fail
                return await Task.FromResult(result);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Removes the database part from the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override string RemoveDatabaseFromConnectionString(SQLServerOptionsDataModel options, string connectionString)
            => connectionString.Replace($"Database={options.DatabaseName}", string.Empty);

        #endregion
    }
}
