using NetTopologySuite.Features;

namespace findmyzone.IO
{
    public interface IFeatureCollectionReader
    {
        FeatureCollection Read(string filepath);
    }
}