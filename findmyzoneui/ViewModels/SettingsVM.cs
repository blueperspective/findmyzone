using findmyzone.Core;
using ReactiveUI;
using System.Runtime.Serialization;

namespace findmyzoneui.ViewModels
{
    [DataContract]
    public class SettingsVM : ViewModelBase
    {
        private readonly ICoreSettings coreSettings;

        public SettingsVM(ICoreSettings coreSettings)
        {
            this.coreSettings = coreSettings;
        }


        [DataMember]
        public string DownloadDirectory
        {
            get => coreSettings.DownloadDirectory;
            set
            {
                if (coreSettings.DownloadDirectory != value)
                {
                    coreSettings.DownloadDirectory = value;
                    this.RaisePropertyChanged(nameof(DownloadDirectory));
                }
            }
        }
    }
}
