using Atom.Core;
using Atom.PlugIns;
using Atom.Windows;
using Atom.Windows.Controls;
using Atom.Windows.Controls.TabControl;
using Atom.Windows.PlugIns;
using Atom.Windows.PlugIns.Billing;
using Atom.Windows.PlugIns.Communications;
using Atom.Windows.PlugIns.Data;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CeidDiplomatiki
{
    /// <summary>
    /// The app's true entry point used for defining a custom main method
    /// </summary>
    public class EntryPoint
    {
        [STAThread]
        public static async Task Main()
        {
            // Initialize the DI
            var construction = Framework.Construct<DefaultFrameworkConstruction>()
                     .AddControlManager()
                     .AddTaskManager()
                     .AddDialogManager()
                     .AddDefaultDirectoriesProvider(ApplicationConstants.omniView)
                     .AddCeidDiplomatikiManager();
         
            // Build the framework using the injected services
            Framework.Construction.Build();

            // Set up
            await SetUpAsync();

            // We need to create a new thread to run our application because the STAThread attribute of the main
            // gets ignored when compiled.
            // Issue: https://github.com/dotnet/roslyn/issues/22112
            var thread = new Thread(() =>
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            });

            // Set the thread department to STA
            thread.SetApartmentState(ApartmentState.STA);

            // Start the application
            thread.Start();
        }

        /// <summary>
        /// Set ups the application
        /// </summary>
        private static async Task SetUpAsync()
        {
            // Initialize the manager
            await CeidDiplomatikiDI.GetCeidDiplomatikiManager.InitializeAsync();

            // Get the registered plug ins
            foreach (var plugIn in DI.GetServices<IPlugIn>())
                // Handle the application initialization
                await plugIn.OnApplicationStartedAsync();
        }
    }
}
