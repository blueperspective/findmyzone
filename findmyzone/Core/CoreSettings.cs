using System;
using System.IO;

namespace findmyzone.Core
{
    public class CoreSettings : ICoreSettings
    {
        private const string AppDirectory = "findmyzone";

        private string downloadDirectory = string.Empty;

        public string DownloadDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(downloadDirectory))
                {
                    downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify), AppDirectory);

                    if (!Directory.Exists(downloadDirectory))
                    {
                        Directory.CreateDirectory(downloadDirectory);
                    }
                }

                return downloadDirectory;
            }

            set => downloadDirectory = value;
        }
    }
}
