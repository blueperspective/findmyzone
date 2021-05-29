using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using findmyzoneui.Services;
using findmyzoneui.ViewModels;
using findmyzoneui.Views;

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
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = new MainWindowViewModel(desktop.MainWindow, new UiService(desktop.MainWindow));
                desktop.MainWindow.Position = new PixelPoint(0, 0);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
