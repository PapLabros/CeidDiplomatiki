using Atom.Core;
using Atom.Relational.Analyzers;
using Atom.Windows;
using Atom.Windows.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using static Atom.Personalization.Constants;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The data grid presenter maps page
    /// </summary>
    public class DataGridPresenterMapsPage : BaseFullyInitializableDataPresenterPage<DataGridPresenterMap>
    {
        #region Public Properties

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap { get; }

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
        /// <param name="queryMap">The query map</param>
        public DataGridPresenterMapsPage(QueryMap queryMap) : base()
        {
            QueryMap = queryMap ?? throw new ArgumentNullException(nameof(queryMap));

            CreateGUI();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the flag indicating whether a loading dialog and a completion hint dialog
        /// should be shown at the initialization
        /// </summary>
        /// <returns></returns>
        protected override bool GetShowDialogsOnInitialization() => false;

        /// <summary>
        /// Gets all the data that need to be presented
        /// </summary>
        /// <returns></returns>
        protected async override Task<IFailable<IEnumerable<DataGridPresenterMap>>> GetAllDataAsync()
        {
            // Return the data
            return await Task.FromResult(new Failable<IEnumerable<DataGridPresenterMap>>() { Result = QueryMap.DataGridPresenterMaps });
        }

        /// <summary>
        /// Creates and returns the <see cref="BaseItemsControlPage{TClass}.DataPresenter"/>
        /// </summary>
        /// <returns></returns>
        protected override IDataPresenter<DataGridPresenterMap> CreateDataPresenter()
        {
            var collapsibleDataGrid = new CollapsibleDataGrid<DataGridPresenterMap>() { Mapper = CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value }
                .ShowData(x => x.Name)
                .ShowData(x => x.Description)
                .ShowData(x => x.DataGrids)

                .SetColorUIElement(x => x.Color)
                .SetDataPresenterSubElement(x => x.DataGrids, model => model.DataGrids.Count().ToString("grid", "grids", "No grids"), (model) =>
                {
                    var dataGrid = new CollapsibleDataGrid<DataGridMap>() { Translator = CeidDiplomatikiDataModelHelpers.DataGridMapTranslator.Value, Mapper = CeidDiplomatikiDataModelHelpers.DataGridMapMapper.Value }
                        .ShowData(x => x.Type)
                        .ShowData(x => x.AllowAdd)
                        .ShowData(x => x.AllowEdit)
                        .ShowData(x => x.AllowDelete)
                        .ShowData(x => x.Columns)
                        .ShowData(x => x.DateColumn)
                        .ShowData(x => x.SearchColumns)

                        .SetBooleanUIElement(x => x.AllowAdd)
                        .SetBooleanUIElement(x => x.AllowEdit)
                        .SetBooleanUIElement(x => x.AllowDelete)

                        .SetCustomUIElement(x => x.DateColumn,
                                            (grid, row, model) =>
                                            {
                                                return new TextButton()
                                                {
                                                    HorizontalAlignment = HorizontalAlignment.Center,
                                                    VerticalAlignment = VerticalAlignment.Center,
                                                    Text = model.DateColumn?.Name,
                                                    BorderThickness = new Thickness(VisibleBorderThickness),
                                                    Command = new RelayCommand(async () =>
                                                    {
                                                        // Show the dialog
                                                        var dialogResult = await DialogHelpers.ShowUniformGridSelectSingleDialogAsync(this, "Date column selection", null, model.Type.GetProperties().Where(x => x.PropertyType.IsDate()), (prop) => new InformationalButton() { Text = prop.Name }, null, IconPaths.CalendarCheckPath);

                                                        // If we didn't get positive feedback...
                                                        if (!dialogResult.Feedback)
                                                            // Return
                                                            return;

                                                        // Update the model
                                                        model.DateColumn = dialogResult.Model;

                                                        var result = await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync();

                                                        // If there was an error...
                                                        if (!result.Successful)
                                                        {
                                                            // Show the error
                                                            await result.ShowDialogAsync(this);

                                                            // Return
                                                            return;
                                                        }

                                                        // Update the grid
                                                        grid.Update(model);
                                                    })
                                                };
                                            },
                                            (grid, row, model, element) => element.Text = model.DateColumn?.Name)

                        .SetDataPresenterSubElement(propertySelector: x => x.Columns,
                                                    textValueExtractor: model => model.Columns?.Count().ToString("column", "columns", "No columns"),
                                                    presenterImplementationFactory: model =>
                                                    {
                                                        return new DataGrid<PropertyInfo>()
                                                            .ShowData(x => x.Name, RelationalAnalyzersHelpers.DbProviderColumnMapper.Value.GetTitle(x => x.ColumnName));
                                                    },
                                                    optionButtonConfiguration: (p, m, b) =>
                                                    {
                                                        b.VectorSource = IconPaths.EditPath;

                                                        b.Command = new RelayCommand(async () =>
                                                        {
                                                            // Create the form
                                                            var form = new OptionsSelectionForm<PropertyInfo>();

                                                            // For every column
                                                            foreach (var column in m.Type.GetProperties())
                                                                // Show it for selection
                                                                form.ShowOption(column, column.Name, null, null, m.Columns.Any(x => x.Equals(column)));

                                                            // Show the dialog
                                                            var dialogResult = await DialogHelpers.ShowSelectMultipleDialogAsync(this, "Columns selection", null, form);

                                                            // If we didn't get positive feedback...
                                                            if (!dialogResult.Feedback)
                                                                // Return
                                                                return;

                                                            // Update the columns
                                                            m.Columns = form.GetOptions();

                                                            // Get the manager
                                                            var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                                                            // Save the changes
                                                            var result = await manager.SaveChangesAsync();

                                                            // If there was an error...
                                                            if (!result.Successful)
                                                            {
                                                                // Show the error
                                                                await result.ShowDialogAsync(this);

                                                                // Return
                                                                return;
                                                            }

                                                            p.SetItemsSource(m.Columns);

                                                            DataPresenter.Update(model);
                                                        });
                                                    })
                        .SetDataPresenterSubElement(propertySelector: x => x.SearchColumns,
                                                    textValueExtractor: model => model.SearchColumns?.Count().ToString("column", "columns", "No columns"),
                                                    presenterImplementationFactory: model =>
                                                     {
                                                         return new DataGrid<PropertyInfo>()
                                                             .ShowData(x => x.Name, RelationalAnalyzersHelpers.DbProviderColumnMapper.Value.GetTitle(x => x.ColumnName));
                                                     },
                                                    optionButtonConfiguration: (p, m, b) =>
                                                     {
                                                         b.VectorSource = IconPaths.EditPath;

                                                         b.Command = new RelayCommand(async () =>
                                                         {
                                                             // Create the form
                                                             var form = new OptionsSelectionForm<PropertyInfo>();

                                                             // For every column
                                                             foreach (var column in m.Type.GetProperties().Where(x => x.PropertyType == typeof(string)))
                                                                 // Show it for selection
                                                                 form.ShowOption(column, column.Name, null, null, m.SearchColumns.Any(x => x.Equals(column)));

                                                             // Show the dialog
                                                             var dialogResult = await DialogHelpers.ShowSelectMultipleDialogAsync(this, "Search columns selection", null, form);

                                                             // If we didn't get positive feedback...
                                                             if (!dialogResult.Feedback)
                                                                 // Return
                                                                 return;

                                                             // Update the columns
                                                             m.SearchColumns = form.GetOptions();

                                                             // Get the manager
                                                             var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                                                             // Save the changes
                                                             var result = await manager.SaveChangesAsync();

                                                             // If there was an error...
                                                             if (!result.Successful)
                                                             {
                                                                 // Show the error
                                                                 await result.ShowDialogAsync(this);

                                                                 // Return
                                                                 return;
                                                             }

                                                             p.SetItemsSource(m.SearchColumns);

                                                             DataPresenter.Update(model);
                                                         });
                                                     });
                    dataGrid.ConfigureOptions((container, grid, row, model) =>
                    {
                        container.AddEditOption("Data grid modification", null, () =>
                          {
                              return new DataForm<DataGridMap>() { Mapper = CeidDiplomatikiDataModelHelpers.DataGridMapMapper.Value }
                                  .ShowInput(x => x.AllowAdd)
                                  .ShowInput(x => x.AllowEdit)
                                  .ShowInput(x => x.AllowDelete);
                          }, async model => await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync());
                    });

                    return dataGrid;
                });

            collapsibleDataGrid.ConfigureOptions((container, grid, row, model) =>
            {
                container.AddEditOption("Data grid presenter modification", null, () =>
                  {
                      return new DataForm<DataGridPresenterMap>() { Mapper = CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value }
                          .ShowInput(x => x.Name, settings => settings.IsRequired = true)
                          .ShowInput(x => x.Description)
                          .ShowStringColorInput(x => x.Color);
                  }, async model => await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync());
                container.AddDeleteOption("Data grid presenter deletion", null, async model =>
                {
                    // Get the manager
                    var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                    // Unregister the data presenter
                    QueryMap.Remove(model);

                    // Save the changes
                    return await manager.SaveChangesAsync();
                });
            });

            return collapsibleDataGrid;
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
                // Create the form
                var form = new DataForm<DataGridPresenterMap>(new DataGridPresenterMap(QueryMap)) { Mapper = CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value }
                    .ShowInput(x => x.Name, settings => { settings.Name = CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Name); settings.IsRequired = true; })
                    .ShowInput(x => x.Description, settings => settings.Name = CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Description))
                    .ShowStringColorInput(x => x.Color, settings => settings.Name = CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Color));

                // Show an add dialog
                var dialogResult = await DialogHelpers.ShowConventionalAddDialogAsync(this, "Data grid creation", null, form);

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                    // Return
                    return;

                // Get the manager
                var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                // Register it
                QueryMap.Add(form.Model);

                // Save the changes
                var result = await manager.SaveChangesAsync();

                // If there was an error...
                if (!result.Successful)
                {
                    // Show the error
                    await result.ShowDialogAsync(this);

                    // Return
                    return;
                }

                // Add the model
                DataPresenter.Add(form.Model);
            });

            // Add it to the content grid
            ContentGrid.Children.Add(AddButton);
        }

        #endregion
    }
}
