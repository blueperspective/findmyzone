using CsvHelper;
using CsvHelper.Configuration;
using findmyzone.Geo;
using findmyzone.IO;
using findmyzone.Model;
using findmyzone.Resources;
using findmyzone.Win;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace findmyzone.Core
{
    public class FindMyZoneCore
    {
        private readonly IReporter reporter;

        private readonly string downloadDirectory;

        public  FindMyZoneCore(IReporter reporter, string downloadDirectory)
        {
            this.reporter = reporter;

            if (string.IsNullOrEmpty(downloadDirectory))
            {
                downloadDirectory = Path.Combine(KnownFolders.GetPath(KnownFolder.Downloads), "findmyzone");
                reporter?.Info(Messages.DownloadDir, downloadDirectory);
            }

            this.downloadDirectory = downloadDirectory;
        }

        public async Task<IEnumerable<ZoneFinderResult>> Find(IEnumerable<string> inseeCodes, IEnumerable<string> zipCodes, IEnumerable<string> cities, bool useComputedArea, uint minLotArea, uint maxLotArea, uint minBuildingArea, uint maxBuildingArea, bool ignoreBuildings)
        {
            // force culture for google map coordinates
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            var findInCities = new List<CityInfo>();

            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture, delimiter: ";");
            IList<CityInfo> citiesInfo;

            using (var reader = new StreamReader(Path.Combine(downloadDirectory, "correspondance-code-insee-code-postal.csv")))
            using (var csv = new CsvReader(reader, cfg))
            {
                csv.Context.RegisterClassMap<CityInfoMap>();
                citiesInfo = csv.GetRecords<CityInfo>().ToList();

                foreach (var inseeCode in inseeCodes)
                {
                    var city = citiesInfo.FirstOrDefault(x => x.InseeCode == inseeCode);

                    if (city == null)
                    {
                        reporter?.Error(Messages.WrongZipCode);
                        continue;
                    }

                    findInCities.Add(city);
                }

                foreach (var zipCode in zipCodes)
                {
                    var zipCodeCities = citiesInfo.Where(x => x.ZipCode == zipCode);

                    if (!zipCodeCities.Any())
                    {
                        reporter?.Error(Messages.WrongZipCode);
                        continue;
                    }

                    foreach (var city in zipCodeCities)
                    {
                        findInCities.Add(city);
                    }
                }

                foreach (var cityName in cities.Select(x => x.ToUpperInvariant()))
                {
                    var cityInfoList = citiesInfo.Where(x => x.Name.ToUpperInvariant() == cityName).ToList();

                    if (cityInfoList.Count() > 1)
                    {
                        reporter?.Info($"{cityInfoList.Count()} villes correspondent, utiliser le code postal ou code insee:");
                        foreach (var cityInfo in cityInfoList)
                        {
                            reporter?.Info($"- {cityInfo.ZipCode} {cityInfo.Name} (code INSEE: {cityInfo.InseeCode})");
                        }

                        return Enumerable.Empty<ZoneFinderResult>();
                    }
                    else
                    {
                        var city = cityInfoList.FirstOrDefault();

                        if (city == null)
                        {
                            reporter?.Error(Messages.WrongCityName, cityName);
                            continue;
                        }

                        findInCities.Add(city);
                    }
                }
            }

            if (!findInCities.Any())
            {
                reporter?.Error(Messages.NoInseeCodes);
                return Enumerable.Empty<ZoneFinderResult>();
            }

            var repository = new Repository(new FeatureCollectionReader());
            var downloader = new Downloader(reporter, new Gunziper(reporter), repository, downloadDirectory);
            var finder = new ZoneFinder(repository);

            IEnumerable<ZoneFinderResult> results = Enumerable.Empty<ZoneFinderResult>();

            foreach (var cityInfo in findInCities)
            {
                reporter?.Info($"\n# Recherche sur {cityInfo.ZipCode} {cityInfo.Name} (code INSEE: {cityInfo.InseeCode})\n");

                await downloader.Download(cityInfo.InseeCode);

                results = results.Concat(finder.FindZone(
                    cityInfo.InseeCode,
                    minLotArea,
                    maxLotArea,
                    useComputedArea,
                    minBuildingArea,
                    maxBuildingArea,
                    ignoreBuildings));
            }

            return results;
        }
    }
}
