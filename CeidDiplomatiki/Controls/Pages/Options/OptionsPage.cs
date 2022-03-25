using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;
using Atom.Windows.PlugIns;

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

            // Get all the main plug ins from the services
            var plugIns = DI.GetServices<IConfigurablePlugIn>();

            // For every plug in...
            foreach (var plugIn in plugIns)
            {
                // Add a button that navigates to its options
                Add(new VerticalMenuButton(VerticalMenu, async (presenter, button) => await plugIn.CreateConfigurationUIElementAsync())
                {
                    Text = plugIn.ConfigureText,
                    PathData = plugIn.ConfigurePathData,
                    BackColor = plugIn.OptionsBackColor.ToColor(),
                    ForeColor = plugIn.OptionsForeColor.ToColor()
                });
            }
        }

        #endregion
    }
}
