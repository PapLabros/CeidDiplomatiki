using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;

namespace CeidDiplomatiki
{
    /// <summary>
    /// An implementation of the <see cref="IDbContextOptionsExtension"/> used for defining a tables prefix
    /// </summary>
    public class PrefixDbContextOptions : IDbContextOptionsExtension
    {
        #region Public Properties

        /// <summary>
        /// Information/metadata about the extension.
        /// </summary>
        public DbContextOptionsExtensionInfo Info { get; }

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
        public PrefixDbContextOptions(string prefix) : base()
        {
            Prefix = prefix;
            Info = new ExtensionInfo(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gives the extension a chance to validate that all options in the extension are
        /// valid. Most extensions do not have invalid combinations and so this will be a
        /// no-op. If options are invalid, then an exception should be thrown.
        /// </summary>
        /// <param name="options"></param>
        public void Validate(IDbContextOptions options)
        {
        }

        /// <summary>
        /// Adds the services required to make the selected options work. This is used when
        /// there is no external System.IServiceProvider and EF is maintaining its own service
        /// provider internally. This allows database providers (and other extensions) to
        /// register their required services when EF is creating an service provider.
        /// </summary>
        /// <param name="services">The collection to add services to.</param>
        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton(new PrefixAccessor(Prefix));
        }

        #endregion

        #region Private Methods

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            #region Constructors

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="extension">The extension.</param>
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// True, since this is a database provider base class.
            /// </summary>
            public override bool IsDatabaseProvider
                => false;

            /// <summary>
            /// A message fragment for logging typically containing information about
            /// any useful non-default options that have been configured.
            /// </summary>
            public override string LogFragment => string.Empty;

            /// <summary>
            /// Returns a hash code created from any options that would cause a new <see cref="IServiceProvider"/>
            /// to be needed. For example, if the options affect a singleton service. However
            /// most extensions do not have any such options and should return zero.
            /// </summary>
            /// <returns></returns>
            public override long GetServiceProviderHashCode() => 0;

            /// <summary>
            /// Populates a dictionary of information that may change between uses of the extension
            /// such that it can be compared to a previous configuration for this option and
            /// differences can be logged. The dictionary key should be prefixed by the extension
            /// name. For example, "SqlServer:".
            /// </summary>
            /// <param name="debugInfo"></param>
            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {

            }

            #endregion
        }

        #endregion
    }
}
