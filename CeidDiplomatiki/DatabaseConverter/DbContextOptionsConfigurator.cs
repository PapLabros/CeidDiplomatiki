
using Microsoft.EntityFrameworkCore;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Provides abstractions for a database context options configurator
    /// </summary>
    public abstract class DbContextOptionsConfigurator
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbContextOptionsConfigurator() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures the specified <paramref name="builder"/>
        /// </summary>
        /// <param name="builder">The builder</param>
        /// <param name="provider">The provider</param>
        public abstract void Configure(DbContextOptionsBuilder builder, SQLDatabaseProvider provider);

        #endregion
    }
}
