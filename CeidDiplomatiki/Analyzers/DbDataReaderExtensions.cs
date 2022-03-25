using Atom.Core;

using System;
using System.Data.Common;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Extension methods for <see cref="DbDataReader"/>
    /// </summary>
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// Gets a possibly nullable <see cref="string"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static string GetDbNullableString(this DbDataReader reader, string name, string fallBackValue = null)
            => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString();

        /// <summary>
        /// Gets a possibly nullable <see cref="int"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static int? GetDbNullableInt(this DbDataReader reader, string name, int? fallBackValue = null)
            => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString().ToInt();

        /// <summary>
        /// Gets a possibly nullable <see cref="double"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static double? GetDbNullableDouble(this DbDataReader reader, string name, double? fallBackValue = null)
          => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString().ToDouble();

        /// <summary>
        /// Gets a possibly nullable <see cref="short"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static short? GetDbNullableShort(this DbDataReader reader, string name, short? fallBackValue = null)
            => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString().ToShort();

        /// <summary>
        /// Gets a possibly nullable <see cref="long"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static long? GetDbNullableLong(this DbDataReader reader, string name, long? fallBackValue = null)
            => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString().ToLong();

        /// <summary>
        /// Gets a possibly null <see cref="ulong"/>
        /// </summary>
        /// <param name="reader">The row</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static ulong? GetDbNullableUnsignedLong(this DbDataReader reader, string name, ulong? fallBackValue = null)
            => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString().ToUnsignedLong();

        /// <summary>
        /// Gets a possibly nullable <see cref="decimal"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="name">The name of the column</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static decimal? GetDbNullableDecimal(this DbDataReader reader, string name, decimal? fallBackValue = null)
           => Convert.IsDBNull(reader[name]) ? fallBackValue : reader[name].ToString().ToDecimal();
    }
}
