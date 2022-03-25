using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The query maps page
    /// </summary>
    public class QueryMapsPage : ConventionalBaseDataPresenterPage<QueryMap>
    {
        #region Public Properties

        /// <summary>
        /// The database
        /// </summary>
        public IDbProviderDatabase Database { get; }

        /// <summary>
        /// The database options
        /// </summary>
        public BaseDatabaseOptionsDataModel DatabaseOptions { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The add button
        /// </summary>
        protected IconButton AddButton { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="database">The database</param>
        /// <param name="databaseOptions">The database options</param>
        public QueryMapsPage(IDbProviderDatabase database, BaseDatabaseOptionsDataModel databaseOptions) : base()
        {
            Database = database ?? throw new System.ArgumentNullException(nameof(database));
            DatabaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));

            CreateGUI();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseItemsControlPage{TClass}.DataPresenter"/>
        /// </summary>
        /// <returns></returns>
        protected override IDataPresenter<QueryMap> CreateDataPresenter()
        {
            return new CollapsibleDataGrid<QueryMap>()
               .ShowData(x => x.Name, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Name))
               .ShowData(x => x.Description, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Description))
               .ShowData(x => x.Tables, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Tables))
               .ShowData(x => x.Columns, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Columns))
               .ShowData(x => x.Joins, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Joins))

               .SetDataPresenterSubElement(x => x.Tables, model => model.Tables.Count().ToString("table", "tables", "No tables"), model => 
               {
                   return new DataGrid<IDbProviderTable>()
                    .ShowData(x => x.TableCatalog, "Table catalog")
                    .ShowData(x => x.TableSchema, "Table schema")
                    .ShowData(x => x.TableName, "Table name");
               })
               .SetDataPresenterSubElement(x => x.Columns, model => model.Columns.Count().ToString("column", "columns", "No columns"), model => 
               {
                   return new DataGrid<IDbProviderColumn>()
                    .ShowData(x => x.TableName, "Table name")
                    .ShowData(x => x.ColumnName, "Column name")
                    .ShowData(x => x.DataType, "Data type")
                    .ShowData(x => x.IsPrimaryKey, "Is primary key")
                    .ShowData(x => x.IsNullable, "Is nullable")
                    .ShowData(x => x.IsUnique, "Is unique")
                    .ShowData(x => x.IsAutoIncrement, "Is auto increment")

                    .SetLabelUIElement(x => x.DataType, model => model.DataType.ToColumnValueType().ToLocalizedString(), model => model.DataType.ToColumnValueType().ToColorHex())
                    .SetBooleanUIElement(x => x.IsPrimaryKey)
                    .SetBooleanUIElement(x => x.IsNullable)
                    .SetBooleanUIElement(x => x.IsUnique)
                    .SetBooleanUIElement(x => x.IsAutoIncrement);
               })
               .SetDataPresenterSubElement(x => x.Joins, model => model.Joins.Count().ToString("join", "joins", "No joins"), model => 
               {
                   return new DataGrid<JoinMap>() { Mapper = CeidDiplomatikiDataModelHelpers.JoinMapMapper.Value, Translator = CeidDiplomatikiDataModelHelpers.JoinMapTranslator.Value }
                    .ShowData(x => x.Table)
                    .ShowData(x => x.PrincipleKeyColumn)
                    .ShowData(x => x.ReferencedTable)
                    .ShowData(x => x.ForeignKeyColumn)
                    .ShowData(x => x.IsRightJoin)
                    
                    .SetBooleanUIElement(x => x.IsRightJoin);
               })

               .SetComponentBackColorSetter((model, index) => model.Color.ToColor())
               .SetComponentForeColorSetter((model, index) => model.Color.ToColor().DarkOrWhite())
               
               .SetOpenOption((button, grid, row, model) => 
               {
                   WindowsControlsDI.GetWindowsDialogManager.OpenAsync(model.Name, IconPaths.TablePath, () =>
                   {
                       return new PropertyMapsAndPresentersPage(model);
                   }, model.Id);
               })
               .SetEditOption(() => 
               {
                   return new DataForm<QueryMap>()
                    .ShowInput(x => x.Name, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Name), true)
                    .ShowInput(x => x.Description, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Description))
                    .ShowStringColorFormInput(x => x.Color, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Color));
               }, "Query map modification", null, async (model) => await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync(), IconPaths.TablePath)
               .SetDeleteOption("Query map deletion", null, async (model) => 
               {
                   // Get the manager
                   var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                   // Unregister the map
                   await manager.Unregister(model);

                   // Save the changes
                   return await manager.SaveChangesAsync();
               }, IconPaths.TablePath);
        }

        /// <summary>
        /// Gets all the data that need to be presented
        /// </summary>
        /// <returns></returns>
        protected override async Task<IFailable<IEnumerable<QueryMap>>> GetAllDataAsync()
        {
            // Get the manager
            var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

            // Get the tables of the database
            return await Task.FromResult(new Failable<IEnumerable<QueryMap>>() { Result = manager.QueryMaps.Where(x => x.DatabaseOptions == DatabaseOptions).ToList() });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and adds the required GUI elements
        /// </summary>
        private void CreateGUI()
        {
            // Create the add button
            AddButton = ControlsFactory.CreateStandardAddCircularButton();

            AddButton.Command = new RelayCommand(async () =>
            {
                // Disable the button
                AddButton.IsEnabled = false;

                // Get the connection string
                DatabaseOptions.TryGetConnectionString(out var connectionString);

                // Get the analyzer
                var analyzer = CeidDiplomatikiDI.GetDatabaseAnalyzer(DatabaseOptions.Provider, connectionString);

                // Get all the tables
                var tablesResult = analyzer.GetTables(Database.DatabaseName);

                var tables = tablesResult.Result;

                // Get all the foreign key columns
                var foreignKeyColumnsResult = analyzer.GetForeignKeyColumns(Database.DatabaseName, null);
                var foreignKeyColumns = foreignKeyColumnsResult.Result;

                // Create a steps presenter
                var stepsPresenter = new StepsPresenter() { AllowArbitraryNavigation = false };

                // Create a node component
                var nodeComponent = new NodeComponent<IDbProviderTable>(tables, (table) =>
                {
                    // Get the foreign keys of the table
                    var foreignKeys = foreignKeyColumns.Where(x => x.TableName == table.TableName).ToList();

                    // Get the tables whose primary key is related to one of the foreign keys
                    var foreignToPrimaryKeyTables = tables.Where(x => foreignKeys.Any(y => y.ReferencedTableName == x.TableName)).ToList();

                    // Get the foreign keys that are related to the primary key of the table
                    var relatedForeignKeys = foreignKeyColumns.Where(x => x.ReferencedTableName == table.TableName).ToList();

                    // Get the tables
                    var primaryToForeignKeyTables = tables.Where(x => relatedForeignKeys.Any(y => y.TableName == x.TableName)).ToList();

                    return foreignToPrimaryKeyTables.Concat<IDbProviderTable, IDbProviderTable>(primaryToForeignKeyTables).Distinct();
                }, table => table.TableName);

                var nodeComponentScrollViewer = new ScrollViewer()
                {
                    Content = nodeComponent,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                };

                // Add it to the steps presenter
                stepsPresenter.Add("Query", nodeComponentScrollViewer, element => nodeComponent.NodePath.Model != null);

                // Create the form
                var form = new DataForm<QueryMap>()
                    .ShowInput(x => x.Name, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Name), true)
                    .ShowInput(x => x.Description, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Description))
                    .ShowStringColorFormInput(x => x.Color, CeidDiplomatikiDataModelHelpers.QueryMapMapper.Value.GetTitle(x => x.Color));

                // Add it to the steps presenter
                stepsPresenter.Add("Info", form, (element) => element.Validate());

                // Show a dialog
                var dialogResult = await DialogHelpers.ShowStepsDialogAsync(this, "Query map creation", null, stepsPresenter, IconPaths.TablePath);

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                {
                    // Re enable the button
                    AddButton.IsEnabled = true;

                    // Return
                    return;
                }

                // Get the selected tables
                tables = nodeComponent.NodePath.DistinctModels;

                // Get the pairs
                var pairs = nodeComponent.NodePath.Pairs;

                // Get the node path
                var nodePath = nodeComponent.NodePath;
                var columns = new List<IDbProviderColumn>();
                foreach(var table in tables)
                {
                    // Get the columns of the table
                    var columnsResult = analyzer.GetColumns(DatabaseOptions.DatabaseName, table.TableName);
                    columns.AddRange(columnsResult.Result);
                }
               
                // The joins collection
                var joins = new List<JoinMap>();

                // For every pair...
                foreach (var pair in pairs)
                {
                    JoinMap joinMap;

                    if (foreignKeyColumns.Any(x => x.TableName == pair.Value.Model.TableName && x.ReferencedTableName == pair.Key.Model.TableName))
                    {
                        // Get the foreign key column
                        var foreignKeyColumn = foreignKeyColumns.First(x => x.TableName == pair.Value.Model.TableName && x.ReferencedTableName == pair.Key.Model.TableName);

                        // Get the principle column
                        var principleColumn = columns.First(x => x.ColumnName == foreignKeyColumn.ReferencedColumnName);

                        // Get the foreign key column
                        var referencedColumn = columns.First(x => x.ColumnName == foreignKeyColumn.ColumnName);

                        // Create the join map
                        joinMap = new JoinMap(pair.Key.Model, principleColumn, pair.Value.Model, referencedColumn, pair.Key.Index, false);
                    }
                    else
                    {
                        // Get the foreign key column
                        var foreignKeyColumn = foreignKeyColumns.First(x => x.ReferencedTableName == pair.Value.Model.TableName && x.TableName == pair.Key.Model.TableName);

                        // Get the principle column
                        var principleColumn = columns.First(x => x.ColumnName == foreignKeyColumn.ColumnName);

                        // Get the foreign key column
                        var referencedColumn = columns.First(x => x.ColumnName == foreignKeyColumn.ReferencedColumnName);

                        // Create the join map
                        joinMap = new JoinMap(pair.Key.Model, principleColumn, pair.Value.Model, referencedColumn, pair.Key.Index, true);
                    }

                    // Add it to the joins
                    joins.Add(joinMap);
                }

                // Create the model
                var model = QueryMap.FromDataModel(DatabaseOptions, Database, tables, columns, joins);

                // Set it to the form
                form.Model = model;

                // Update its values
                form.UpdateModelValues();

                // Get the manager
                var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                // Register the model
                await manager.RegisterAsync(model);

                // Save the changes
                var result = await manager.SaveChangesAsync();

                // If there was an error...
                if (!result.Successful)
                {
                    // Show the error
                    await result.ShowDialogAsync(this);

                    // Re enable the button
                    AddButton.IsEnabled = true;

                    // Return
                    return;
                }

                // Add it to the presenter
                DataPresenter.Add(model);

                // Re enable the button
                AddButton.IsEnabled = true;
            });

            // Add it to the content grid
            ContentGrid.Children.Add(AddButton);
        }

        #endregion
    }
}
