using NetTopologySuite.Features;
using findmyzone.Model;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Collections.Generic;
using System.Linq;

namespace findmyzone.Geo
{
    class ZoneFinder : IZoneFinder
    {
        public const string AreaAttribute = "contenance";

        private readonly IRepository repository;

        private MathTransform mt = null;

        public ZoneFinder(IRepository repository)
        {
            this.repository = repository;

            CreateMathTransform();
        }

        private void CreateMathTransform()
        {
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var utm17n_fromWKT = (ProjectedCoordinateSystem)ProjNet.IO.CoordinateSystems.CoordinateSystemWktReader.Parse(Wtk.Lambert93);
            var ctFactory = new CoordinateTransformationFactory();
            var ct = ctFactory.CreateFromCoordinateSystems(wgs84, utm17n_fromWKT);
            mt = ct.MathTransform;
        }

        public IEnumerable<ZoneFinderResult> FindZone(
            string codeInsee,
            uint minLotArea,
            uint maxLotArea,
            bool useComputedArea,
            uint minBuildingArea,
            uint maxBuildingArea,
            bool ignoreBuilding)
        {
            var zoneFeatures = repository.GetZoneFeatures(codeInsee);
            var buildingFeatures = repository.GetBuildingFeatures(codeInsee);
            foreach (var zoneFeature in zoneFeatures)
            {
                var projGeo = GeoExtensions.Transform(zoneFeature.Geometry, mt);

                long advertisedArea = 0;
                if (zoneFeature.Attributes.Exists(AreaAttribute))
                {
                    advertisedArea = (long)zoneFeature.Attributes[AreaAttribute];
                }

                if (advertisedArea == 0)
                {
                    useComputedArea = true;
                }

                if ((useComputedArea && projGeo.Area >= minLotArea && projGeo.Area < maxLotArea)
                    || (!useComputedArea && advertisedArea >= minLotArea && advertisedArea < maxLotArea))
                {
                    var result = new ZoneFinderResult { Feature = zoneFeature, ProjZoneGeometry = projGeo };

                    foreach (var buildingFeature in buildingFeatures)
                    {
                        if (buildingFeature.Geometry.CoveredBy(zoneFeature.Geometry))
                        {
                            var projGeoBuilding = GeoExtensions.Transform(buildingFeature.Geometry, mt);
                            result.ProjBuildingGeometries.Add(projGeoBuilding);
                        }
                    }

                    if (result.ProjBuildingGeometries.Any())
                    {
                        var totalArea = result.ProjBuildingGeometries.Sum(x => x.Area);

                        if ((minBuildingArea > 0 && maxBuildingArea > 0
                                && minBuildingArea <= totalArea && totalArea <= maxBuildingArea)
                            || (minBuildingArea > 0 && maxBuildingArea == 0 && totalArea >= minBuildingArea)
                            || (minBuildingArea == 0 && maxBuildingArea > 0 && totalArea <= maxBuildingArea)
                            || (minBuildingArea == 0 && maxBuildingArea == 0))
                        {
                            yield return result;
                        }
                    }
                    else if (ignoreBuilding)
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}
