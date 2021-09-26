using CsvHelper;
using CsvHelper.Configuration;
using findmyzone.Model;
using System.Globalization;
using System.IO;

namespace findmyzone.IO
{
    public class CityInfoReader
    {
        private readonly StreamReader streamReader;

        public CityInfoReader(StreamReader streamReader)
        {
            this.streamReader = streamReader;
        }

        public void Fill(IRepository repository)
        {
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture);
            cfg.Delimiter = ";";

            using (var csv = new CsvReader(streamReader, cfg))
            {
                repository.Cities.AddRange(csv.GetRecords<CityInfo>());
            }
        }
    }
}
