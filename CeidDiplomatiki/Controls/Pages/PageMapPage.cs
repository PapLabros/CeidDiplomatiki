using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// A page that contains accessors to all of the sub pages related to the <see cref="PageMap"/>
    /// </summary>
    public class PageMapPage : VerticalMenuPage
    {
        #region Public Properties

        /// <summary>
        /// The page map
        /// </summary>
        public PageMap PageMap { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageMapPage(PageMap pageMap) : base()
        {
            PageMap = pageMap ?? throw new ArgumentNullException(nameof(pageMap));

            CreateGUI();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and adds the required GUI
        /// </summary>
        private void CreateGUI()
        {
            // Lock the menu
            VerticalMenu.MenuLocked = true;

            // For every sub page map...
            foreach(var pageMap in PageMap.Pages.OrderBy(x => x.Order))
            {
                // Create the button
                var button = new VerticalMenuButton(VerticalMenu, async (menu, button) => 
                {
                    // If there are other pages...
                    if (pageMap.Pages.Count() > 0)
                        // Create a sub pages page
                        return await Task.FromResult(new PageMapPage(pageMap));
                    // Else...
                    else
                    {
                        // Get the presenter
                        var presenter = pageMap.Presenter;

                        // Create and return a page
                        return await Task.FromResult(PresenterPagesFactory.CreatePresenterPage(presenter, pageMap));
                    }
                })
                {
                    Text = pageMap.Name,
                    PathData = pageMap.PathData,
                    BackColor = pageMap.Color.ToColor(),
                    ForeColor = pageMap.Color.ToColor().DarkOrWhite(),
                    IsEnabled = !(pageMap.Presenter == null && pageMap.Pages.Count == 0)
                };

                // Create the context menu
                var contextMenu = new StandardContextMenu();

                contextMenu.Add("Open in new tab", IconPaths.TabPlusPath, new RelayCommand(() =>
                {
                    WindowsControlsDI.GetWindowsDialogManager.OpenAsync(pageMap.Name, pageMap.PathData, () =>
                    {
                        // If there are other pages...
                        if (pageMap.Pages.Count() > 0)
                            // Create a sub pages page
                            return new PageMapPage(pageMap);
                        // Else...
                        else
                        {
                            // Get the presenter
                            var presenter = pageMap.Presenter;

                            // Create and return a page
                            return PresenterPagesFactory.CreatePresenterPage(presenter, pageMap);
                        }
                    }, pageMap.Id);
                }));

                // Set it to the button
                button.ContextMenu = contextMenu;

                // Add it to the menu
                Add(button, pageMap.Category);
            }
        }

        #endregion
    }
}
