using System.Collections.Generic;

namespace findmyzone.Geo
{
    interface IZoneFinder
    {
        IEnumerable<ZoneFinderResult> FindZone(
            string codeInsee, 
            uint minLotArea, 
            uint maxLotArea,
            bool useComputedArea,
            uint minBuildingArea,
            uint maxBuildingArea,
            bool ignoreBuilding);
    }
}