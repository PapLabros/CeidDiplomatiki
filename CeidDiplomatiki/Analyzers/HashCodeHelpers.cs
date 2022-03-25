
using System;
using System.Data;
using System.Linq;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The helper methods for handling hash codes
    /// </summary>
    public static class HashCodeHelpers
    {
        /// <summary>
        /// Combines <paramref name="items"/> into a hash code.
        /// </summary>
        /// <param name="items">The items</param>
        /// <returns></returns>
        public static int Combine(params object[] items)
        {
            var hash = new HashCode();

            foreach (var item in items.Where(x => x != null))
                hash.Add(item.GetHashCode());

            return hash.ToHashCode();
        }
    }
}
