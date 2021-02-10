using NetTopologySuite.Features;

namespace findmyzone.Model
{
    interface IRepository
    {
        void AddBuildingFile(string code, string buildingFile);
        void AddZoneFile(string code, string zoneFile);
        FeatureCollection GetBuildingFeatures(string code);
        FeatureCollection GetZoneFeatures(string code);
    }
}