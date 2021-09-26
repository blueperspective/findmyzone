using System.Collections.Generic;
using System.Threading.Tasks;

namespace findmyzone.Geo
{
    public interface IZoneFinder
    {
        IAsyncEnumerable<ZoneFinderResult> FindZone(
            string codeInsee, 
            uint minLotArea, 
            uint maxLotArea,
            bool useComputedArea,
            uint minBuildingArea,
            uint maxBuildingArea,
            bool ignoreBuilding);
    }
}