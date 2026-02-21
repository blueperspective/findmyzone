using findmyzone.Win;
using System.IO;

namespace findmyzone.Core
{
    public class CoreSettings : ICoreSettings
    {
        private string downloadDirectory = string.Empty;

        public string DownloadDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(downloadDirectory))
                {
                    downloadDirectory = Path.Combine(KnownFolders.GetPath(KnownFolder.Downloads), "findmyzone");
                }

                return downloadDirectory;
            }

            set => downloadDirectory = value;
        }
    }
}
