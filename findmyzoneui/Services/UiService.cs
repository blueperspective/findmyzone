using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using findmyzoneui.Views;
using System;
using System.Threading.Tasks;

namespace findmyzoneui.Services
{
    public class UiService : IUiService
    {
        private readonly Window parent;

        public UiService(MainWindow parent)
        {
            this.parent = parent;
        }

        public async Task<ButtonResult> Ask(string title, string message)
        {
            var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ShowInCenter = true,
                ButtonDefinitions = ButtonEnum.YesNo,
                ContentTitle = title,
                ContentMessage = message,
                Icon = Icon.Plus,
            });

            var res = await msBoxStandardWindow.Show();

            return res;
        }

        public async Task ShowMessage(string title, string message)
        {
            var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentTitle = title,
                ContentMessage = message
            });

            await msBoxStandardWindow.Show();
        }

        public async Task ShowMessage(string title, string message, Icon icon, ButtonEnum buttonEnum)
        {
            var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentTitle = title,
                ContentMessage = message,
                Icon = icon,
                ButtonDefinitions = buttonEnum
            });

            await msBoxStandardWindow.Show();
        }

        public async Task ShowException(string title, string message, Exception e)
        {
            var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            if (desktop == null)
            {
                throw new NotImplementedException();
            }

            var window = new ExceptionDialog(title, message, e)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog(desktop.MainWindow);
        }
    }
}
