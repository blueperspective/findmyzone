﻿using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using findmyzone.Cli;
using findmyzone.Resources;
using findmyzone.Win;
using findmyzone.Model;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using findmyzone.IO;
using findmyzone.Geo;
using System;

namespace findmyzone
{
    class Program
    {
        public class Options
        {
            [Option('c', "codepostal", Required = false, HelpText = "Liste des codes postaux")]
            public IList<string> ZipCodes { get; set; }

            [Option('v', "ville", Required = false, HelpText = "Liste des villes")]
            public IList<string> Cities { get; set; }

            [Option('i', "codeinsee", Required = false, HelpText = "Liste des codes INSEE")]
            public IList<string> InseeCodes { get; set; }

            [Option("pmin", Required = false, HelpText = "Surface minimale de parcelle (en m2)")]
            public uint MinLotArea { get; set; }

            [Option("pmax", Required = false, HelpText = "Area minimale de parcelle (en m2)")]
            public uint MaxLotArea { get; set; }

            [Option("sc", Default = false, HelpText = "Utiliser la surface calculée (sinon, utiliser la surface du fichier cadastral)")]
            public bool UseComputedArea { get; set; }

            [Option("bmin", Required = false, HelpText = "Area minimale de batiment (en m2)")]
            public uint MinBuildingArea { get; set; }

            [Option("bmax", Required = false, HelpText = "Area maximale de batiment (en m2)")]
            public uint MaxBuildingArea { get; set; }

            [Option("dl",
                    Default = true, HelpText = "Télécharger automatiquement les fichiers manquants")]
            public bool Download { get; set; }

            [Option('d', "dir", HelpText = "Dossier de téléchargement")]
            public string Directory { get; set; }

            [Option("nobuilding", Default = false, HelpText = "Ignorer les batiments")]
            public bool IgnoreBuildings { get; set; }
        }

        static async Task Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            await result.MapResult(async
                options =>
                {
                    var reporter = new ConsoleReporter();

                    if (string.IsNullOrEmpty(options.Directory))
                    {
                        options.Directory = KnownFolders.GetPath(KnownFolder.Downloads);
                        reporter.Info(Messages.DownloadDir, options.Directory);
                    }

                    // force culture for google map coordinates
                    CultureInfo.CurrentCulture = new CultureInfo("en-US");

                    var findInCities = new List<CityInfo>();

                    var cfg = new CsvConfiguration(CultureInfo.InvariantCulture, delimiter: ";");
                    IList<CityInfo> citiesInfo;

                    using (var reader = new StreamReader(Path.Combine(options.Directory, "correspondance-code-insee-code-postal.csv")))
                    using (var csv = new CsvReader(reader, cfg))
                    {
                        csv.Context.RegisterClassMap<CityInfoMap>();
                        citiesInfo = csv.GetRecords<CityInfo>().ToList();

                        foreach (var inseeCode in options.InseeCodes)
                        {
                            var city = citiesInfo.FirstOrDefault(x => x.InseeCode == inseeCode);

                            if (city == null)
                            {
                                reporter.Error(Messages.WrongZipCode);
                                continue;
                            }

                            findInCities.Add(city);
                        }

                        foreach (var zipCode in options.ZipCodes)
                        {
                            var cities = citiesInfo.Where(x => x.ZipCode == zipCode);

                            if (!cities.Any())
                            {
                                reporter.Error(Messages.WrongZipCode);
                                continue;
                            }

                            foreach (var city in cities)
                            {
                                findInCities.Add(city);
                            }
                        }

                        foreach (var cityName in options.Cities.Select(x => x.ToUpperInvariant()))
                        {
                            var cities = citiesInfo.Where(x => x.Name.ToUpperInvariant() == cityName).ToList();
                            
                            if (cities.Count() > 1)
                            {
                                reporter.Info($"{cities.Count()} villes correspondent, utiliser le code postal ou code insee:");
                                foreach (var cityInfo in cities)
                                {
                                    reporter.Info($"- {cityInfo.ZipCode} {cityInfo.Name} (code INSEE: {cityInfo.InseeCode})");
                                }
                                return;
                            }
                            else
                            {
                                var city = cities.FirstOrDefault();

                                if (city == null)
                                {
                                    reporter.Error(Messages.WrongCityName, cityName);
                                    continue;
                                }

                                findInCities.Add(city);
                            }
                        }
                    }

                    if (!findInCities.Any())
                    {
                        reporter.Error(Messages.NoInseeCodes);
                        return;
                    }

                    var repository = new Repository(new FeatureCollectionReader());
                    var downloader = new Downloader(reporter, new Gunziper(reporter), repository, options.Directory) ;
                    var finder = new ZoneFinder(repository);

                    foreach (var cityInfo in findInCities)
                    {
                        reporter.Info($"\n# Recherche sur {cityInfo.ZipCode} {cityInfo.Name} (code INSEE: {cityInfo.InseeCode})\n");

                        await downloader.Download(cityInfo.InseeCode);

                        var results = finder.FindZone(
                            cityInfo.InseeCode, 
                            options.MinLotArea,
                            options.MaxLotArea, 
                            options.UseComputedArea,
                            options.MinBuildingArea,
                            options.MaxBuildingArea, 
                            options.IgnoreBuildings);

                        foreach (var result in results)
                        {
                            reporter.Info($"\nParcelle {result.Feature.Attributes["id"]}, " +
                                $"prefixe: {result.Feature.Attributes["prefixe"]}, " +
                                $"section: {result.Feature.Attributes["section"]}, " +
                                $"numero: {result.Feature.Attributes["numero"]}");
                            reporter.Info($"  position géographique: {result.Feature.Geometry.Centroid.Y} {result.Feature.Geometry.Centroid.X}");
                            reporter.Info($"  contenance: {result.Feature.Attributes["contenance"]} m2");
                            reporter.Info($"  surface calculée: {Math.Floor(result.ProjZoneGeometry.Area)} m2");
                            reporter.Info($"  {result.ProjBuildingGeometries.Count} batiments, surface. calculée: {Math.Floor(result.ProjBuildingGeometries.Sum(x => x.Area))} m2");

                            if (result.ProjBuildingGeometries.Any())
                            {
                                foreach (var building in result.ProjBuildingGeometries)
                                {
                                    reporter.Info($"    - {Math.Floor(building.Area)} m2");
                                }
                            }
                        }
                    }
                },
                errors => Task.FromResult(0)
            );
        }
    }
}
