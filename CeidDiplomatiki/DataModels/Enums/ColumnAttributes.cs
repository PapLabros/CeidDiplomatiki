using System;
using System.Collections.Generic;
using System.Linq;

using static Atom.Personalization.Constants;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Implements a set of predefined <see cref="ColumnAttribute"/>s
    /// </summary>
    public static class ColumnAttributes
    {
        #region Constants

        public const string NamesGroupId = "Names";

        public const string NumericValuesGroupId = "Numerics";

        public const string CommunicationMeansGroupId = "CommunicationMeans";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the available column attributes
        /// </summary>
        public static Lazy<IEnumerable<ColumnAttribute>> Data { get; } = new Lazy<IEnumerable<ColumnAttribute>>(() =>
        {
            // Get all the properties that have a getter
            var properties = typeof(ColumnAttributes).GetProperties().Where(x => x.CanRead && x.PropertyType == typeof(ColumnAttribute));

            // Return the values
            return properties.Select(x => (ColumnAttribute)x.GetValue(null)).ToList();
        });

        /// <summary>
        /// Represents a standard column
        /// </summary>
        public static ColumnAttribute None { get; } = new ColumnAttribute("None", "None", Blue);

        /// <summary>
        /// Represents a phone number column
        /// </summary>
        public static ColumnAttribute PhoneNumber { get; } = new ColumnAttribute("PhoneNumber", "Phone number", Red, CommunicationMeansGroupId);

        /// <summary>
        /// Represents an email column
        /// </summary>
        public static ColumnAttribute Email { get; } = new ColumnAttribute("Email", "Email", Green, CommunicationMeansGroupId);

        /// <summary>
        /// Represents a percentage column
        /// </summary>
        public static ColumnAttribute Percentage { get; } = new ColumnAttribute("Percentage", "Percentage", DarkTangerine, NumericValuesGroupId);

        /// <summary>
        /// Represents a money column
        /// </summary>
        public static ColumnAttribute Money { get; } = new ColumnAttribute("Money", "Money", DarkTangerine, NumericValuesGroupId);

        /// <summary>
        /// Represents a column that stores the total amount
        /// </summary>
        public static ColumnAttribute Total { get; } = new ColumnAttribute("Total", "Total", DarkTangerine);

        /// <summary>
        /// Represents a column that stores the tax amount
        /// </summary>
        public static ColumnAttribute TotalTax { get; } = new ColumnAttribute("TotalTax", "Total tax", DarkTangerine);

        /// <summary>
        /// Represents a column that stores the amount that was paid
        /// </summary>
        public static ColumnAttribute TotalPaid { get; } = new ColumnAttribute("TotalPaid", "Total paid", DarkTangerine);

        /// <summary>
        /// Represents a column that stores colors using the HEX format
        /// </summary>
        public static ColumnAttribute HEXColor { get; } = new ColumnAttribute("HEXColor", "HEX color", BlazeOrange);

        /// <summary>
        /// Represents a column that stores the first name of a person
        /// </summary>
        public static ColumnAttribute FirstName { get; } = new ColumnAttribute("FirstName", "First name", Gray, NamesGroupId);

        /// <summary>
        /// Represents a column that stores the last name of a person
        /// </summary>
        public static ColumnAttribute LastName { get; } = new ColumnAttribute("LastName", "Last name", Gray, NamesGroupId);

        /// <summary>
        /// Represents a column that stores the full name of a person
        /// </summary>
        public static ColumnAttribute FullName { get; } = new ColumnAttribute("FullName", "Full name", Gray, NamesGroupId);

        /// <summary>
        /// Represents a column that stores a note
        /// </summary>
        public static ColumnAttribute Note { get; } = new ColumnAttribute("Note", "Note", MaastrichtBlue);

        /// <summary>
        /// Represents a column that stores the date when an operation has/will started/start
        /// </summary>
        public static ColumnAttribute DateStart { get; } = new ColumnAttribute("DateStart", "Date start", RoyalPurple);

        /// <summary>
        /// Represents a column that stores the date when an operation has/will ended/end
        /// </summary>
        public static ColumnAttribute DateEnd { get; } = new ColumnAttribute("DateEnd", "Date end", RoyalPurple);

        #endregion
    }
}
