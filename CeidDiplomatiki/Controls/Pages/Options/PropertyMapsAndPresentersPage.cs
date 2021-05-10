using Atom.Windows.Controls;

using System;
using System.Windows;
using System.Windows.Controls;

using static Atom.Personalization.Constants;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The property maps and presenters page
    /// </summary>
    public class PropertyMapsAndPresentersPage : BaseControl
    {
        #region Public Properties

        /// <summary>
        /// The query map
        /// </summary>
        public QueryMap QueryMap { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The scroll viewer that contains the <see cref="ContentStackPanel"/>
        /// </summary>
        protected ScrollViewer ContentScrollViewer { get; private set; }

        /// <summary>
        /// The stack panel that contains all the content
        /// </summary>
        protected SeparatedStackPanelItemsControl ContentStackPanel { get; private set; }

        /// <summary>
        /// The container that contains the <see cref="ColumnMapsPage"/>
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> ColumnMapsPageContainer { get; private set; }

        /// <summary>
        /// The column maps page
        /// </summary>
        protected PropertyMapsPage ColumnMapsPage { get; private set; }

        /// <summary>
        /// The container that contains the 
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> DataGridPresentersComponentContainer { get; private set; }

        /// <summary>
        /// The data grid presenter maps page
        /// </summary>
        protected DataGridPresenterMapsPage DataGridPresenterMapsPage { get; private set; }

        /// <summary>
        /// The container that contains the 
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> CalendarPresentersComponentContainer { get; private set; }

        /// <summary>
        /// The container that contains the 
        /// </summary>
        protected StackPanelCollapsibleVerticalMenu<UIElement> DashboardPresentersComponentContainer { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="queryMap">The query map</param>
        public PropertyMapsAndPresentersPage(QueryMap queryMap) : base()
        {
            QueryMap = queryMap ?? throw new ArgumentNullException(nameof(queryMap));

            CreateGUI();
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
            ContentStackPanel = new SeparatedStackPanelItemsControl() 
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(LargeUniformMargin),
            };

            // Wrap it in a scroll viewer
            ContentScrollViewer = ControlsFactory.WrapInScrollViewer(ContentStackPanel);

            // Return the scroll viewer
            return ContentScrollViewer;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates and adds the required GUI elements
        /// </summary>
        private void CreateGUI()
        {
            // Create the column maps component container
            ColumnMapsPageContainer = new StackPanelCollapsibleVerticalMenu<UIElement>()
            {
                IsOpen = true,
                Text = "Column configurations",
            };

            // Create the column maps page
            ColumnMapsPage = new PropertyMapsPage(QueryMap) { AllowVerticalScroll = false };

            // Add it to the container
            ColumnMapsPageContainer.Add(ColumnMapsPage);

            // Add it to the stack panel
            ContentStackPanel.Add(ColumnMapsPageContainer);

            // Create the data grid presenters component container
            DataGridPresentersComponentContainer = new StackPanelCollapsibleVerticalMenu<UIElement>()
            {
                IsOpen = true,
                Text = "Data grid presenters"
            };

            // Create the data grid presenter maps page
            DataGridPresenterMapsPage = new DataGridPresenterMapsPage(QueryMap) { AllowVerticalScroll = false };

            // Add it to the container
            DataGridPresentersComponentContainer.Add(DataGridPresenterMapsPage);

            // Add it to the stack panel
            ContentStackPanel.Add(DataGridPresentersComponentContainer);
        }

        #endregion
    }
}
