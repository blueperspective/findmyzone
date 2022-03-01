using findmyzone.Core;
using findmyzone.IO;
using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace findmyzone.Model
{
    public class Repository : IRepository
    {
        private readonly ICoreSettings coreSettings;

        private readonly IFindMyZoneDownloader downloader;

        private readonly IFeatureCollectionReader reader;

        private readonly IDictionary<string, RepoElement> codeElement = new Dictionary<string, RepoElement>();

        public List<CityInfo> Cities { get; } = new();

        public Repository(ICoreSettings coreSettings, IFindMyZoneDownloader downloader, IFeatureCollectionReader reader)
        {
            this.coreSettings = coreSettings;
            this.downloader = downloader;
            this.reader = reader;
        }

        public async Task<FeatureCollection> GetBuildingFeatures(string inseeCode)
        {
            if (!codeElement.TryGetValue(inseeCode, out RepoElement? repoElement))
            {
                repoElement = new RepoElement();
                codeElement[inseeCode] = repoElement;
            }

            if (string.IsNullOrEmpty(repoElement.BuildingFile))
            {
                var buildingFile = await downloader.DownloadBuilding(inseeCode);
                repoElement.BuildingFile = buildingFile;
            }

            if (repoElement.BuildingCollection == null)
            {
                repoElement.BuildingCollection = reader.Read(repoElement.BuildingFile);
            }

            return repoElement.BuildingCollection;
        }

        public async Task<FeatureCollection> GetZoneFeatures(string inseeCode)
        {
            if (!codeElement.TryGetValue(inseeCode, out RepoElement? repoElement))
            {
                repoElement = new RepoElement();
                codeElement[inseeCode] = repoElement;
            }

            if (string.IsNullOrEmpty(repoElement.ZoneFile))
            {
                var zoneFile = await downloader.DownloadZone(inseeCode);
                repoElement.ZoneFile = zoneFile;
            }

            if (repoElement.ZoneCollection == null)
            {
                repoElement.ZoneCollection = reader.Read(repoElement.ZoneFile);
            }

            return repoElement.ZoneCollection;
        }

        class RepoElement
        {
            public string? ZoneFile { get; set; }

            public FeatureCollection? ZoneCollection { get; set; }

            public string? BuildingFile { get; set; }
            public FeatureCollection? BuildingCollection { get; set; }
        }
    }
}
