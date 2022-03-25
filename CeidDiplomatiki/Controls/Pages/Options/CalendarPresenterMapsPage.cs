using Atom.Core;
using Atom.Windows.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The calendar presenter maps page
    /// </summary>
    public class CalendarPresenterMapsPage : ConventionalBaseDataPresenterPage<CalendarPresenterMap>
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
        public CalendarPresenterMapsPage(QueryMap queryMap) : base()
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
        /// Creates and returns the <see cref="BaseItemsControlPage{TClass}.DataPresenter"/>
        /// </summary>
        /// <returns></returns>
        protected override IDataPresenter<CalendarPresenterMap> CreateDataPresenter()
        {
            return new CollapsibleDataGrid<CalendarPresenterMap>() { Mapper = CeidDiplomatikiDataModelHelpers.CalendarPresenterMapMapper.Value, Translator = CeidDiplomatikiDataModelHelpers.CalendarPresenterMapTranslator.Value }
                .ShowData(x => x.Name)
                .ShowData(x => x.Description)
                .ShowData(x => x.TitleFormula)
                .ShowData(x => x.DescriptionFormula)
                .ShowData(x => x.DateStartColumn)
                .ShowData(x => x.DateEndColumn)
                .ShowData(x => x.SearchColumns)
                .ShowData(x => x.AllowAdd)
                .ShowData(x => x.AllowEdit)
                .ShowData(x => x.AllowDelete)

                .SetBooleanUIElement(x => x.AllowAdd)
                .SetBooleanUIElement(x => x.AllowEdit)
                .SetBooleanUIElement(x => x.AllowDelete)

                .SetDataPresenterSubElement(propertySelector: x => x.SearchColumns,
                                            textValueExtractor: model => model.SearchColumns?.Count().ToString("column", "columns", "No columns"),
                                            presenterImplementationFactory: model =>
                                            {
                                                return new DataGrid<PropertyInfo>()
                                                    .ShowData(x => x.Name, "Column name");
                                            },
                                            optionButtonConfiguration: (presenter, model, button) =>
                                            {
                                                button.PathData = IconPaths.EditPath;
                                            
                                                button.Command = new RelayCommand(async () =>
                                                {
                                                    // Create the form
                                                    var form = new OptionsSelectionForm<PropertyInfo>();
                                            
                                                    // For every column
                                                    foreach (var column in model.QueryMap.RootType.GetProperties().Where(x => x.PropertyType == typeof(string)))
                                                        // Show it for selection
                                                        form.ShowOption(column, column.Name, null, null, model.SearchColumns.Any(x => x.Equals(column)));
                                            
                                                    // Show the dialog
                                                    var dialogResult = await DialogHelpers.ShowOptionsSelectionDialogAsync(this, "Search columns selection", null, form);
                                            
                                                    // If we didn't get positive feedback...
                                                    if (!dialogResult.Feedback)
                                                        // Return
                                                        return;
                                            
                                                    // Update the columns
                                                    model.SearchColumns = form.GetOptions();
                                            
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
                                            
                                                    presenter.SetItemsSource(model.SearchColumns);
                                            
                                                    DataPresenter.Update(model);
                                                });
                                            })

                .SetEditOption(() => 
                {
                    return new DataForm<CalendarPresenterMap>(new CalendarPresenterMap(QueryMap)) { Mapper = CeidDiplomatikiDataModelHelpers.CalendarPresenterMapMapper.Value }
                    .ShowInput(x => x.Name, CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Name), true)
                    .ShowInput(x => x.Description, CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Description))
                    .ShowStringColorFormInput(x => x.Color, CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Color))
                    .ShowShortcodesTextInput(x => x.TitleFormula, QueryMap.PropertyShortcodes.Value, null, true)
                    .ShowShortcodesTextInput(x => x.DescriptionFormula, QueryMap.PropertyShortcodes.Value)
                    .ShowSelectSingleOptionInput(x => x.DateStartColumn, (form, propertyInfo) => new DropDownMenuOptionsFormInput<PropertyInfo>(form, propertyInfo, QueryMap.RootType.GetProperties().Where(x => x.PropertyType.IsDate()), x => x.Name), null, true)
                    .ShowSelectSingleOptionInput(x => x.DateEndColumn, (form, propertyInfo) => new DropDownMenuOptionsFormInput<PropertyInfo>(form, propertyInfo, QueryMap.RootType.GetProperties().Where(x => x.PropertyType.IsDate()), x => x.Name))
                    .ShowInput(x => x.AllowAdd)
                    .ShowInput(x => x.AllowEdit)
                    .ShowInput(x => x.AllowDelete);
                }, "Calendar presenter modification", null, async model => await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync())
                .SetDeleteOption("Calendar presenter deletion", null, async model => 
                {
                    // Get the manager
                    var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                    // Unregister the data presenter
                    QueryMap.Remove(model);

                    // Save the changes
                    return await manager.SaveChangesAsync();
                });
        }

        /// <summary>
        /// Gets all the data that need to be presented
        /// </summary>
        /// <returns></returns>
        protected override async Task<IFailable<IEnumerable<CalendarPresenterMap>>> GetAllDataAsync() 
            => await Task.FromResult(new Failable<IEnumerable<CalendarPresenterMap>>() { Result = QueryMap.CalendarPresenterMaps });

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
                // Create the map
                var map = new CalendarPresenterMap(QueryMap);

                // Create the form
                var form = new DataForm<CalendarPresenterMap>(map) { Mapper = CeidDiplomatikiDataModelHelpers.CalendarPresenterMapMapper.Value }
                    .ShowInput(x => x.Name, CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Name), true)
                    .ShowInput(x => x.Description, CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Description))
                    .ShowStringColorFormInput(x => x.Color, CeidDiplomatikiDataModelHelpers.DataGridPresenterMapMapper.Value.GetTitle(x => x.Color))
                    .ShowShortcodesTextInput(x => x.TitleFormula, QueryMap.PropertyShortcodes.Value, null, true)
                    .ShowShortcodesTextInput(x => x.DescriptionFormula, QueryMap.PropertyShortcodes.Value)
                    .ShowSelectSingleOptionInput(x => x.DateStartColumn, (form, propertyInfo) => new DropDownMenuOptionsFormInput<PropertyInfo>(form, propertyInfo, QueryMap.RootType.GetProperties().Where(x => x.PropertyType.IsDate()), x=> x.Name), null, true)
                    .ShowSelectSingleOptionInput(x => x.DateEndColumn, (form, propertyInfo) => new DropDownMenuOptionsFormInput<PropertyInfo>(form, propertyInfo, QueryMap.RootType.GetProperties().Where(x => x.PropertyType.IsDate()), x => x.Name))
                    .ShowInput(x => x.AllowAdd)
                    .ShowInput(x => x.AllowEdit)
                    .ShowInput(x => x.AllowDelete);

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
