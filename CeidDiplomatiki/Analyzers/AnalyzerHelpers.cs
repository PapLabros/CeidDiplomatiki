using Atom.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The helper method related 
    /// </summary>
    public static class AnalyzerHelpers
    {
        /// <summary>
        /// Wraps the <paramref name="method"/> into an asynchronous version of itself
        /// </summary>
        /// <typeparam name="TValue">The type of the provider value</typeparam>
        /// <param name="method">The method that fetches the results</param>
        /// <returns></returns>
        public static Task<IFailable<IEnumerable<TValue>>> GetDatabaseCollectionAsync<TValue>(Func<IFailable<IEnumerable<TValue>>> method) 
            => Task.Factory.StartNew(method);
    }
}
