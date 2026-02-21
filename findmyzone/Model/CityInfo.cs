using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace findmyzone.Model
{
    public class CityInfo
    {
        [Name("Code INSEE")]
        public string? InseeCode { get; set; }

        [Name("Code Postal")]
        [TypeConverter(typeof(ToStringArrayConverter))]
        public string[]? ZipCodes { get; set; }

        [Name("Commune")]
        public string? Name { get; set; }

        public override string ToString()
        {
            return $"{Name} ({InseeCode} / {(ZipCodes != null ? string.Join('-', ZipCodes) : string.Empty)})";
        }
    }

    public class ToStringArrayConverter : TypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text)) return new string[0];
            return text.Split('/');
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return string.Join("/", (string[])value);
        }
    }
}
