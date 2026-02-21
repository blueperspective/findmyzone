using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using findmyzone.IO;
using System;

namespace findmyzoneui
{
    public class AvaloniaReporter : IReporter
    {
        private readonly INotificationManager notificationManager;

        public AvaloniaReporter(Window host)
        {
            notificationManager = new WindowNotificationManager(host)
            {
                Position = NotificationPosition.TopRight,
                MaxItems = 3
            };
        }

        public void Error(string text, params string[] args)
        {
            notificationManager.Show(new Notification("Error", string.Format(text, args), NotificationType.Error));
        }

        public void Info(string text, params string[] args)
        {
            notificationManager.Show(new Notification("Info", string.Format(text, args), NotificationType.Information));
        }

        public void OpEndError()
        {
        }

        public void OpEndSuccess()
        {            
        }

        public void StartOp(string text, params string[] args)
        {
            notificationManager.Show(new Notification("Info", string.Format(text, args), NotificationType.Information));
        }
    }
}
