using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;
using Atom.Windows.Controls.TabControl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The main application page
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public class CeidDiplomatikiMainApplicationPage : TabControlApplicationPage, ICeidDiplomatikiMainPageBuilder
#pragma warning restore IDE1006 // Naming Styles
    {
        #region Private Members

        /// <summary>
        /// The containers that contain the buttons that navigate to the dynamic pages
        /// </summary>
        private readonly List<StackPanelCollapsibleVerticalMenu> mDynamicPageButtonContainers = new List<StackPanelCollapsibleVerticalMenu>();

        #endregion

        #region Protected Properties

        /// <summary>
        /// The menu that contains all the application related options
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu ApplicationMenuOptionsContainer { get; private set; }

        /// <summary>
        /// The button that navigates to the options
        /// </summary>
        protected MenuButton OptionsMenuButton { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CeidDiplomatikiMainApplicationPage() : base()
        {
            // Set the service
            CeidDiplomatikiDI.GetCeidDiplomatikiMainPageBuilder = this;

            CreateGUI();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Recreates the dynamically created menu buttons
        /// </summary>
        public void Refresh()
        {
            // For every button container...
            foreach(var container in mDynamicPageButtonContainers)
                // Remove it
                LeftMenuItemsContainer.Children.Remove(container);

            // Get the manager
            var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

            // For every root page map grouped by category...
            foreach (var rootPageMapGroup in manager.RootPages.OrderBy(x => x.Order).GroupBy(x => x.Category))
            {
                // Create the presenter menu options container
                var presenterMenuOptionsContainer = new StackPanelCollapsibleVerticalMenu()
                {
                    IsOpen = true,
                    Text = rootPageMapGroup.Key
                };

                // For every map...
                foreach (var rootPageMap in rootPageMapGroup)
                {
                    // Create the button
                    var button = new MenuButton()
                    {
                        Text = rootPageMap.Name,
                        VectorSource = rootPageMap.PathData,
                        BackColor = rootPageMap.Color.ToColor(),
                        ForeColor = rootPageMap.Color.ToColor().DarkOrWhite(),
                        IsEnabled = !(rootPageMap.Presenter == null && rootPageMap.Pages.Count == 0)
                    };

                    button.Command = new RelayCommand(() =>
                    {
                        WindowsControlsDI.GetWindowsDialogManager.OpenAsync(rootPageMap.Name, rootPageMap.PathData, () =>
                        {
                            // If there are other paged...
                            if (rootPageMap.Pages.Count() > 0)
                                // Create a sub pages page
                                return new PageMapPage(rootPageMap);
                            // Else...
                            else
                            {
                                // Get the presenter
                                var presenter = rootPageMap.Presenter;

                                // Create and return a page
                                return PresenterPagesFactory.CreatePresenterPage(presenter, rootPageMap);
                            }
                        }, rootPageMap.Id);
                    });

                    // Add it to the menu
                    presenterMenuOptionsContainer.Add(button);
                }

                // Add it to the containers
                mDynamicPageButtonContainers.Add(presenterMenuOptionsContainer);

                // Add it to the left side menu
                AddLeftMenuElement(presenterMenuOptionsContainer);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the initialization of the page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Refresh
            Refresh();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and adds the required GUI elements
        /// </summary>
        private void CreateGUI()
        {
            Add("Overview", new Grid(), false);

            Host.Get(0).SelectTab(0);

            // Create the application menu options container
            ApplicationMenuOptionsContainer = new StackPanelCollapsibleVerticalMenu() 
            {
                Text = "Application",
                IsOpen = true
            };

            // Add the options button
            OptionsMenuButton = ApplicationMenuOptionsContainer.Add("Options", IconPaths.SettingsPath);
            
            OptionsMenuButton.Command = new RelayCommand(() =>
            {
                WindowsControlsDI.GetWindowsDialogManager.OpenAsync("Options", IconPaths.SettingsPath, () => new OptionsPage(), "options");
            });

            // Add it to the left side menu
            AddLeftMenuElement(ApplicationMenuOptionsContainer);
        }

        #endregion
    }
}
