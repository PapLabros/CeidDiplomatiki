using Atom.Core;
using Atom.Relational;
using Atom.Relational.Providers;
using Atom.Windows.Controls;
using Atom.Windows.Controls.Relational;
using Atom.Windows.Controls.Relational.Providers;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The databases page
    /// </summary>
    public class DatabasesPage : BaseFullyInitializableItemsControlPage<BaseDatabaseOptionsDataModel>
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
        public DatabasesPage() : base()
        {
            CreateGUI();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the <see cref="BaseItemsControlPage{TClass}.DataPresenter"/>
        /// </summary>
        /// <returns></returns>
        protected override IItemsControl<BaseDatabaseOptionsDataModel> CreateItemsControl() => new DatabasesPresenter();

        /// <summary>
        /// Gets all the data that need to be presented
        /// </summary>
        /// <returns></returns>
        protected override async Task<IFailable<IEnumerable<BaseDatabaseOptionsDataModel>>> GetAllDataAsync()
        {
            // Get the manager
            var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

            return await Task.FromResult(new Failable<IEnumerable<BaseDatabaseOptionsDataModel>>() { Result = manager.Databases });
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
                // Create the component
                var component = new DatabaseOptionComponentsContainer();

                // Add the options
                component.Add(new SQLiteOptionsComponent(new SQLiteOptionsDataModel()));
                component.Add(new MySQLOptionsComponent(new MySQLOptionsDataModel()));
                component.Add(new SQLServerOptionsComponent(new SQLServerOptionsDataModel()));
                component.Add(new PostgreSQLOptionsComponent(new PostgreSQLOptionsDataModel()));

                // Show the dialog
                var dialogResult = await DialogHelpers.ShowValidationDialogAsync(this, "Database addition", null, component, (element) =>
                {
                    // TODO: Make sure that the database exists
                    if (!element.Validate())
                        return false;

                    element.UpdateOptionData();

                    return true;
                });

                // If we didn't get positive feedback...
                if (!dialogResult.Feedback)
                    // Return
                    return;

                // Get the options
                var options = component.SelectedOptions;

                // Get the manager
                var manager = CeidDiplomatikiDI.GetCeidDiplomatikiManager;

                // Register the options
                manager.Register(options);

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

                // Add them to the presenter
                DataPresenter.Add(options);
            });

            // Add it to the content grid
            ContentGrid.Children.Add(AddButton);
        }

        #endregion
    }
}
