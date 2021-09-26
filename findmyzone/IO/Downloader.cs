using findmyzone.Resources;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace findmyzone.IO
{
    public class Downloader : IDownloader
    {
        public const string GzipExt = ".gz";
        public const string TplBuildingUrl = "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/communes/{0}/{1}/cadastre-{1}-batiments.json.gz";
        public const string TplBuildingFilename = "cadastre-{0}-batiments.json";
        public const string TplZoneUrl = "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/communes/{0}/{1}/cadastre-{1}-parcelles.json.gz";
        public const string TplZoneFilename = "cadastre-{0}-parcelles.json";

        private readonly IReporter reporter;
        private readonly IGunziper gunziper;

        public Downloader(IReporter reporter, IGunziper gunziper)
        {
            this.reporter = reporter;
            this.gunziper = gunziper;
        }

        public string FilesDirectory { get; set; }

        public async Task<string> DownloadZone(string inseeCode)
        {
            if (string.IsNullOrEmpty(inseeCode))
                throw new ArgumentNullException("code");

            if (inseeCode.Length != 5)
                throw new ArgumentException(string.Format(Messages.CodeShouldHave5Digits, inseeCode.Length, inseeCode));

            string zoneUrl = string.Format(TplZoneUrl, inseeCode.Substring(0, 2), inseeCode);
            string zoneFile = Path.Combine(FilesDirectory, string.Format(TplZoneFilename, inseeCode));
            string zoneFileGz = zoneFile + GzipExt;

            await DownloadIfNeeded(zoneUrl, zoneFile, zoneFileGz);

            return zoneFile;
        }

        public async Task<string> DownloadBuilding(string inseeCode)
        {
            if (string.IsNullOrEmpty(inseeCode))
                throw new ArgumentNullException("code");

            if (inseeCode.Length != 5)
                throw new ArgumentException(string.Format(Messages.CodeShouldHave5Digits, inseeCode.Length, inseeCode));

            string buildingUrl = string.Format(TplBuildingUrl, inseeCode.Substring(0, 2), inseeCode);
            string buildingFile = Path.Combine(FilesDirectory, string.Format(TplBuildingFilename, inseeCode));
            string buildingFileGz = buildingFile + GzipExt;

            await DownloadIfNeeded(buildingUrl, buildingFile, buildingFileGz);

            return buildingFile;
        }

        private async Task DownloadIfNeeded(string url, string file, string fileGz)
        {
            // check if filename exists in json

            if (File.Exists(file))
            {
                reporter?.Info(Messages.AlreadyDownloaded, new FileInfo(file).Name);
                return;
            }

            // check if filename exists in json.gz

            if (File.Exists(fileGz))
            {
                gunziper.UngzipFile(fileGz, FilesDirectory);
                return;
            }

            // download and ungzip

            using (var webClient = new WebClient())
            {
                var realFile = await webClient.DownloadFileToDirectory(url, FilesDirectory, new FileInfo(fileGz).Name);
                gunziper.UngzipFile(realFile, FilesDirectory);
            }
        }
    }
}
