using NetTopologySuite.Features;
using System.Collections.Generic;

namespace findmyzone.Geo
{
    interface IZoneFinder
    {
        IEnumerable<ZoneFinderResult> FindZone(string codeInsee, uint minLotSurface, uint maxLotSurface, uint minBuildingSurface, uint maxBuildingSurface, bool ignoreBuilding);
    }
}