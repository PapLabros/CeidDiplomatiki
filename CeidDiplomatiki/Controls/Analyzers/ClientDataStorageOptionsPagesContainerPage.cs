
using Atom.Windows.Controls;

using System.Threading.Tasks;
using System.Windows;

namespace CeidDiplomatiki
{
    /// <summary>
    /// A page that contains multiple <see cref="ClientDataStorageOptionsPage"/>s
    /// </summary>
    public class ClientDataStorageOptionsPagesContainerPage : BaseControl
    {
        #region Protected Properties

        /// <summary>
        /// The presenter that contain the multiple <see cref="ClientDataStorageOptionsPage"/>s
        /// </summary>
        protected VerticalMenu Presenter { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientDataStorageOptionsPagesContainerPage()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and adds a <see cref="ClientDataStorageOptionsPage"/> with the specified <paramref name="fileName"/>
        /// </summary>
        /// <param name="vectorSource">The vector source</param>
        /// <param name="text">The text</param>
        /// <param name="directoryPath">The directory path of the default directory where the SQLite database is stored</param>
        /// <param name="fileName">The directory path as well as the xml name of the file</param>
        /// <param name="databaseName">The default database name used when generating the options file</param>
        public void Add(string text, string vectorSource, string fileName, string directoryPath, string databaseName)
        {
            // Add it to the menu
            Presenter.Add(text, vectorSource, (presenter, presenterButton) =>
            {
                return Task.FromResult<FrameworkElement>(new ClientDataStorageOptionsPage(fileName, directoryPath, databaseName));
            });
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the GUI in a form of a <see cref="FrameworkElement"/>
        /// </summary>
        /// <returns></returns>
        protected override FrameworkElement CreateBaseGUIElement()
        {
            // Create the presenter
            Presenter = new VerticalMenu()
            {
                MenuLocked = true
            };

            // Return it
            return Presenter;
        }

        #endregion
    }
}
