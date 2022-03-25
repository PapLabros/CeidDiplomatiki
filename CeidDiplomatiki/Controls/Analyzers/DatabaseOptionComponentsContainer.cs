using Atom.Windows.Controls;

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using static Atom.Core.Personalization;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Contains multiple database options components
    /// </summary>
    public class DatabaseOptionComponentsContainer : BaseControl
    {
        #region Private Members

        /// <summary>
        /// Mapper that maps the database options with their related components
        /// </summary>
        private readonly Dictionary<BaseDatabaseOptionsDataModel, BaseDatabaseOptionsComponent> mMapper = new Dictionary<BaseDatabaseOptionsDataModel, BaseDatabaseOptionsComponent>();

        #endregion

        #region Protected Properties

        /// <summary>
        /// The stack panel that contains all the content
        /// </summary>
        protected StackPanel ContentStackPanel { get; private set; }

        /// <summary>
        /// The stack panel that contains the <see cref="OptionSelectorTextBlock"/> and the <see cref="OptionSelectorDropDown"/>
        /// </summary>
        protected StackPanel OptionSelectorStackPanel { get; private set; }

        /// <summary>
        /// The option selector text block
        /// </summary>
        protected TextBlock OptionSelectorTextBlock { get; private set; }

        /// <summary>
        /// The option selector drop down
        /// </summary>
        protected DropDownMenu<BaseDatabaseOptionsDataModel> OptionSelectorDropDown { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseOptionComponentsContainer() : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an options component
        /// </summary>
        /// <typeparam name="TOptions">The type of the options</typeparam>
        /// <param name="component">The component</param>
        public void Add<TOptions>(BaseDatabaseOptionsComponent<TOptions> component)
            where TOptions : BaseDatabaseOptionsDataModel
        {
            // Start with the component collapsed
            component.Visibility = Visibility.Collapsed;

            // Map them
            mMapper.Add(component.Options, component);

            // Add it to the content stack panel
            ContentStackPanel.Children.Add(component);

            // Add the options to the option selector
            OptionSelectorDropDown.Add(component.Options);

            // If there is a selected option...
            if (OptionSelectorDropDown.Value == null)
                // Select this one
                OptionSelectorDropDown.Select(component.Options);
        }

        /// <summary>
        /// Gets the selected options
        /// </summary>
        public BaseDatabaseOptionsDataModel SelectedOptions => OptionSelectorDropDown.Value;

        /// <summary>
        /// Shows the component that presents the specified <paramref name="options"/>
        /// </summary>
        /// <param name="options">The options</param>
        public void Show(BaseDatabaseOptionsDataModel options) => OptionSelectorDropDown.Select(options);

        /// <summary>
        /// Validates the data of the currently visible component
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            // Get the visible component
            var component = mMapper.Values.First(x => x.IsVisible);

            // Validate the component
            return component.Validate();
        }

        /// <summary>
        /// Updates the data of the options data model with the values inserted by the user.
        /// NOTE: Before calling this method the <see cref="Validate"/> method should get called
        ///       to ensure the validity of the data!
        /// </summary>
        public void UpdateOptionData()
        {
            // Get the selected component
            var component = mMapper.First(x => x.Key.Provider == SelectedOptions.Provider).Value;

            // Update the option data that this component represents
            component.UpdateOptionData();
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
            ContentStackPanel = new StackPanel() { Orientation = Orientation.Vertical };

            // Create the option selector stack panel
            OptionSelectorStackPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };

            // Create the option selector text block
            OptionSelectorTextBlock = ControlsFactory.CreateStandardTextBlock("Selected database provider:");

            // Add it to the option selector stack panel
            OptionSelectorStackPanel.Children.Add(OptionSelectorTextBlock);

            // Create the option selector drop down
            OptionSelectorDropDown = new DropDownMenu<BaseDatabaseOptionsDataModel>((options) => options.Name)
            {
                Margin = new Thickness(NormalUniformMargin)
            };

            OptionSelectorDropDown.ValueChanged += new DependencyPropertyChangedEventHandler((sender, e) =>
            {
                // Get the selected options
                var options = (BaseDatabaseOptionsDataModel)e.NewValue;

                // For every option-component pair...
                foreach (var pair in mMapper)
                {
                    if (pair.Key == options)
                        pair.Value.Visibility = Visibility.Visible;
                    else
                        pair.Value.Visibility = Visibility.Collapsed;
                }
            });

            // Add it to the option selector stack panel
            OptionSelectorStackPanel.Children.Add(OptionSelectorDropDown);

            // Add the option selector stack panel to the content stack panel
            ContentStackPanel.Children.Add(OptionSelectorStackPanel);

            // Return the content stack panel
            return ContentStackPanel;
        }

        #endregion
    }
}
