using Atom.Core;
using Atom.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    public class CeidDiplomatikiPropertyShortcodesComponent : PropertyShortcodeComponent<CeidDiplomatikiPropertyShortcodesComponent, PropertyShortcodeValueTranslatorComponent, PropertyShortcodeFilterComponent>
    {
        #region Constructors

        /// <summary>
        /// Shortcode based constructor
        /// </summary>
        /// <param name="shortcode">The shortcode</param>
        /// <param name="instanceType">The type of the instance that contains all the shortcode information</param>
        public CeidDiplomatikiPropertyShortcodesComponent(PropertyShortcode shortcode, Type instanceType) : base(shortcode, instanceType)
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="instance">The base instance, that's the one that it or any of its sub-properties contain the <see cref="PropertyShortcodeComponent{TPropertyShortcodeComponent, TPropertyShortcodeValueTranslatorComponent, TPropertyShortcodeFilterComponent}.Property"/></param>
        /// <param name="instanceType">The type of the instance that contains all the shortcode information</param>
        /// <param name="propertyInfo">The property info</param>
        /// <param name="parentPath">The parent path</param>
        public CeidDiplomatikiPropertyShortcodesComponent(object instance, Type instanceType, PropertyInfo propertyInfo, string parentPath) : base(instance, instanceType, propertyInfo, parentPath)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns a property shortcode filter component
        /// </summary>
        /// <param name="nonEnumerableType">The type of the instance</param>
        /// <returns></returns>
        protected override PropertyShortcodeFilterComponent CreateFilterComponent(Type nonEnumerableType) 
            => new PropertyShortcodeFilterComponent(nonEnumerableType);

        /// <summary>
        /// Creates and returns a property shortcode filter component
        /// </summary>
        /// <param name="nonEnumerableType">The type of the instance</param>
        /// <param name="filter">The filter</param>
        /// <returns></returns>
        protected override PropertyShortcodeFilterComponent CreateFilterComponent(PropertyShortcodeFilter filter, Type nonEnumerableType) 
            => new PropertyShortcodeFilterComponent(filter, nonEnumerableType);

        /// <summary>
        /// Creates and returns a shortcode component
        /// </summary>
        /// <param name="instance">The instance of the value of the property if any</param>
        /// <param name="instanceType">The type of the instance that contains all the shortcode information</param>
        /// <param name="propertyInfo">The property info</param>
        /// <param name="parentPath">The parent path</param>
        /// <returns></returns>
        protected override CeidDiplomatikiPropertyShortcodesComponent CreateShortcodeComponent(object instance, Type instanceType, PropertyInfo propertyInfo, string parentPath) 
            => new CeidDiplomatikiPropertyShortcodesComponent(instance, instanceType, propertyInfo, parentPath);

        /// <summary>
        /// Creates and returns a value translator component
        /// </summary>
        /// <param name="propertyInfo">The property of the shortcode that the translator is created for</param>
        /// <returns></returns>
        protected override PropertyShortcodeValueTranslatorComponent CreateValueTranslatorComponent(PropertyInfo propertyInfo) 
            => new PropertyShortcodeValueTranslatorComponent(propertyInfo);

        /// <summary>
        /// Creates and returns a value translator component
        /// </summary>
        /// <param name="translator">The translator</param>
        /// <param name="propertyInfo">The property of the shortcode that the translator is created for</param>
        /// <returns></returns>
        protected override PropertyShortcodeValueTranslatorComponent CreateValueTranslatorComponent(PropertyShortcodeValueTranslator translator, PropertyInfo propertyInfo) 
            => new PropertyShortcodeValueTranslatorComponent(translator, propertyInfo);

        /// <summary>
        /// Gets the custom shortcodes related to the component
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown when unable to retrieve the shortcodes</exception>
        protected override Task<IEnumerable<PropertyShortcode>> GetCustomShortcodesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the addition of a custom shortcode.
        /// NOTE: This method should be overridden to attach functionality to the shortcode
        ///       after its addition (ex. the data storage save) has been completed!
        ///       Ex. Attach a context menu to the shortcode.
        /// NOTE: This method is called both when a custom shortcode is created and on the
        ///       <see cref="PropertyShortcodeComponent{TPropertyShortcodeComponent, TPropertyShortcodeValueTranslatorComponent, TPropertyShortcodeFilterComponent}.OnInitialized(EventArgs)"/>
        ///       method where all the pre-existing custom shortcodes are retrieved and placed
        ///       in the container!
        /// </summary>
        /// <param name="shortcode">The shortcode</param>
        protected override void OnCustomShortcodeAdded(PropertyShortcode shortcode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the creation of a shortcode.
        /// NOTE: Override the <see cref="PropertyShortcodeComponent{TPropertyShortcodeComponent, TPropertyShortcodeValueTranslatorComponent, TPropertyShortcodeFilterComponent}.CreateShortcode(ShortcodeCreationEventArgs)"/>
        ///       method to provide a custom shortcode implementation!
        /// </summary>
        /// <param name="shortcode">The shortcode</param>
        protected override void OnShortcodeCreated(PropertyShortcode shortcode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
