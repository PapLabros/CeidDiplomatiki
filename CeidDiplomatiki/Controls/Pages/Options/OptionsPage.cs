using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;

using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The options page
    /// </summary>
    public class OptionsPage : VerticalMenuPage
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public OptionsPage() : base()
        {
            CreateGUI();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and adds the required GUI elements
        /// </summary>
        private void CreateGUI()
        {
            // Add the databases page
            Add("Databases", IconPaths.DatabasePath, async (menu, button) =>
            {
                return await Task.FromResult(new DatabasesPage());
            });

            // Add the page maps page
            Add("Pages", null, async (menu, button) =>
            {
                return await Task.FromResult(new PageMapsPage());
            });
        }

        #endregion
    }
}
