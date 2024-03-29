using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using findmyzone.Core;
using findmyzone.Geo;
using findmyzone.IO;
using findmyzone.Model;
using findmyzoneui.Json;
using findmyzoneui.Services;
using findmyzoneui.ViewModels;
using findmyzoneui.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using System;

namespace findmyzoneui
{
    public class App : Application
    {
        public override void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            try
            {
                IServiceCollection serviceCollection = new ServiceCollection();

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var mainWindow = new MainWindow();
                    desktop.MainWindow = mainWindow;

                    var downloadActions = new DownloadActions();

                    serviceCollection.AddSingleton(mainWindow);
                    serviceCollection.AddSingleton(downloadActions);
                    serviceCollection.AddSingleton<IUiService, UiService>();
                    serviceCollection.AddSingleton<IZoneFinder, ZoneFinder>();
                    serviceCollection.AddSingleton<IRepository, Repository>();
                    serviceCollection.AddSingleton<IFeatureCollectionReader, FeatureCollectionReader>();
                    serviceCollection.AddSingleton<IFindMyZoneDownloader, FindMyZoneDownloader>();
                    serviceCollection.AddSingleton<IGunziper, Gunziper>();
                    serviceCollection.AddSingleton<IDownloader, Downloader>();
                    serviceCollection.AddSingleton<ICoreSettings, CoreSettings>();
                    serviceCollection.AddSingleton<MainWindowViewModel>();
                    serviceCollection.AddSingleton<SettingsVM>();

                    var notificationManager = new WindowNotificationManager(mainWindow)
                    {
                        Position = NotificationPosition.TopRight,
                        MaxItems = 3
                    };
                    serviceCollection.AddSingleton<IManagedNotificationManager>(notificationManager);

                    var serviceProvider = serviceCollection.BuildServiceProvider();

                    var contractResolve = new ServiceProviderResolver(serviceProvider);

                    // Create the AutoSuspendHelper.
                    var suspension = new AutoSuspendHelper(ApplicationLifetime);
                    RxApp.SuspensionHost.CreateNewAppState = () => serviceProvider.GetRequiredService<MainWindowViewModel>();
                    RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver("appstate.json", contractResolve));
                    suspension.OnFrameworkInitializationCompleted();

                    // Load the saved view model state.
                    var state = RxApp.SuspensionHost.GetAppState<MainWindowViewModel>();

                    desktop.MainWindow.DataContext = state;
                    desktop.MainWindow.Position = new PixelPoint(0, 0);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error during initialization");
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
