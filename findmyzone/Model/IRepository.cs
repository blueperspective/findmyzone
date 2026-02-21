using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace findmyzone.Model
{
    public interface IRepository
    {
        List<CityInfo> Cities { get; }
        Task<FeatureCollection> GetBuildingFeatures(string code);
        Task<FeatureCollection> GetZoneFeatures(string code);
    }
}