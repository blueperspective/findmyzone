using CommandLine;
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
using findmyzone.Core;

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

            [Option('o', "output", HelpText = "Page HTML")]
            public string HtmlOutput { get; set; }

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

                    var core = new FindMyZoneCore(reporter, options.Directory);
                    var results = await core.Find(
                        options.InseeCodes, 
                        options.ZipCodes, 
                        options.Cities, 
                        options.UseComputedArea, 
                        options.MinLotArea,
                        options.MaxLotArea, 
                        options.MinBuildingArea, 
                        options.MaxBuildingArea, 
                        options.IgnoreBuildings);

                    IOutput output;

                    if (!string.IsNullOrEmpty(options.HtmlOutput))
                    {
                        output = new HtmlOutput(options.HtmlOutput);
                    }
                    else
                    {
                        output = new ReporterOutput(reporter);
                    }

                    output.Render(results);
                },
                errors => Task.FromResult(0)
            );
        }
    }
}
