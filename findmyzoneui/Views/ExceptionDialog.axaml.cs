using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace findmyzoneui.Views;

public partial class ExceptionDialog : Window
{
    public ExceptionDialog()
    {
        this.InitializeComponent();
    }

    public ExceptionDialog(string title, string message, Exception e)
    {
        this.InitializeComponent();

        txtTitle.Text = title;
        txtMessage.Text = message;
        txtStackTrace.Text = e.ToString();
    }

    public void Ok_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public void Copy_Click(object sender, RoutedEventArgs e)
    {
        this.Clipboard?.SetTextAsync(txtMessage.Text);
    }
}
