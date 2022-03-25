using Atom.Core;
using Atom.Windows;
using Atom.Windows.Controls;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The base for all the database options components
    /// </summary>
    public abstract class BaseDatabaseOptionsComponent : BaseControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDatabaseOptionsComponent()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates the inserted data
        /// </summary>
        /// <returns></returns>
        public abstract bool Validate();

        /// <summary>
        /// Updates the data of the options data model with the values inserted by the user.
        /// NOTE: Before calling this method the <see cref="Validate"/> method should get called
        ///       to ensure the validity of the data!
        /// </summary>
        public abstract void UpdateOptionData();

        #endregion
    }

    /// <summary>
    /// The base for all the database options components
    /// </summary>
    /// <typeparam name="TOptions">The type of the options</typeparam>
    public abstract class BaseDatabaseOptionsComponent<TOptions> : BaseDatabaseOptionsComponent
        where TOptions : BaseDatabaseOptionsDataModel
    {
        #region Public Properties

        /// <summary>
        /// The options
        /// </summary>
        public TOptions Options { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The stack panel that contains all the content
        /// </summary>
        protected StackPanel ContentStackPanel { get; private set; }

        /// <summary>
        /// The options form
        /// </summary>
        protected DataForm<TOptions> OptionsForm { get; private set; }

        /// <summary>
        /// The options container
        /// </summary>
        protected OptionsContainer OptionsContainer { get; private set; }

        /// <summary>
        /// The connection string button
        /// </summary>
        protected IOptionButton ConnectionStringButton { get; private set; }

        /// <summary>
        /// The test button
        /// </summary>
        protected IOptionButton TestButton { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="options">The options</param>
        public BaseDatabaseOptionsComponent(TOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            OptionsForm.Model = options;
            OptionsForm.UpdateFormValues();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates the inserted data
        /// </summary>
        /// <returns></returns>
        public override bool Validate() => OptionsForm.Validate();

        /// <summary>
        /// Updates the data of the options data model with the values inserted by the user.
        /// NOTE: Before calling this method the <see cref="Validate"/> method should get called
        ///       to ensure the validity of the data!
        /// </summary>
        public override void UpdateOptionData() => OptionsForm.UpdateModelValues();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and returns the GUI in a form of a <see cref="FrameworkElement"/>
        /// </summary>
        /// <returns></returns>
        protected override FrameworkElement CreateBaseGUIElement()
        {
            // Create the content stack panel
            ContentStackPanel = new StackPanel();

            // Create the form
            OptionsForm = CreateForm();

            // Add it to the content grid
            ContentStackPanel.Children.Add(OptionsForm);

            // Create the options container
            OptionsContainer = new OptionsContainer() { HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, NormalUniformMargin, 0, NormalUniformMargin) };

            // Create the connections string button
            ConnectionStringButton = OptionsContainer.AddOption(
                "connectionString", 
                async (button) =>
                {
                    if (!OptionsForm.Validate())
                        return;

                    OptionsForm.UpdateModelValues();

                    // If the connection string is valid...
                    if (OptionsForm.Model.TryGetConnectionString(out var connectionString))
                    {
                        // Format the connection string
                        connectionString = FormatValidConnectionString(connectionString);

                        // Copy to clip board
                        Clipboard.SetText(connectionString);

                        // Show a success dialog
                        await DialogHelpers.ShowInfoHintDialogAsync(this, $"Connection string copied!");
                    }
                    // Else...
                    else
                        // Show an error dialog
                        await DialogHelpers.ShowErrorHintDialogAsync(this, $"Invalid connection string!");
                },
                IconPaths.LockPath,
                "Connection string",
                Blue.ToColor());

            // Create the test button
            TestButton = OptionsContainer.AddOption(
                "test", 
                async (button) =>
                {
                    if (!OptionsForm.Validate())
                        return;

                    OptionsForm.UpdateModelValues();

                    // Disable the button
                    TestButton.IsEnabled = false;

                    // If the connection string is valid...
                    if (OptionsForm.Model.TryGetConnectionString(out var connectionString))
                    {
                        // Format the connection string
                        connectionString = FormatValidConnectionString(connectionString);

                        // Show a loading dialog
                        var component = await DialogHelpers.ShowLoadingHintDialogAsync(this);

                        // Attempt to connect
                        var result = await IsConnectionValidAsync(Options, connectionString);

                        // If there was an error...
                        if (!result.Successful)
                        {
                            // Attempt to connect to the server and not to the database
                            var nonDatabaseResult = await IsConnectionValidAsync(Options, RemoveDatabaseFromConnectionString(Options, connectionString));


                            // If there wasn't an error... 
                            if (nonDatabaseResult.Successful)
                                result = new Failable() { ErrorType = ErrorType.Warning, ErrorMessage = "Valid connection string but non existing database" };
                        }

                        // Hide the loading dialog
                        component.Result.SetNoInteraction();

                        // Enable the button
                        TestButton.IsEnabled = true;

                        if (result.Successful)
                            await DialogHelpers.ShowSuccessHintDialogAsync(this, $"Valid connection string!");
                        else
                            await DialogHelpers.ShowErrorHintDialogAsync(this, result.ErrorMessage);

                        // Return
                        return;
                    }

                    // Enable the button
                    TestButton.IsEnabled = true;

                    // Show an error dialog
                    await DialogHelpers.ShowErrorHintDialogAsync(this, $"Valid connection string!");
                }, IconPaths.TestTubeEmptyPath, "Test", Blue.ToColor());

            // Add the options container to the content stack panel
            ContentStackPanel.Children.Add(OptionsContainer);

            // Return the content stack panel
            return ContentStackPanel;
        }

        /// <summary>
        /// Further formats the already valid connection string.
        /// Ex. Remove the MySQL default port connection string part.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected virtual string FormatValidConnectionString(string connectionString) => connectionString;

        /// <summary>
        /// Removes the database part from the specified <paramref name="connectionString"/>
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected abstract string RemoveDatabaseFromConnectionString(TOptions options, string connectionString);

        /// <summary>
        /// Creates and returns the <see cref="BaseDatabaseOptionsComponent{TOptions}.OptionsForm"/>
        /// </summary>
        /// <returns></returns>
        protected abstract DataForm<TOptions> CreateForm();

        /// <summary>
        /// Checks whether the specified valid <paramref name="connectionString"/> contained in the specified 
        /// <paramref name="options"/> can open a connection or not
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns></returns>
        protected abstract Task<IFailable> IsConnectionValidAsync(TOptions options, string connectionString);

        #endregion
    }
}
