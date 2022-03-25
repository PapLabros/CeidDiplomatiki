using Atom.Core;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Represents a property shortcode value translator
    /// </summary>
    public class CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel : PropertyShortcodeValueTranslatorDataModel<CeidDiplomatikiPropertyShortcodeDataModel, CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel, CeidDiplomatikiPropertyShortcodeFilterDataModel>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel() : base()
        {

        }

        /// <summary>
        /// Translator based constructor
        /// </summary>
        /// <param name="translator">The translator</param>
        public CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel(PropertyShortcodeValueTranslator translator) : base(translator)
        {
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="valueConversion"></param>
        /// <param name="fromValue">The value to translate from</param>
        /// <param name="formula">The formula value that the <see cref="PropertyShortcodeValueTranslatorDataModel{TPropertyShortcodeDataModel, TPropertyShortcodeValueTranslatorDataModel, TPropertyShortcodePropertyValueFilterDataModel}.FromValue"/> is translated to</param>
        /// <param name="wordReplacement">A flag indicating whether word replacement is enabled or not</param>
        /// <param name="wordReplacementOperation">The word replacement operation</param>
        /// <param name="fromWord">The word to replace</param>
        /// <param name="toWord">The word that the <see cref="PropertyShortcodeValueTranslatorDataModel{TPropertyShortcodeDataModel, TPropertyShortcodeValueTranslatorDataModel, TPropertyShortcodePropertyValueFilterDataModel}.FromWord"/> should be replaced with</param>
        /// <param name="priority">The priority index</param>
        public CeidDiplomatikiPropertyShortcodeValueTranslatorDataModel(bool valueConversion, string fromValue, string formula, bool wordReplacement, WordReplacementOperation wordReplacementOperation, string fromWord, string toWord, int priority = 0) : base(valueConversion, fromValue, formula, wordReplacement, wordReplacementOperation, fromWord, toWord, priority)
        {
        }

        #endregion
    }
}
