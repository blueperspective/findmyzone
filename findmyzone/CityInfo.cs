using CsvHelper.Configuration;

namespace findmyzone
{
    public class CityInfo
    {
        public string InseeCode { get; set; }
        public string ZipCode { get; set; }
        public string Name { get; set; }
    }

    public sealed class CityInfoMap : ClassMap<CityInfo>
    {
        public CityInfoMap()
        {
            Map(m => m.InseeCode).Name("Code INSEE");
            Map(m => m.ZipCode).Name("Code Postal");
            Map(m => m.Name).Name("Commune");
        }
    }
}
