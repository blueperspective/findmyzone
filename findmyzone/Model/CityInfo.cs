using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace findmyzone.Model;

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

public class ToStringArrayConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text)) return System.Array.Empty<string>();
        return text.Split('/');
    }

    public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value == null)
            return string.Empty;

        return string.Join("/", (string[])value);
    }
}
