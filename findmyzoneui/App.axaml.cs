using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using findmyzoneui.Json;
using findmyzoneui.Services;
using findmyzoneui.ViewModels;
using findmyzoneui.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace findmyzoneui
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;

                serviceCollection.AddSingleton(mainWindow);
                serviceCollection.AddSingleton<MainWindowViewModel>();
                serviceCollection.AddSingleton<IUiService, UiService>();
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var contractResolve = new ServiceProviderResolver(serviceProvider);

                // Create the AutoSuspendHelper.
                var suspension = new AutoSuspendHelper(ApplicationLifetime);
                RxApp.SuspensionHost.CreateNewAppState = () => new MainWindowViewModel(serviceProvider.GetRequiredService<MainWindow>(), serviceProvider.GetRequiredService<IUiService>());
                RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver("appstate.json", contractResolve));
                suspension.OnFrameworkInitializationCompleted();

                // Load the saved view model state.
                var state = RxApp.SuspensionHost.GetAppState<MainWindowViewModel>();

                desktop.MainWindow.DataContext = state;
                desktop.MainWindow.Position = new PixelPoint(0, 0);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
