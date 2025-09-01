using Avalonia.ReactiveUI;
using findmyzoneui.ViewModels;
using ReactiveUI;

namespace findmyzoneui.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposable => { });
    }
}
