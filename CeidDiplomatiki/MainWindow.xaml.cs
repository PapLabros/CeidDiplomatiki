using Atom.Windows.Controls;

using System.IO;
using System.Linq;
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
            UpdateLibraries();

            InitializeComponent();

            // Set the view model
            DataContext = new WindowViewModel(this)
            {
                Title = "Ceid Diplomatiki",
                IsMain = true
            };

            Content = new CeidDiplomatikiMainApplicationPage();
        }

        private void UpdateLibraries()
        {
            // Get the files of the source directory
            var sourceFiles = Directory.EnumerateFiles(@"C:\Users\PapLabros\Desktop\Projects\Atom\TestRange\bin\Debug\netcoreapp3.1").Where(x => x.EndsWith(".dll") || x.EndsWith(".xml")).ToList();

            // Get the files of the destination directory
            var destinationFiles = Directory.EnumerateFiles(@"C:\Users\PapLabros\Desktop\Projects\CeidDiplomatiki").Where(x => x.EndsWith(".dll") || x.EndsWith(".xml")).ToList();

            // For every destination file...
            foreach (var destinationFile in destinationFiles)
            {
                // Get the destination file name
                var destinationFileName = Path.GetFileName(destinationFile);

                // Get the source file
                var sourceFile = sourceFiles.First(x => Path.GetFileName(x) == destinationFileName);

                // Copy the file
                File.Copy(sourceFile, destinationFile, true);
            }
        }
    }
}
