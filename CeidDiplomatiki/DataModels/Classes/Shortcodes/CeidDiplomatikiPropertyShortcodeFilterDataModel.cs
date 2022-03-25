using Atom.Core;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a property shortcode filter data model
    /// </summary>
    public class CeidDiplomatikiPropertyShortcodeFilterDataModel : PropertyShortcodeFilterDataModel<CeidDiplomatikiPropertyShortcodeDataModel, CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel, CeidDiplomatikiPropertyShortcodeFilterDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CeidDiplomatikiPropertyShortcodeFilterDataModel() : base()
        {

        }

        /// <summary>
        /// Filter based constructor
        /// </summary>
        /// <param name="filter">The filter</param>
        public CeidDiplomatikiPropertyShortcodeFilterDataModel(PropertyShortcodeFilter filter) : base(filter)
        {
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="propertyName">The name of the property that the translator used for filtering</param>
        /// <param name="value">The value of the property</param>
        public CeidDiplomatikiPropertyShortcodeFilterDataModel(string propertyName, string value) : base(propertyName, value)
        {
        }

        #endregion
    }
}
