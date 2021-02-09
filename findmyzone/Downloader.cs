using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace findmyzone
{
    class Downloader
    {
        public const string GzipExt = ".gz";
        public const string TplBuildingUrl = "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/communes/{0}/{1}/cadastre-{1}-batiments.json.gz";
        public const string TplBuildingFilename = "cadastre-{0}-batiments.json";
        public const string TplZoneUrl = "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/communes/{0}/{1}/cadastre-{1}-parcelles.json.gz";
        public const string TplZoneFilename = "cadastre-{0}-parcelles.json";

        public string FilesDirectory { get; set; } = @"f:\Users\endymion\Downloads";

        public async Task Download(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("code");

            if (code.Length != 5)
                throw new ArgumentException(string.Format(Messages.CodeShouldHave5Digits, code.Length, code));

            string buildingUrl = string.Format(TplBuildingUrl, code.Substring(0, 2), code);
            string buildingFile = Path.Combine(FilesDirectory, string.Format(TplBuildingFilename, code));
            string buildingFileGz = buildingFile + GzipExt;

            await DownloadIfNeeded(buildingUrl, buildingFile, buildingFileGz);

            string zoneUrl = string.Format(TplZoneUrl, code.Substring(0, 2), code);
            string zoneFilename = Path.Combine(FilesDirectory, string.Format(TplZoneFilename, code));
            string zoneFilenameGz = zoneFilename + GzipExt;

            await DownloadIfNeeded(zoneUrl, zoneFilename, zoneFilenameGz);
        }

        private async Task DownloadIfNeeded(string url, string file, string fileGz)
        {
            // check if filename exists in json

            if (File.Exists(file))
            {
                return;
            }

            // check if filename exists in json.gz

            if (File.Exists(fileGz))
            {
                UngzipFile(fileGz);
                return;
            }

            // download and ungzip

            using (var webClient = new WebClient())
            {
                var realFile = await webClient.DownloadFileToDirectory(url, FilesDirectory, new FileInfo(fileGz).Name);
                UngzipFile(realFile);
            }
        }

        private void UngzipFile(string gzipFile)
        {
            // Use a 4K buffer. Any larger is a waste.    
            byte[] dataBuffer = new byte[4096];

            using (System.IO.Stream fs = new FileStream(gzipFile, FileMode.Open, FileAccess.Read))
            {
                using (GZipInputStream gzipStream = new GZipInputStream(fs))
                {
                    // Change this to your needs
                    string fnOut = Path.Combine(FilesDirectory, Path.GetFileNameWithoutExtension(gzipFile));

                    using (FileStream fsOut = File.Create(fnOut))
                    {
                        StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                    }
                }
            }
        }
    }
}
