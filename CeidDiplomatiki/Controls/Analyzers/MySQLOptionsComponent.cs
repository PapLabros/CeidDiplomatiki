
using Atom.Core;
using Atom.Windows.Controls;

using MySql.Data.MySqlClient;

using System;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Options component for a MySQL database
    /// </summary>
    public class MySQLOptionsComponent : BaseDatabaseOptionsComponent<MySQLOptionsDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MySQLOptionsComponent(MySQLOptionsDataModel options) : base(options)
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseDatabaseOptionsComponent{TOptions}.OptionsForm"/>
        /// </summary>
        /// <returns></returns>
        protected override DataForm<MySQLOptionsDataModel> CreateForm()
        {
            return new DataForm<MySQLOptionsDataModel>()
                .ShowInput(x => x.Server, "Server/Ip", true)
                .ShowInput(x => x.Port, "Port")
                .ShowInput(x => x.UserId, "Username", true)
                .ShowInput(x => x.Password, "Password", true)
                .ShowInput(x => x.DatabaseName, "Database name", true);
        }

        /// <summary>
        /// Checks whether the specified valid <paramref name="connectionString"/> contained in the specified 
        /// <paramref name="options"/> can open a connection or not
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override async Task<IFailable> IsConnectionValidAsync(MySQLOptionsDataModel options, string connectionString)
        {
            // Create the result
            var result = new Failable();

            // Create a connection
            var connection = new MySqlConnection(connectionString);

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
        /// Further formats the already valid connection string.
        /// Ex. Remove the MySQL default port connection string part.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override string FormatValidConnectionString(string connectionString)
        {
            // If the connection string contains the default port...
            if (connectionString.Contains($"Port={RelationalConstants.DefaultMySQLServerPort};"))
                // Remove it
                connectionString = connectionString.Replace($"Port={RelationalConstants.DefaultMySQLServerPort};", string.Empty);

            // Return the connection string
            return connectionString;
        }

        /// <summary>
        /// Removes the database part from the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected override string RemoveDatabaseFromConnectionString(MySQLOptionsDataModel options, string connectionString)
            => connectionString.Replace($"Database={options.DatabaseName};", string.Empty);

        #endregion
    }
}
