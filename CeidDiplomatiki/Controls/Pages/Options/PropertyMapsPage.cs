using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static Atom.Personalization.Constants;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The property maps page
    /// </summary>
    public class PropertyMapsPage : BaseFullyInitializableItemsControlPage<PropertyMap>
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
        public PropertyMapsPage(QueryMap queryMap) : base()
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
        protected async override Task<IFailable<IEnumerable<PropertyMap>>> GetAllDataAsync()
        {
            // Get the models
            return await Task.FromResult(new Failable<IEnumerable<PropertyMap>>() { Result = QueryMap.PropertyMaps });
        }

        /// <summary>
        /// Creates and returns the <see cref="BaseItemsControlPage{TClass}.DataPresenter"/>
        /// </summary>
        /// <returns></returns>
        protected override IItemsControl<PropertyMap> CreateItemsControl()
        {
            return new CollapsibleDataGrid<PropertyMap>() { Mapper = CeidDiplomatikiDataModelHelpers.PropertyMapMapper.Value }
                .ShowData(x => x.TableName)
                .ShowData(x => x.ColumnName)
                .ShowData(x => x.ValueType)
                .ShowData(x => x.Name)
                .ShowData(x => x.Description)
                .ShowData(x => x.Attributes)
                .ShowData(x => x.Color)
                .ShowData(x => x.DefaultValue)
                .ShowData(x => x.Order)
                .ShowData(x => x.IsEditable)
                .ShowData(x => x.IsRequired)
                .ShowData(x => x.IsPreview)

                .SetColorUIElement(x => x.Color)
                .SetLabelUIElement(x => x.ValueType, model => model.ValueType.ToLocalizedString(), model => model.ValueType.ToColorHex())
                .SetBooleanUIElement(x => x.IsEditable)
                .SetBooleanUIElement(x => x.IsRequired)
                .SetBooleanUIElement(x => x.IsPreview)
                .SetSubElement(x => x.Attributes,
                               model => model.Attributes?.Count().ToString("attribute", "attributes", "No attributes"),
                               async (row, model) =>
                               {
                                   var itemsControl = new WrapPanelItemsControl<ColumnAttribute, BorderedTextBlock>(x =>
                                   {
                                       var label = ControlsFactory.CreateLabelTag(x.Name, x.Color);

                                       label.Margin = new Thickness(NormalUniformMargin);

                                       return label;
                                   })
                                   {
                                       Orientation = Orientation.Horizontal,
                                       HorizontalAlignment = HorizontalAlignment.Center
                                   };

                                   itemsControl.SetItemsSource(model.Attributes);

                                   return await Task.FromResult(itemsControl);
                               },
                               (row, model, element) => element.SetItemsSource(model.Attributes))
                .SetTextInputUIElement(x => x.Order, async (row, model, value) =>
                {
                    var result = await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync();

                    return new Failable<PropertyMap>() { ErrorMessage = result.ErrorMessage };
                })

                .ConfigureOptions((container, grid, row, model) =>
                {
                    container.AddEditOption("Property map modification", null, () =>
                     {
                         return new DataForm<PropertyMap>() { Mapper = CeidDiplomatikiDataModelHelpers.PropertyMapMapper.Value }
                         .ShowInput(x => x.Name, settings => settings.IsRequired = true)
                         .ShowInput(x => x.Description)
                         .ShowStringColorInput(x => x.Color)
                         .ShowSelectMultipleOptionsInput(x => x.Attributes, (form, propertyInfo) => new DropDownMenuOptionsFormInput<ColumnAttribute>(form, propertyInfo, ColumnAttributes.Data.Value, x => x.Name), settings => settings.IsRequired = true)
                         .ShowNumericInput(x => x.Order)
                         .ShowInput(x => x.DefaultValue)
                         .ShowInput(x => x.IsEditable)
                         .ShowInput(x => x.IsRequired)
                         .ShowInput(x => x.IsPreview);
                     },  async (model) => await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync(), null, IconPaths.TableColumnPath);
                     container.AddDeleteOption("Property map deletion", null, async (model) =>
                     {
                         // Get the manager
                         var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;
                     
                         // Unregister the map
                         QueryMap.Remove(model);
                     
                         // Save the changes
                         return await manager.SaveChangesAsync();
                     }, null, IconPaths.TableColumnPath);
                });
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

                // Create a steps presenter
                var stepsPresenter = new StepsPresenter() { AllowArbitraryNavigation = false };

                // Create a columns container
                var propertiesContainer = new UniformGridCollapsibleVerticalMenuCategorizingItemsControl<PropertyInfo, InformationalButton>()
                {
                    Columns = 3,
                    CategoryNameGetter = (prop) => prop.DeclaringType.Name.Split("-").Last()
                };

                propertiesContainer.Linker = (prop) => new InformationalButton()
                {
                    MinWidth = 0,
                    Text = prop.Name,
                    Command = new RelayCommand(() =>
                    {
                        foreach (var element in propertiesContainer.Elements)
                            element.Selected = false;

                        propertiesContainer.GetElement(prop).Selected = true;
                    })
                };

                // Add the tables
                propertiesContainer.SetItemsSource(QueryMap.DataModelTypes.SelectMany(x => x.GetProperties().Where(y => !DataPresenter.Items.Any(z => z.PropertyInfo == y))).OrderBy(x => x.Name));

                // Add it to the steps presenter
                stepsPresenter.Add("Property", propertiesContainer, (element) => element.Elements.Any(x => x.Selected));

                // Create the form
                var form = new DataForm<PropertyMap>() { Mapper = CeidDiplomatikiDataModelHelpers.PropertyMapMapper.Value }
                    .ShowInput(x => x.Name, settings => settings.IsRequired = true)
                    .ShowInput(x => x.Description)
                    .ShowStringColorInput(x => x.Color)
                    .ShowSelectMultipleOptionsInput(x => x.Attributes, (form, propertyInfo) => new DropDownMenuOptionsFormInput<ColumnAttribute>(form, propertyInfo, ColumnAttributes.Data.Value, x => x.Name), null, settings => settings.IsRequired = true)
                    .ShowNumericInput(x => x.Order)
                    .ShowInput(x => x.DefaultValue)
                    .ShowInput(x => x.IsEditable)
                    .ShowInput(x => x.IsRequired)
                    .ShowInput(x => x.IsPreview);

                // Add it to the steps presenter
                stepsPresenter.Add("Info", form, (element) => element.Validate());

                // Show a dialog
                var dialogResult = await DialogHelpers.ShowStepsDialogAsync(this, "Property map creation", null, stepsPresenter, IconPaths.TableColumnPath);

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                {
                    // Re enable the button
                    AddButton.IsEnabled = true;

                    // Return
                    return;
                }

                // Get the selected property
                var property = propertiesContainer.Get(propertiesContainer.Elements.First(x => x.Selected));

                // Create the model
                var model = new PropertyMap(QueryMap, property.DeclaringType, property, QueryMap.GetColumnOrNull(property));

                // Set it to the form
                form.Model = model;

                // Update its values
                form.UpdateModelValues();

                // Get the manager
                var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                // Register the model
                QueryMap.Add(model);

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
