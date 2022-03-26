
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Provides access to the tables <see cref="Prefix"/>
    /// </summary>
    public class PrefixAccessor
    {
        #region Public Properties

        /// <summary>
        /// The prefix
        /// </summary>
        public string Prefix { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="prefix">The prefix</param>
        public PrefixAccessor(string prefix) : base()
        {
            Prefix = prefix;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to get the prefix from one of the services injected at the specified <paramref name="dbContext"/>
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <returns></returns>
        public static string GetPrefixFromDbContextOrEmpty(DbContext dbContext)
        {
            // Get the service provider
            var serviceProvider = ((IInfrastructure<IServiceProvider>)dbContext).Instance;

            // Get the options
            var options = serviceProvider.GetService<IDbContextOptions>();

            var prefixExtension = options?.FindExtension<PrefixDbContextOptions>();

            // Return the prefix
            return prefixExtension?.Prefix ?? string.Empty;
        }

        #endregion
    }
}
