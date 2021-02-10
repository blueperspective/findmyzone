using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace findmyzone.Geo
{
    class ZoneFinderResult
    {
        public IFeature Feature { get; set; }
        public Geometry ProjZoneGeometry { get; set; }
        public IList<Geometry> ProjBuildingGeometries { get; set; } = new List<Geometry>();
    }
}
