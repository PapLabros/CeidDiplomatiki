using Atom.Core;
using System;
using System.Collections.Generic;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a property shortcode
    /// </summary>
    public class CeidDiplomatikiPropertyShortcodeDataModel : PropertyShortcodeDataModel<CeidDiplomatikiPropertyShortcodeDataModel, CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel,CeidDiplomatikiPropertyShortcodeFilterDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CeidDiplomatikiPropertyShortcodeDataModel() : base()
        {

        }

        /// <summary>
        /// Shortcode based constructor
        /// </summary>
        /// <param name="shortcode">The shortcode</param>
        /// <param name="translatorsImplementationFactory">Creates and returns a translator data model from a shortcode translator</param>
        /// <param name="filtersImplementaitonFactory">Creates and returns a filter data model from a shortcode filter</param>
        public CeidDiplomatikiPropertyShortcodeDataModel(PropertyShortcode shortcode, Func<PropertyShortcodeValueTranslator, CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel> translatorsImplementationFactory, Func<PropertyShortcodeFilter, CeidDiplomatikiPropertyShortcodeFilterDataModel> filtersImplementaitonFactory) : base(shortcode, translatorsImplementationFactory, filtersImplementaitonFactory)
        {
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="propertyName">The name of the related property</param>
        /// <param name="fallbackValue">The value that is used when the formula returns a null or empty string</param>
        /// <param name="value">A string value that represents the shortcode formula</param>
        /// <param name="newLineAfterEveryItem">
        /// If this is set to true a new line character is set after every item.
        /// NOTE: This applies only to enumerables!
        /// </param>
        /// <param name="name">The name of the shortcode</param>
        /// <param name="slug">The shortcode slug</param>
        /// <param name="color">The color that represents the shortcode</param>
        /// <param name="propertyPath">
        /// The property path that navigates to the property whose non enumerable type contains the property with the specified <paramref name="propertyName"/>.
        /// NOTE: This is used for shortcodes of a property of a sub-property and so on, of the instance that contains the property with the specified <see cref="PropertyShortcodeDataModel{TPropertyShortcodeDataModel, TPropertyShortcodeValueTranslatorDataModel, TPropertyShortcodePropertyValueFilterDataModel}.PropertyName"/>!
        /// NOTE: This is set to <see cref="null"/> when the property with the specified <see cref="PropertyShortcodeDataModel{TPropertyShortcodeDataModel, TPropertyShortcodeValueTranslatorDataModel, TPropertyShortcodePropertyValueFilterDataModel}.PropertyName"/> is a property contained in the instance of the target type!
        /// Ex: FirstLevelProperty.SecondLevelProperty
        /// </param>
        /// <param name="translators">The translators</param>
        /// <param name="filters">The filters</param>
        public CeidDiplomatikiPropertyShortcodeDataModel(string propertyName, string propertyPath, string value, bool newLineAfterEveryItem, string fallbackValue, string name, string slug, string color, IEnumerable<CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel> translators, IEnumerable<CeidDiplomatikiPropertyShortcodeFilterDataModel> filters) : base(propertyName, propertyPath, value, newLineAfterEveryItem, fallbackValue, name, slug, color, translators, filters)
        {
        }

        #endregion
    }
}
