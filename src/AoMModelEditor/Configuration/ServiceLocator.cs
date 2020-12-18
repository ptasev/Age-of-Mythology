using AoMModelEditor.Services;
using AoMModelEditor.Models;
using AoMModelEditor.Settings;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Serilog;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace AoMModelEditor.Configuration
{
    public class ServiceLocator
    {
        private static readonly ConcurrentDictionary<int, ServiceLocator> _serviceLocators = new ConcurrentDictionary<int, ServiceLocator>();
        private static ServiceProvider? _rootServiceProvider;

        public static ServiceLocator Current
        {
            get
            {
                // TODO: figure out way to get unique ID to have a different scope per Window
                int currentViewId = 0;
                return _serviceLocators.GetOrAdd(currentViewId, key => new ServiceLocator());
            }
        }

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

            serviceCollection.AddSingleton<GltfImportDialogService>();
            serviceCollection.AddSingleton<FileDialogService>();
            serviceCollection.AddSingleton<AppSettings>();

            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<ModelsViewModel>();

            _rootServiceProvider = serviceCollection.BuildServiceProvider();

            // According to docs I may need to call ReactiveUI func again on built container??
            _rootServiceProvider.UseMicrosoftDependencyResolver();
        }

        private readonly IServiceScope _serviceScope;

        private ServiceLocator()
        {
            _serviceScope = _rootServiceProvider!.CreateScope();
        }

        public T GetService<T>()
            where T : notnull
        {
            return _rootServiceProvider!.GetRequiredService<T>();
        }

        public T? GetService<T>(bool isRequired)
            where T : notnull
        {
            if (isRequired)
            {
                return _serviceScope.ServiceProvider.GetRequiredService<T>();
            }
            return _serviceScope.ServiceProvider.GetService<T>();
        }
    }
}
