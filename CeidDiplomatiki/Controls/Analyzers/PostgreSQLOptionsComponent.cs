
using Atom.Core;
using Atom.Windows.Controls;

using Npgsql;

using System;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Options component for a PostgreSQL database
    /// </summary>
    public class PostgreSQLOptionsComponent : BaseDatabaseOptionsComponent<PostgreSQLOptionsDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="options">The options</param>
        public PostgreSQLOptionsComponent(PostgreSQLOptionsDataModel options) : base(options)
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseDatabaseOptionsComponent{TOptions}.OptionsForm"/>
        /// </summary>
        /// <returns></returns>
        protected override DataForm<PostgreSQLOptionsDataModel> CreateForm()
        {
            return new DataForm<PostgreSQLOptionsDataModel>()
              .ShowInput(x => x.Host, "Host", true)
              .ShowInput(x => x.Port, "Port", true)
              .ShowInput(x => x.UserName, "Username", true)
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
        protected async override Task<IFailable> IsConnectionValidAsync(PostgreSQLOptionsDataModel options, string connectionString)
        {
            // Create the result
            var result = new Failable();

            // Create a connection
            var connection = new NpgsqlConnection(connectionString);

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
        protected override string RemoveDatabaseFromConnectionString(PostgreSQLOptionsDataModel options, string connectionString) 
            => connectionString.Replace($"Database={options.DatabaseName};", string.Empty);

        #endregion
    }
}
