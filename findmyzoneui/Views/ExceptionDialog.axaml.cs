using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace findmyzoneui.Views
{
    public class ExceptionDialog : Window
    {
        public ExceptionDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
        public ExceptionDialog(string title, string message, Exception e)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var txtTitle = this.FindControl<TextBlock>("title");
            txtTitle.Text = title;

            var txtMessage = this.FindControl<TextBox>("message");
            txtMessage.Text = message;

            var txtStackTrace = this.FindControl<TextBox>("stacktrace");
            txtStackTrace.Text = e.ToString();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Copy_Click(object sender, RoutedEventArgs e)
        {
            var txtMessage = this.FindControl<TextBox>("stacktrace");
            Application.Current?.Clipboard?.SetTextAsync(txtMessage.Text);
        }
    }
}
