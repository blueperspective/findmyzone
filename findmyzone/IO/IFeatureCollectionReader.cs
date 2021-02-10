using NetTopologySuite.Features;

namespace findmyzone.IO
{
    interface IFeatureCollectionReader
    {
        FeatureCollection Read(string filepath);
    }
}