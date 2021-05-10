using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static Atom.Personalization.Constants;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The data grid presenter page
    /// </summary>
    public class DataGridPresenterPage : BasePresenterPage<DataGridPresenterMap>
    {
        #region Private Members

        /// <summary>
        /// A cancellation token source used for canceling the load request
        /// </summary>
        private CancellationTokenSource mLoadCancellationToken;

        /// <summary>
        /// A flag indicating whether the next page of items is loading
        /// </summary>
        private bool mIsNextPageLoading = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// The data grid map of the main data grid
        /// </summary>
        public DataGridMap DataGridMap { get; }

        /// <summary>
        /// The mapper that maps the properties of the type of the <see cref="DataGridMap"/>
        /// </summary>
        public IPropertyMapper Mapper { get; }

        /// <summary>
        /// The args
        /// </summary>
        public DataGridPresenterArgs Args { get; }

        /// <summary>
        /// The day span used for limiting the results
        /// </summary>
        public DaySpan? DaySpan
        {
            get => Args.After == null || Args.Before == null ? (DaySpan?)null : new DaySpan(Args.After.Value.DateTime, Args.Before.Value.DateTime);

            set
            {
                Args.After = value?.StartingDate;
                Args.Before = value?.EndingDate;
            }
        }

        /// <summary>
        /// The data storage
        /// </summary>
        public DataGridPresenterDataStorage DataStorage { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The scroll viewer that contains the <see cref="DataPresenter"/>
        /// </summary>
        protected ScrollViewer DataPresenterScrollViewer { get; private set; }

        /// <summary>
        /// Represents a <see cref="DataGrid{TClass}"/> that contains all the content
        /// </summary>
        protected ICollapsibleDataPresenter DataPresenter { get; private set; }

        /// <summary>
        /// The options container
        /// </summary>
        protected CollapsibleOptionsContainer OptionsContainer { get; private set; }

        /// <summary>
        /// The add button.
        /// NOTE: The add button is only visible when the <see cref="DataGridMap.AllowAdd"/> of the data grid map
        ///       that represents the <see cref="QueryMap.RootType"/> is set to <see cref="true"/>!
        /// </summary>
        protected IconButton AddButton { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="presenterMap">The presenter map</param>
        /// <param name="pageMap">The page map related to this page</param>
        /// <param name="args">The args</param>
        public DataGridPresenterPage(DataGridPresenterMap presenterMap, PageMap pageMap, DataGridPresenterArgs args = null) : base(presenterMap, pageMap)
        {
            DataGridMap = presenterMap.DataGrids.First(x => x.Type == presenterMap.QueryMap.RootType);
            Mapper = DataGridMap.CreatePropertyMapper();
            Args = args ?? new DataGridPresenterArgs() { PerPage = 10 };
            DataStorage = new DataGridPresenterDataStorage(presenterMap);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the page
        /// </summary>
        public virtual async void Update()
        {
            // Clear
            DataPresenter.Clear();

            // Reset the page index
            Args.Page = 0;

            // Cancel any pending loadings
            mLoadCancellationToken?.Cancel();

            // Load the next items
            await LoadNextAsync();
        }

        /// <summary>
        /// Loads the next set of data
        /// </summary>
        /// <returns></returns>
        public async Task LoadNextAsync()
        {
            // Cancel any pending requests
            mLoadCancellationToken?.Cancel();

            // Create a cancellation token
            var cancellationToken = new CancellationTokenSource();

            // Set it
            mLoadCancellationToken = cancellationToken;

            // Show the loading dialog component
            var component = await DialogHelpers.ShowLoadingHintDialogAsync(this);

            // Mark that we are loading
            mIsNextPageLoading = true;

            // Get the models
            var result = await DataStorage.GetDataAsync(Args);

            // Hide the loading component
            component.SetCompleted();

            // Mark that we finished loading
            mIsNextPageLoading = false;

            // If there was an error...
            if (!result.Successful)
            {
                // Show the error
                await result.ShowDialogAsync(this);

                // Return
                return;
            }

            // If the loading is cancelled...
            if (cancellationToken.IsCancellationRequested)
                // Return
                return;

            // If no more items were returned...
            if (DataPresenter.ItemsCount > 0 && result.Result.Cast<object>().Count() == 0)
            {
                // Show a hint dialog
                await DialogHelpers.ShowInfoHintDialogAsync(this, GetNoMoreDataWereRetrievedMessage());

                // Return
                return;
            }

            // If no data at all were retrieved...
            if (result.Result.Cast<object>().Count() == 0)
            {
                // Show a hint dialog
                await DialogHelpers.ShowInfoHintDialogAsync(this, GetNoDataWereRetrievedMessage());

                // Return
                return;
            }

            // Add the models
            DataPresenter.AddRange(result.Result.Cast<object>());
            
            // Increment the page index
            Args.Page++;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes the page
        /// </summary>
        /// <returns></returns>
        protected override Task<IFailable> InitializeAsync() => Task.FromResult<IFailable>(new Failable());

        /// <summary>
        /// Handles the successful initialization. 
        /// NOTE: Usually used for creating the UI elements after the initialization succeeded!
        /// </summary>
        /// <returns></returns>
        protected async override Task OnSuccessfulInitializationAsync()
        {
            // Create the data grid type
            var dataGridType = typeof(CollapsibleDataGrid<>).MakeGenericType(PresenterMap.QueryMap.RootType);

            // Create the data presenter
            DataPresenter = (ICollapsibleDataPresenter)Activator.CreateInstance(dataGridType);

            DataPresenter.Element.Margin = new Thickness(LargeUniformMargin);

            // If there are search columns...
            if (!DataGridMap.SearchColumns.IsNullOrEmpty())
            {
                // Get the set search rules method
                var method = GetType().GetMethod(nameof(SetSearchRules), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(DataGridMap.Type);

                // Call it
                method.Invoke(this, new object[] { DataPresenter });
            }

            // Wrap it in
            DataPresenterScrollViewer = ControlsFactory.WrapInScrollViewer(DataPresenter.Element);

            DataPresenterScrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(async (sender, e) =>
            {
                if (e.Delta >= 0)
                    return;

                if (DataPresenterScrollViewer.VerticalOffset == DataPresenterScrollViewer.ScrollableHeight)
                {
                    if (mIsNextPageLoading)
                        return;

                    await LoadNextAsync();
                }
            });

            // Add it to the content grid
            ContentGrid.Children.Add(DataPresenterScrollViewer);

            // Create the options container
            OptionsContainer = new CollapsibleOptionsContainer()
            {
                Margin = new Thickness(LargeUniformMargin),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                FirstQaudrantPlacement = true,
                SecondQaudrantPlacement = false,
                ThirdQaudrantPlacement = false,
                FourthQaudrantPlacement = false
            };

            // Add the preview options button
            OptionsContainer.AddSettingsOption(async (button) =>
            {
                // Create the options selection form
                var form = new OptionsSelectionForm<PropertyInfo>("Show");

                // For every property...
                foreach (var property in DataGridMap.Type.GetProperties())
                    // Show an option
                    form.ShowOption(property, (string)Mapper.UnsafeGet(PropertyMapperExtensions.Title, property, property.Name), null, null, DataGridMap.Columns.Any(x => x.Name == property.Name));

                // Show a dialog
                var dialogResult = await DialogHelpers.ShowSelectMultipleDialogAsync(this, "Field selection", null, form, IconPaths.SettingsPath);

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                    // Return
                    return;

                // Get the selected columns
                var columns = form.GetOptions();

                // Set them to the map
                DataGridMap.Columns = columns;

                // Save the changes
                var result = await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync();

                // If there was an error...
                if (!result.Successful)
                {
                    // Show the error
                    await DialogHelpers.ShowErrorDialogAsync(this, result.ErrorMessage);

                    // Return
                    return;
                }

                // Cleat the columns of the presenter
                DataPresenter.HideAllData();

                // For every selected column...
                foreach (var columnAndOrderPair in DataGridMap.Columns.ToDictionary(x => x, prop => QueryMap.PropertyMaps.FirstOrDefault(x => x.PropertyInfo == prop)?.Order ?? int.MaxValue).OrderBy(x => x.Value))
                {
                    var column = columnAndOrderPair.Key;

                    // Show the data
                    DataPresenter.UnsafeShowData(column, null);
                }
            });

            // If there is a data column...
            if (DataGridMap.DateColumn != null)
            {
                OptionsContainer.AddDaySpanLimitOption(() => DaySpan, (daySpan) =>
                {
                    Update();
                });
            }

            // Add it to the content grid
            ContentGrid.Children.Add(OptionsContainer);

            // Get the phone number property map if any
            var phoneNumberPropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.PhoneNumber)));

            // Get the email property map if any
            var emailPropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.Email)));

            // Get the first name property map if any
            var firstNamePropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.FirstName)));

            // Get the last name property map if any
            var lastNamePropertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.Attributes.Any(y => y.Equals(ColumnAttributes.LastName)));

            // Configure the presenter
            ConfigurePresenter(PresenterMap, DataGridMap, DataPresenter);

            // Create the add button
            AddButton = ControlsFactory.CreateStandardAddCircularButton();

            AddButton.Command = new RelayCommand(async () =>
            {
                // Disable the button
                AddButton.IsEnabled = false;

                // Create the form type
                var formType = typeof(DataForm<>).MakeGenericType(DataGridMap.Type);

                // Create the form
                var form = (IDataForm)Activator.CreateInstance(formType);

                form.Model = DataGridMap.CreateModelInstance();

                // For every property map related to this data grid...
                foreach (var propertyMap in QueryMap.PropertyMaps.Where(x => x.Type == DataGridMap.Type && (x.IsEditable || x.IsPreview)).OrderBy(x => x.Order))
                {
                    if (propertyMap.Attributes.Any(x => x.Equals(ColumnAttributes.HEXColor)))
                    {
                        // Show a string color input
                        form.UnsafeShowInput(propertyMap.PropertyInfo, settings => 
                        {
                            settings.CustomFormInputImplementationFactory = (form, propertyInfo) => new StringColorFormInput(form, propertyInfo);
                            settings.Name = propertyMap.Name; 
                            settings.IsRequired = propertyMap.IsRequired;
                            settings.Description = propertyMap.Description;
                            settings.IsReadOnly = propertyMap.IsPreview;
                            settings.Color = propertyMap.Color;
                        });

                        // Continue
                        continue;
                    }

                    // Show a standard input
                    form.UnsafeShowInput(propertyMap.PropertyInfo, settings => 
                    {
                        settings.Name = propertyMap.Name;
                        settings.IsRequired = propertyMap.IsRequired;
                        settings.Description = propertyMap.Description;
                        settings.IsReadOnly = propertyMap.IsPreview;
                        settings.Color = propertyMap.Color;
                    });
                }

                // Show a dialog
                var dialogResult = await DialogHelpers.ShowValidationDialogAsync(this, "Item addition", null, form.Element, element => form.Validate(), PageMap.PathData, overlay => overlay.PositiveFeedbackText = "Add");

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                {
                    // Re enable the button
                    AddButton.IsEnabled = true;

                    // Return
                    return;
                }

                // Update the values of the model
                form.UpdateModelValues();

                // Add the model
                var result = await DialogHelpers.ShowAddLoadingHintDialogAsync(this, () => DataStorage.AddDataAsync(form.Model));

                // If there was an error....
                if (!result.Successful)
                {
                    // Re enable the button
                    AddButton.IsEnabled = true;

                    // Show the error
                    await result.ShowDialogAsync(this);

                    // Return
                    return;
                }

                // Re enable the button
                AddButton.IsEnabled = true;

                // Add it to the data presenter
                DataPresenter.Add(form.Model);

                // Show a success dialog
                await DialogHelpers.ShowSuccessHintDialogAsync(this, result.ErrorMessage);
            });

            // If the addition is now allowed...
            if (!DataGridMap.AllowAdd)
                // Hide the button
                AddButton.Visibility = Visibility.Collapsed;

            // Add it to the content grid
            ContentGrid.Children.Add(AddButton);

            // Load the next set of data
            await LoadNextAsync();
        }

        /// <summary>
        /// Gets the message that is displayed when no data were retrieved but the <see cref="DataPresenter"/> contains some data
        /// Localization
        /// </summary>
        /// <returns></returns>
        protected virtual string GetNoMoreDataWereRetrievedMessage() => "No more data were retrieved!";

        /// <summary>
        /// Gets the message that is displayed when no data were retrieved but the <see cref="DataPresenter"/> doesn't contain any data
        /// Localization
        /// </summary>
        /// <returns></returns>
        protected virtual string GetNoDataWereRetrievedMessage() => "No data were retrieved!";

        #endregion

        #region Private Methods

        /// <summary>
        /// Configures the specified <paramref name="dataPresenter"/>
        /// </summary>
        /// <param name="presenterMap">The presenter map that contains information related to all its data grids</param>
        /// <param name="dataGridMap">The data grid map of the <paramref name="dataPresenter"/></param>
        /// <param name="dataPresenter">The presenter to configure</param>
        private void ConfigurePresenter(DataGridPresenterMap presenterMap, DataGridMap dataGridMap, ICollapsibleDataPresenter dataPresenter)
        {
            // Create and set the mapper
            dataPresenter.Mapper = dataGridMap.CreatePropertyMapper();

            // For every selected column...
            foreach (var columnAndOrderPair in dataGridMap.Columns.ToDictionary(x => x, prop => QueryMap.PropertyMaps.FirstOrDefault(x => x.PropertyInfo == prop)?.Order ?? int.MaxValue).OrderBy(x => x.Value))
            {
                var column = columnAndOrderPair.Key;

                // Show the data
                dataPresenter.UnsafeShowData(column, null);

                // Get the property map if any
                var propertyMap = QueryMap.PropertyMaps.FirstOrDefault(x => x.PropertyInfo == column);

                // If the property is an enumerable...
                if (column.PropertyType != typeof(string) && column.PropertyType.IsGenericIEnumerable())
                {
                    // Get type of the models that the presenter will presenter
                    var modelType = TypeHelpers.GetNonEnumerableType(column.PropertyType);

                    // Create the presenter type
                    var presenterType = typeof(CollapsibleDataGrid<>).MakeGenericType(modelType);

                    dataPresenter.UnsafeSetSubElement(column, (model) =>
                    {
                        var value = (IEnumerable)column.GetValue(model);

                        if (value == null)
                            return "No items";

                        var count = value.Cast<object>().Count();

                        if (count == 1)
                            return "1 item";
                        else
                            return $"{count} items";
                    }, 
                    async (model) =>
                    {
                        // Get the DataGridMap related to the type that the presenter presents
                        var map = presenterMap.DataGrids.First(x => x.Type == modelType);

                        var presenter = new InnerCollapsibleDataGridPage(map);

                        // Show the data for this presenter
                        ConfigurePresenter(presenterMap, map, presenter.DataPresenter);

                        // Get the values
                        var values = (IEnumerable<object>)column.GetValue(model);

                        // Set the items source
                        presenter.ItemsSource = values;

                        // Load the first items
                        presenter.LoadNext();

                        return await Task.FromResult(presenter);
                    }, (model, element) =>
                    {
                        // Clear the existing items
                        element.Clear();

                        // Load the next items
                        element.LoadNext();
                    });
                }

                // If there is a property map...
                if (propertyMap != null)
                {
                    // If this column is a color column...
                    if (propertyMap.Attributes.Any(x => x.Equals(ColumnAttributes.HEXColor)))
                    {
                        // Set a color presenter
                        dataPresenter.UnsafeSetCustomUIElement<Border>(column, (presenter, model) =>
                        {
                            // Get the hex color value
                            var value = (string)column.GetValue(model);

                            // Return a border
                            return new Border() { Effect = EffectsFactory.CreateDropShadowEffect(), CornerRadius = new CornerRadius(NormalCornerRadius), Margin = new Thickness(NormalUniformMargin), Background = value.ToBrush() };
                        }, 
                        (presenter, model, element) => 
                        {
                            // Get the hex color value
                            var value = (string)column.GetValue(model);

                            // Update the color
                            element.Background = value.ToBrush();
                        });
                    }
                }

                // If the property is a boolean...
                if (column.PropertyType == typeof(bool))
                {
                    dataPresenter.UnsafeSetCustomUIElement(column, (presenter, model) =>
                    {
                        // Get the value
                        var value = (bool)column.GetValue(model);

                        // Create the element
                        return ControlsFactory.CreateLabelTag(value ? "Yes" : "No", value ? Green : Red);
                    },
                    (presenter, model, element) =>
                    {
                        // Get the value
                        var value = (bool)column.GetValue(model);

                        element.Text = value ? "Yes" : "No";
                        ControlsFactory.SetLabelTagColor(element, value ? Green.ToColor() : Red.ToColor());
                    });
                }
            }

            // If the data of the presenter can get manipulated...
            if (dataGridMap.AllowEdit)
            {
                dataPresenter.ConfigureOptions((container, presenter, model) =>
                {
                    container.AddOption("edit", async (button) =>
                    {
                        // Disable the button
                        button.IsEnabled = false;

                        // Create the form type
                        var formType = typeof(DataForm<>).MakeGenericType(dataGridMap.Type);

                        // Create the form
                        var form = (IDataForm)Activator.CreateInstance(formType);

                        form.Model = model;

                        // For every property map related to this data grid...
                        foreach(var propertyMap in QueryMap.PropertyMaps.Where(x => x.Type == dataGridMap.Type && (x.IsEditable || x.IsPreview)).OrderBy(x => x.Order))
                        {
                            if (propertyMap.Attributes.Any(x => x.Equals(ColumnAttributes.HEXColor)))
                            {
                                // Show a string color input
                                form.UnsafeShowInput(propertyMap.PropertyInfo, settings =>
                                {
                                    settings.CustomFormInputImplementationFactory = (form, propertyInfo) => new StringColorFormInput(form, propertyInfo);
                                    settings.Name = propertyMap.Name;
                                    settings.IsRequired = propertyMap.IsRequired;
                                    settings.Description = propertyMap.Description;
                                    settings.IsReadOnly = propertyMap.IsPreview;
                                    settings.Color = propertyMap.Color;
                                });

                                // Continue
                                continue;
                            }

                            // Show a standard input
                            form.UnsafeShowInput(propertyMap.PropertyInfo, settings =>
                            {
                                settings.Name = propertyMap.Name;
                                settings.IsRequired = propertyMap.IsRequired;
                                settings.Description = propertyMap.Description;
                                settings.IsReadOnly = propertyMap.IsPreview;
                                settings.Color = propertyMap.Color;
                            });
                        }

                        // Show a dialog
                        var dialogResult = await DialogHelpers.ShowValidationDialogAsync(this, "Item modification", null, form.Element, element => form.Validate(), PageMap.PathData, overlay => overlay.PositiveFeedbackText = "Update");

                        // If we didn't get positive feedback...
                        if (!dialogResult.Feedback)
                        {
                            // Re enable the button
                            button.IsEnabled = true;

                            // Return
                            return;
                        }

                        // Update the values of the model
                        form.UpdateModelValues();

                        // Update the item
                        var result = await DialogHelpers.ShowUpdateLoadingHintDialogAsync(this, () => DataStorage.UpdateDataAsync(model));

                        // Re enable the button
                        button.IsEnabled = true;

                        // If there was an error...
                        if (!result.Successful)
                        {
                            // Show the error
                            await result.ShowDialogAsync(this);

                            // Return
                            return;
                        }

                        // Update the data presenter
                        presenter.Update(model);

                        // Show a success dialog
                        await DialogHelpers.ShowChangesSavedHintDialogAsync(this);
                    }, IconPaths.EditPath, "Edit", Blue);
                });
            }

            // If the data of the presenter can get deleted...
            if (dataGridMap.AllowDelete)
            {
                dataPresenter.ConfigureOptions((container, presenter, model) =>
                {
                    container.AddOption("delete", async (button) =>
                    {
                        // Disable the button
                        button.IsEnabled = false;

                        // Show a transitional dialog
                        var dialogResult = await DialogHelpers.ShowDeletionTransitionalDialogAsync(this, "Item deletion", null, IconPaths.DeletePath);

                        // If we didn't get positive feedback...
                        if (!dialogResult.Feedback)
                        {
                            // Re enable the button
                            button.IsEnabled = true;

                            // Return
                            return;
                        }

                        // Delete the data
                        var result = await DialogHelpers.ShowDeleteLoadingHintDialogAsync(this, () => DataStorage.DeleteDataAsync(model));

                        // If there was an error...
                        if (!result.Successful)
                        {
                            // Re enable the button
                            button.IsEnabled = true;

                            // Show the error
                            await result.ShowDialogAsync(this);

                            // Return
                            return;
                        }

                        // Remove the model from the data presenter
                        presenter.Remove(model);

                        // Re enable the button
                        button.IsEnabled = true;
                    }, IconPaths.DeletePath, "Delete", Red);
                });
                
            }
        }

        /// <summary>
        /// Sets a search rule to the specified <paramref name="dataGrid"/>
        /// </summary>
        /// <typeparam name="TClass">The type of the models</typeparam>
        /// <param name="dataGrid">The data grid</param>
        private void SetSearchRules<TClass>(CollapsibleDataGrid<TClass> dataGrid)
            where TClass : class
        {
            dataGrid.ConfigureFilters((container, grid) =>
            {
                container.AddSearchFilter(value =>
                {
                    Args.Search = value;

                    Update();
                });
            });
        }

        #endregion

        #region Protected Classes

        protected class InnerCollapsibleDataGridPage : BaseControl
        {
            #region Private Members

            /// <summary>
            /// The current page index
            /// </summary>
            private int mCurrentPageIndex = 0;

            #endregion

            #region Public Properties

            /// <summary>
            /// The data grid map that maps information related to the <see cref="DataPresenter"/>
            /// </summary>
            public DataGridMap Map { get; }

            /// <summary>
            /// The data presenter
            /// </summary>
            public ICollapsibleDataPresenter DataPresenter { get; private set; }

            /// <summary>
            /// The mapper
            /// </summary>
            public IPropertyMapper Mapper { get; private set; }

            /// <summary>
            /// The items source
            /// </summary>
            public IEnumerable<object> ItemsSource { get; set; }

            #endregion

            #region Protected Properties

            /// <summary>
            /// The grid that contains all the content
            /// </summary>
            protected Grid ContentGrid { get; private set; }

            /// <summary>
            /// The add button
            /// </summary>
            protected IconButton AddButton { get; private set; }
            
            /// <summary>
            /// The load next button
            /// </summary>
            protected IconButton LoadNextButton { get; private set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Default constructor
            /// </summary>
            public InnerCollapsibleDataGridPage(DataGridMap map) : base()
            {
                Map = map ?? throw new ArgumentNullException(nameof(map));

                CreateGUI();
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Loads the next set of items
            /// </summary>
            public void LoadNext()
            {
                // Get the items
                var items = ItemsSource.Skip(mCurrentPageIndex * 10).Take(10);

                // If there aren't any items...
                if (items.IsNullOrEmpty())
                {
                    // Hide the button
                    LoadNextButton.Visibility = Visibility.Collapsed;

                    // Return
                    return;
                }

                // Add them to the presenter
                DataPresenter.AddRange(items);

                // Increment the page index
                mCurrentPageIndex++;

                // Get the number of pages
                var numberOfPages = ItemsSource.Count() / 10;

                // If the aren't any more items...
                if (numberOfPages < mCurrentPageIndex)
                    // Hide the button
                    LoadNextButton.Visibility = Visibility.Collapsed;
            }

            /// <summary>
            /// Clears the items
            /// </summary>
            public void Clear()
            {
                mCurrentPageIndex = 0;

                DataPresenter.Clear();
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// Creates and returns the GUI in a form of a <see cref="FrameworkElement"/>
            /// </summary>
            /// <returns></returns>
            protected override FrameworkElement CreateBaseGUIElement()
            {
                // Create the content grid
                ContentGrid = new Grid();

                // Return the content grid
                return ContentGrid;
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Creates and adds the required GUI elements
            /// </summary>
            private void CreateGUI()
            {
                // Create the data presenter
                DataPresenter = (ICollapsibleDataPresenter)Activator.CreateInstance(typeof(CollapsibleDataGrid<>).MakeGenericType(Map.Type));

                DataPresenter.Mapper = Map.CreatePropertyMapper();

                DataPresenter.Element.Margin = new Thickness(LargeUniformMargin);

                // Add it to the content grid
                ContentGrid.Children.Add(DataPresenter.Element);

                // Create the add button
                AddButton = ControlsFactory.CreateStandardAddCircularButton();

                AddButton.Command = new RelayCommand(() =>
                {

                });

                AddButton.Visibility = Map.AllowAdd ? Visibility.Visible : Visibility.Collapsed;

                // Add it to the content grid
                ContentGrid.Children.Add(AddButton);

                // Create the load more button
                LoadNextButton = ControlsFactory.CreateCircularIconButton(IconPaths.ChevronDownPath, SmallButtonSize, true);

                LoadNextButton.Command = new RelayCommand(() => LoadNext());

                LoadNextButton.Margin = new Thickness(NormalUniformMargin);
                LoadNextButton.VerticalAlignment = VerticalAlignment.Bottom;
                LoadNextButton.HorizontalAlignment = HorizontalAlignment.Center;

                // Add it to the content grid
                ContentGrid.Children.Add(LoadNextButton);
            }

            #endregion
        }

        #endregion
    }
}
