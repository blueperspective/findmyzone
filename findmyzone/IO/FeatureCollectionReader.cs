using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System.IO;

namespace findmyzone.IO
{
    public class FeatureCollectionReader : IFeatureCollectionReader
    {
        public FeatureCollection Read(string filepath)
        {
            FeatureCollection featureCollection = null;

            var serializer = new JsonSerializer();
            using (var s = File.Open(filepath, FileMode.Open))
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                var geoJsonReader = new GeoJsonReader();
                featureCollection = geoJsonReader.Read<FeatureCollection>(reader);
            }

            return featureCollection;
        }
    }
}
