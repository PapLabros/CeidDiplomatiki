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
        #region Constants

        public const string DatabaseExportPath = "M17.86 18L18.9 19C17.5 20.2 14.94 21 12 21C7.59 21 4 19.21 4 17V7C4 4.79 7.58 3 12 3C14.95 3 17.5 3.8 18.9 5L17.86 6L17.5 6.4C16.65 5.77 14.78 5 12 5C8.13 5 6 6.5 6 7S8.13 9 12 9C13.37 9 14.5 8.81 15.42 8.54L16.38 9.5H13.5V10.92C13 10.97 12.5 11 12 11C9.61 11 7.47 10.47 6 9.64V12.45C7.3 13.4 9.58 14 12 14C12.5 14 13 13.97 13.5 13.92V14.5H16.38L15.38 15.5L15.5 15.61C14.41 15.86 13.24 16 12 16C9.72 16 7.61 15.55 6 14.77V17C6 17.5 8.13 19 12 19C14.78 19 16.65 18.23 17.5 17.61L17.86 18M18.92 7.08L17.5 8.5L20 11H15V13H20L17.5 15.5L18.92 16.92L23.84 12L18.92 7.08Z";

        #endregion

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

            MySQLOptionsDataGrid.SetOption(
                "databaseConvert",
                async (button, grid, row, model) =>
                {
                    // Disable the button
                    button.IsEnabled = false;

                    // Create the options
                    var options = new DatabaseProviderOptionsDataModel();

                    // Create options container
                    var component = new DatabaseOptionComponentsContainer() { Width = 600 };

                    component.Add(new SQLiteOptionsComponent(options.SQLite));
                    component.Add(new MySQLOptionsComponent(options.MySQL));
                    component.Add(new SQLServerOptionsComponent(options.SQLServer));
                    component.Add(new PostgreSQLOptionsComponent(options.PostgreSQL));

                    // Show the dialog
                    var dialogResult = await DialogHelpers.ShowValidationDialogAsync(this, "Βάση δεδομένων", null, component, element => element.Validate(), DatabaseExportPath);

                    // If we didn't get positive feedback...
                    if (!dialogResult.Feedback)
                    {
                        // Re-enable the button
                        button.IsEnabled = true;

                        // Return
                        return;
                    }

                    // Update the options
                    component.UpdateOptionData();

                    // Set the selected provider option
                    options.Provider = component.SelectedOptions.Provider;

                    model.TryGetConnectionString(out var connectionString);
                    // Convert the database
                    var result = await DialogHelpers.ShowLoadingHintDialogAsync(this, () => Task.Run(() => DatabaseConverter.Instance.ConvertAsync(new DatabaseInfo(SQLDatabaseProvider.MySQL, model.DatabaseName, connectionString, string.Empty), new DatabaseInfo(options.Provider, options.GetDatabaseName(), options.GetConnectionString(), options.Options.TablesPrefix))));

                    // If there was an error...
                    if (!result.Successful)
                    {
                        // Re-enable the button
                        button.IsEnabled = true;

                        // Show the error
                        await result.ShowDialogAsync(this);

                        // Return
                        return;
                    }

                    // Re-enable the button
                    button.IsEnabled = true;

                    // Show a changes saved dialog
                    await DialogHelpers.ShowChangesSavedHintDialogAsync(this);
                },
                DatabaseExportPath,
                "Μετατροπή βάσης",
                Gray);

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
