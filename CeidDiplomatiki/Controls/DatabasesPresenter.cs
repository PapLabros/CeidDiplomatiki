using Atom.Core;
using Atom.Windows.Controls;

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The databases presenter
    /// </summary>
    public class DatabasesPresenter : BaseItemsControl<BaseDatabaseOptionsDataModel>
    {
        #region Protected Properties

        /// <summary>
        /// The stack panel that contains all the content
        /// </summary>
        protected StackPanel ContentStackPanel { get; private set; }

        /// <summary>
        /// The stack panel collapsible vertical menu that contains the <see cref="SQLiteOptionsDataGrid"/>
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> SQLiteStackPanelCollapsibleVerticalMenu { get; private set; }

        /// <summary>
        /// The data grid that presents the SQLite options
        /// </summary>
        protected DataGrid<SQLiteOptionsDataModel> SQLiteOptionsDataGrid { get; private set; }

        /// <summary>
        /// The stack panel collapsible vertical menu that contains the <see cref="MySQLOptionsDataGrid"/>
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> MySQLStackPanelCollapsibleVerticalMenu { get; private set; }

        /// <summary>
        /// The data grid that presents the MySQL options
        /// </summary>
        protected DataGrid<MySQLOptionsDataModel> MySQLOptionsDataGrid { get; private set; }

        /// <summary>
        /// The stack panel collapsible vertical menu that contains the <see cref="SQLServerOptionsDataGrid"/>
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> SQLServerStackPanelCollapsibleVerticalMenu { get; private set; }

        /// <summary>
        /// The data grid that presents the SQLServer options
        /// </summary>
        protected DataGrid<SQLServerOptionsDataModel> SQLServerOptionsDataGrid { get; private set; }

        /// <summary>
        /// The stack panel collapsible vertical menu that contains the <see cref="PostgreSQLOptionsDataGrid"/>
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> PostgreSQLStackPanelCollapsibleVerticalMenu { get; private set; }

        /// <summary>
        /// The data grid that presents the PostgreSQL options
        /// </summary>
        protected DataGrid<PostgreSQLOptionsDataModel> PostgreSQLOptionsDataGrid { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabasesPresenter() : base()
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the GUI in a form of a <see cref="FrameworkElement"/>
        /// </summary>
        /// <returns></returns>
        protected override FrameworkElement CreateBaseGUIElement()
        {
            // Create the content stack panel
            ContentStackPanel = new StackPanel() 
            {
                Orientation = Orientation.Vertical
            };

            #region SQLite

            // Create the SQLite stack panel collapsible vertical menu
            SQLiteStackPanelCollapsibleVerticalMenu = new StackPanelCollapsibleVerticalMenu<UIElement>()
            {
                IsOpen = true,
                Text = "SQLite"
            };

            // Create the SQLite options data grid
            SQLiteOptionsDataGrid = CeidDiplomatikiDataModelHelpers.CreateDefaultSQLiteOptionsDataModelDataGrid();

            SQLiteOptionsDataGrid.SetDeleteOption(async (button, grid, row, model) => await RemoveOptionAsync(model));

            SQLiteOptionsDataGrid.Margin = new Thickness(NormalUniformMargin);

            // Add it to the collapsible menu
            SQLiteStackPanelCollapsibleVerticalMenu.Add(SQLiteOptionsDataGrid);

            // Add the collapsible menu to the stack panel
            ContentStackPanel.Children.Add(SQLiteStackPanelCollapsibleVerticalMenu);

            #endregion

            #region MySQL

            // Create the MySQL stack panel collapsible vertical menu
            MySQLStackPanelCollapsibleVerticalMenu = new StackPanelCollapsibleVerticalMenu<UIElement>()
            {
                IsOpen = true,
                Text = "MySQL"
            };

            // Create the MySQL options data grid
            MySQLOptionsDataGrid = CeidDiplomatikiDataModelHelpers.CreateDefaultMySQLOptionsDataModelDataGrid();

            MySQLOptionsDataGrid.SetOpenOption(async (button, grid, row, model) =>
                                {
                                    var provider = SQLDatabaseProvider.MySQL;

                                    // Get the connection string
                                    model.TryGetConnectionString(out var connectionString);

                                    // Get the analyzer
                                    var analyzer = CeidDiplomatikiDI.GetDatabaseAnalyzer(provider, connectionString);

                                    // Get the database
                                    var database = analyzer.GetDatabases().Result.First(x => x.DatabaseName == model.DatabaseName);

                                    // Show the page
                                    await WindowsControlsDI.GetWindowsDialogManager.OpenAsync(model.DatabaseName, IconPaths.DatabasePath, () =>
                                    {
                                        return new QueryMapsPage(database, model);
                                    }, connectionString);
                                })
                                .SetDeleteOption(async (button, grid, row, model) => await RemoveOptionAsync(model));

            MySQLOptionsDataGrid.Margin = new Thickness(NormalUniformMargin);

            // Add it to the collapsible menu
            MySQLStackPanelCollapsibleVerticalMenu.Add(MySQLOptionsDataGrid);

            // Add the collapsible menu to the stack panel
            ContentStackPanel.Children.Add(MySQLStackPanelCollapsibleVerticalMenu);

            #endregion

            #region SQLServer

            // Create the SQLServer stack panel collapsible vertical menu
            SQLServerStackPanelCollapsibleVerticalMenu = new StackPanelCollapsibleVerticalMenu<UIElement>()
            {
                IsOpen = true,
                Text = "SQLServer"
            };

            // Create the SQLServer options data grid
            SQLServerOptionsDataGrid = CeidDiplomatikiDataModelHelpers.CreateDefaultSQLServerOptionsDataModelDataGrid();

            SQLServerOptionsDataGrid.SetDeleteOption(async (button, grid, row, model) => await RemoveOptionAsync(model));

            SQLServerOptionsDataGrid.Margin = new Thickness(NormalUniformMargin);

            // Add it to the collapsible menu
            SQLServerStackPanelCollapsibleVerticalMenu.Add(SQLServerOptionsDataGrid);

            // Add the collapsible menu to the stack panel
            ContentStackPanel.Children.Add(SQLServerStackPanelCollapsibleVerticalMenu);

            #endregion

            #region PostgreSQL

            // Create the PostgreSQL stack panel collapsible vertical menu
            PostgreSQLStackPanelCollapsibleVerticalMenu = new StackPanelCollapsibleVerticalMenu<UIElement>()
            {
                IsOpen = true,
                Text = "PostgreSQL"
            };

            // Create the PostgreSQL options data grid
            PostgreSQLOptionsDataGrid = CeidDiplomatikiDataModelHelpers.CreateDefaultPostgreSQLOptionsDataModelDataGrid();

            PostgreSQLOptionsDataGrid.SetDeleteOption(async (button, grid, row, model) => await RemoveOptionAsync(model));

            PostgreSQLOptionsDataGrid.Margin = new Thickness(NormalUniformMargin);

            // Add it to the collapsible menu
            PostgreSQLStackPanelCollapsibleVerticalMenu.Add(PostgreSQLOptionsDataGrid);

            // Add the collapsible menu to the stack panel
            ContentStackPanel.Children.Add(PostgreSQLStackPanelCollapsibleVerticalMenu);

            #endregion

            // Return the content stack panel
            return ContentStackPanel;
        }

        /// <summary>
        /// Handles the addition of an item
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnItemAdded(ItemAddedEventArgs<BaseDatabaseOptionsDataModel> e)
        {
            base.OnItemAdded(e);

            HandleAddition(e.Item);
        }

        /// <summary>
        /// Handles the removal of an item
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnItemRemoved(ItemRemovedEventArgs<BaseDatabaseOptionsDataModel> e)
        {
            base.OnItemRemoved(e);

            HandleRemoval(e.Item);
        }

        /// <summary>
        /// Handles the replacement of an item
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnItemReplaced(ItemReplacedEventArgs<BaseDatabaseOptionsDataModel> e)
        {
            base.OnItemReplaced(e);

            HandleAddition(e.AddedItem);
            HandleRemoval(e.RemovedItem);
        }

        /// <summary>
        /// Handles the disposal of all items
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnItemsCleared(ItemsClearedEventArgs<BaseDatabaseOptionsDataModel> e)
        {
            base.OnItemsCleared(e);

            // Clear the data grid
            SQLiteOptionsDataGrid.Clear();
            MySQLOptionsDataGrid.Clear();
            SQLServerOptionsDataGrid.Clear();
            PostgreSQLOptionsDataGrid.Clear();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Removes the specified <paramref name="option"/>
        /// </summary>
        /// <returns></returns>
        private async Task RemoveOptionAsync(BaseDatabaseOptionsDataModel option)
        {
            // Show a transitional dialog
            var dialogResult = await DialogHelpers.ShowDeletionTransitionalDialogAsync(this, $"{option.DatabaseName} removal", null, IconPaths.DatabasePath);

            // If we got negative feedback...
            if (!dialogResult.Feedback)
                // Return
                return;

            // Remove the option
            Remove(option);

            // Get the manager
            var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

            // Unregister the database
            manager.Unregister(option);

            // Save the changes
            var result = await manager.SaveChangesAsync();

            // If there was an error...
            if (!result.Successful)
                // Show the error
                await result.ShowDialogAsync(this);
        }

        /// <summary>
        /// Handles the addition of the specified <paramref name="options"/>
        /// </summary>
        /// <param name="options">The options</param>
        private void HandleAddition(BaseDatabaseOptionsDataModel options)
        {
            if (options is SQLiteOptionsDataModel sqliteOptions)
                SQLiteOptionsDataGrid.Add(sqliteOptions);
            else if (options is MySQLOptionsDataModel mysqlOptions)
                MySQLOptionsDataGrid.Add(mysqlOptions);
            else if (options is SQLServerOptionsDataModel sqlServerOptions)
                SQLServerOptionsDataGrid.Add(sqlServerOptions);
            else if (options is PostgreSQLOptionsDataModel postgreSQLOptions)
                PostgreSQLOptionsDataGrid.Add(postgreSQLOptions);
        }

        /// <summary>
        /// Handles the removal of the specified <paramref name="options"/>
        /// </summary>
        /// <param name="options">The options</param>
        private void HandleRemoval(BaseDatabaseOptionsDataModel options)
        {
            if (options is SQLiteOptionsDataModel sqliteOptions)
                SQLiteOptionsDataGrid.Remove(sqliteOptions);
            else if (options is MySQLOptionsDataModel mysqlOptions)
                MySQLOptionsDataGrid.Remove(mysqlOptions);
            else if (options is SQLServerOptionsDataModel sqlServerOptions)
                SQLServerOptionsDataGrid.Remove(sqlServerOptions);
            else if (options is PostgreSQLOptionsDataModel postgreSQLOptions)
                PostgreSQLOptionsDataGrid.Remove(postgreSQLOptions);
        }

        #endregion
    }
}
