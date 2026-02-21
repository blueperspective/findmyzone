using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using findmyzoneui.ViewModels;
using ReactiveUI;
using System;

namespace findmyzoneui.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.WhenActivated(disposable => { });
        }

        private void Window_Activated(object sender, EventArgs args)
        {
            _ = (this.DataContext as MainWindowViewModel)?.LoadRepository();
        }
    }
}
