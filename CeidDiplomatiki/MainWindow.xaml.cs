using Atom.Windows.Controls;

using System.Windows;

namespace CeidDiplomatiki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the view model
            DataContext = new WindowViewModel(this)
            {
                Title = "CeidDiplomatiki",
                IsMain = true
            };

            Content = new CeidDiplomatikiMainApplicationPage();
        }
    }
}
