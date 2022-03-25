using Atom.Core;

using System;
using System.Data;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Extension methods for <see cref="DataRow"/>
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Gets a <see cref="string"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static string GetString(this DataRow row, int index)
            => (string)row[index];

        /// <summary>
        /// Gets a <see cref="int"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static int GetInt(this DataRow row, int index)
            => (int)row[index];

        /// <summary>
        /// Gets a <see cref="uint"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static uint GetUnsignedInt(this DataRow row, int index)
            => (uint)row[index];

        /// <summary>
        /// Gets a <see cref="double"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static double GetDouble(this DataRow row, int index)
          => (double)row[index];

        /// <summary>
        /// Gets a <see cref="short"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static short GetShort(this DataRow row, int index)
            => (short)row[index];

        /// <summary>
        /// Gets a <see cref="long"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static long GetLong(this DataRow row, int index)
            => (long)row[index];

        /// <summary>
        /// Gets a <see cref="ulong"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static ulong GetUnsignedLong(this DataRow row, int index)
            => (ulong)row[index];

        /// <summary>
        /// Gets a <see cref="decimal"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static decimal GetDecimal(this DataRow row, int index)
           => (decimal)row[index];

        /// <summary>
        /// Gets a <see cref="bool"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static bool GetBool(this DataRow row, int index)
        {
            // Get the value
            var value = row[index];

            // If the value is string...
            if (value is string stringValue)
            {
                // Transform the string to lower
                stringValue = stringValue.ToLower();

                if (stringValue == "yes")
                    return true;

                if (stringValue == "no")
                    return false;
            }

            return (bool)row[index];
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static DateTime GetDateTime(this DataRow row, int index)
            => (DateTime)row[index];

        /// <summary>
        /// Gets a <see cref="string"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public static byte GetByte(this DataRow row, int index)
            => (byte)row[index];

        /// <summary>
        /// Gets a <see cref="byte"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static string GetDbNullableString(this DataRow row, int index, string fallBackValue = null)
            => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString();

        /// <summary>
        /// Gets a possibly null <see cref="int"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static int? GetDbNullableInt(this DataRow row, int index, int? fallBackValue = null)
            => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString().ToInt();

        /// <summary>
        /// Gets a possibly null <see cref="double"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static double? GetDbNullableDouble(this DataRow row, int index, double? fallBackValue = null)
          => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString().ToDouble();

        /// <summary>
        /// Gets a possibly null <see cref="short"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static short? GetDbNullableShort(this DataRow row, int index, short? fallBackValue = null)
            => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString().ToShort();

        /// <summary>
        /// Gets a possibly null <see cref="long"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static long? GetDbNullableLong(this DataRow row, int index, long? fallBackValue = null)
            => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString().ToLong();

        /// <summary>
        /// Gets a possibly null <see cref="ulong"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static ulong? GetDbNullableUnsignedLong(this DataRow row, int index, ulong? fallBackValue = null)
            => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString().ToUnsignedLong();

        /// <summary>
        /// Gets a possibly null <see cref="decimal"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static decimal? GetDbNullableDecimal(this DataRow row, int index, decimal? fallBackValue = null)
           => Convert.IsDBNull(row[index]) ? fallBackValue : row[index].ToString().ToDecimal();

        /// <summary>
        /// Gets a possibly null <see cref="bool"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static bool? GetDbNullableBool(this DataRow row, int index, bool? fallBackValue = null)
           => Convert.IsDBNull(row[index]) ? fallBackValue : (bool)row[index];

        /// <summary>
        /// Gets a possible null <see cref="DateTime"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallbackValue">The fallback value</param>
        /// <returns></returns>
        public static DateTime? GetDbNullableDateTime(this DataRow row, int index, DateTime? fallbackValue = null)
            => Convert.IsDBNull(row[index]) ? fallbackValue : (DateTime)row[index];

        /// <summary>
        /// Gets a possibly null <see cref="byte"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="index">The index</param>
        /// <param name="fallBackValue">The fallback value</param>
        /// <returns></returns>
        public static byte? GetDbNullableByte(this DataRow row, int index, byte? fallBackValue = null)
            => Convert.IsDBNull(row[index]) ? fallBackValue : (byte?)row[index];
    }
}
