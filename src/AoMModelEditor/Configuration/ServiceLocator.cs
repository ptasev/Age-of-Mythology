using AoMModelEditor.Dialogs;
using AoMModelEditor.Models;
using AoMModelEditor.Settings;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Serilog;
using System;
using System.Reflection;

namespace AoMModelEditor.Configuration
{
    public class ServiceLocator
    {
        private static ServiceProvider? _rootServiceProvider;

        public static void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(lb =>
            {
                lb.AddSerilog(dispose: true);
            });

            // Use MS DI for ReactiveUI dependency resolution.
            // After we call the method below, Locator.Current and
            // Locator.CurrentMutable start using the locator.
            serviceCollection.UseMicrosoftDependencyResolver();

            // These .InitializeX() methods will add ReactiveUI platform 
            // registrations to your container. They MUST be present if
            // you *override* the default Locator.
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();

            Locator.CurrentMutable.UseSerilogFullLogger();

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            serviceCollection.AddSingleton<FileDialogService>();
            serviceCollection.AddSingleton<AppSettings>();

            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<ModelsViewModel>();

            _rootServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
