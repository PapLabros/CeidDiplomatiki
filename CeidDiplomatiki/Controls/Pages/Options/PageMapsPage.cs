using Atom.Core;
using Atom.Windows.Controls;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The page used for configuring the CeidDiplomatiki pages
    /// </summary>
    public class PageMapsPage : BaseFullyInitializableDataPresenterPage<PageMap>
    {
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
        public PageMapsPage() : base()
        {
            CreateGUI();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseItemsControlPage{TClass}.DataPresenter"/>
        /// </summary>
        /// <returns></returns>
        protected override IDataPresenter<PageMap> CreateDataPresenter()
        {
            return new CollapsibleDataGrid<PageMap>() { Mapper = CeidDiplomatikiDataModelHelpers.PageMapMapper.Value }
                .ShowData(x => x.Name)
                .ShowData(x => x.Color)
                .ShowData(x => x.Category)
                .ShowData(x => x.Order)
                .ShowData(x => x.Presenter)
                .ShowData(x => x.Pages)

                .SetColorUIElement(x => x.Color)
                .SetDataPresenterSubElement(x => x.Pages,
                                            model => model.Pages.Count.ToString("page", "pages", "No pages"),
                                            model => CreateDataPresenter(),
                                            (presenter, model, button) =>
                                            {
                                                button.VectorSource = IconPaths.PlusPath;

                                                button.Command = new RelayCommand(async () =>
                                                {
                                                    // Create the form
                                                    var form = CeidDiplomatikiDataModelHelpers.CreatePageMapDataForm();

                                                    // Set the model
                                                    form.Model = new PageMap() { Parent = model };

                                                    // Show the dialog
                                                    var dialogResult = await DialogHelpers.ShowConventionalAddDialogAsync(this, "Page creation", null, form);

                                                    // If we didn't get positive feedback...
                                                    if (!dialogResult.Feedback)
                                                        // Return
                                                        return;

                                                    // Update the values of the model
                                                    form.UpdateModelValues();

                                                    // Add the model to the parent model
                                                    model.Pages.Add(form.Model);

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

                                                    // Add it to the presenter
                                                    presenter.Add(form.Model);
                                                });
                                            })

                .ConfigureOptions((container, grid, row, model) =>
                {
                    container.AddEditOption("Page modification", null, () => CeidDiplomatikiDataModelHelpers.CreatePageMapDataForm(), async (model) => await CeidDiplomatikiDI.GetCeidDiplomatikiManager.SaveChangesAsync());
                    container.AddDeleteOption("Page deletion", null, async (model) =>
                    {
                        // Get the manger
                        var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                        // If there is a parent...
                        if (model.Parent != null)
                            // Remove the model form the parent
                            model.Parent.Pages.Remove(model);
                        // Else...
                        else
                            // This is a root page so remove it from the manager
                            manager.Unregister(model);

                        // Save the changes
                        return await manager.SaveChangesAsync();
                    });
                });
        }

        /// <summary>
        /// Gets all the data that need to be presented
        /// </summary>
        /// <returns></returns>
        protected async override Task<IFailable<IEnumerable<PageMap>>> GetAllDataAsync()
        {
            // Get the manager
            var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

            // Get the root page maps
            return await Task.FromResult(new Failable<IEnumerable<PageMap>>(manager.RootPages));
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
                var form = CeidDiplomatikiDataModelHelpers.CreatePageMapDataForm();

                // Set the model
                form.Model = new PageMap();

                // Show the dialog
                var dialogResult = await DialogHelpers.ShowConventionalAddDialogAsync(this, "Page creation", null, form);

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                    // Return
                    return;

                // Update the values of the model
                form.UpdateModelValues();

                // Get the manager
                var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                // Register it
                manager.Register(form.Model);

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

                // Add it to the presenter
                DataPresenter.Add(form.Model);
            });

            // Add it to the content grid
            ContentGrid.Children.Add(AddButton);
        }

        #endregion
    }
}
