using Atom.Core;

using System;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the <see cref="ColumnValueType"/> that best represents the specified <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public static ColumnValueType ToColumnValueType(this Type type)
        {
            // If the data type of the column is a number...
            if (type.IsNumber())
                // Return number
                return ColumnValueType.Number;

            // If the data type of the column is a date...
            if (type.IsDate())
                // Return date time
                return ColumnValueType.DateTime;

            // If the data type of the column is a string...
            if (type == typeof(string))
                // Return text
                return ColumnValueType.Text;

            // Return other
            return ColumnValueType.Other;
        }
    }
}
