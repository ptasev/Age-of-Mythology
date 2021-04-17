using AoMModelEditor.Configuration;
using AoMModelEditor.Settings;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Windows;

namespace AoMModelEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ServiceCollection _serviceCollection;

        static App()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/ame-log-.txt");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _serviceCollection = new ServiceCollection();
            ServiceLocator.Configure(_serviceCollection);
        }

        public App()
        {
            Log.Logger.Information($"Starting app {AoMModelEditor.Properties.Resources.AppTitleLong}.");
            var appSettings = ServiceLocator.Current.GetService<AppSettings>();
            appSettings.Read();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create main application window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            mainWindow.ViewModel?.HandleStartupArgs(e.Args);
        }
    }
}
