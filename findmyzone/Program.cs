using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

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

            [Option('c', "codepostal", Required = false, HelpText = "Liste des codes INSEE")]
            public IList<string> InseeCode { get; set; }

            [Option("pmin", Required = false, HelpText = "Surface minimale de parcelle (en m2)")]
            public uint MinLotSurface { get; set; }

            [Option("pmax", Required = false, HelpText = "Surface minimale de parcelle (en m2)")]
            public uint MaxLotSurface { get; set; }

            [Option("bmin", Required = false, HelpText = "Surface minimale de batiment (en m2)")]
            public uint MinBuildingSurface { get; set; }

            [Option("bmax", Required = false, HelpText = "Surface maximale de batiment (en m2)")]
            public uint MaxBuildingSurface { get; set; }

            [Option("Download",
                    Default = true, HelpText = "Télécharger automatiquement les fichiers manquants")]
            public bool Download { get; set; }
        }

        static async Task Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            await result.MapResult(async
                x =>
                {
                    var cfg = new CsvConfiguration(CultureInfo.InvariantCulture, delimiter: ";");
                    
                    using (var reader = new StreamReader(@"f:\Users\endymion\Downloads\correspondance-code-insee-code-postal.csv"))
                    using (var csv = new CsvReader(reader, cfg))
                    {
                        csv.Context.RegisterClassMap<CityInfoMap>();
                        var records = csv.GetRecords<CityInfo>();
                    }

                    var reporter = new ConsoleReporter();
                    var downloader = new Downloader(reporter, new Gunziper(reporter));
                    await downloader.Download("35026");
                },
                errors => Task.FromResult(0)
            );
        }
    }
}
