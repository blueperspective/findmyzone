using findmyzone.Geo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace findmyzone.IO
{
    class ReporterOutput : IOutput
    {
        private readonly IReporter reporter;

        public ReporterOutput(IReporter reporter)
        {
            this.reporter = reporter;
        }

        public void Render(IEnumerable<ZoneFinderResult> results)
        {
            foreach (var result in results)
            {
                reporter.Info($"\nParcelle {result.Feature.Attributes["id"]}, " +
                    $"prefixe: {result.Feature.Attributes["prefixe"]}, " +
                    $"section: {result.Feature.Attributes["section"]}, " +
                    $"numero: {result.Feature.Attributes["numero"]}");
                reporter.Info($"  position géographique: {result.Feature.Geometry.Centroid.Y} {result.Feature.Geometry.Centroid.X}");
                reporter.Info($"  contenance: {result.Feature.Attributes["contenance"]} m2");
                reporter.Info($"  surface calculée: {Math.Floor(result.ProjZoneGeometry.Area)} m2");
                reporter.Info($"  {result.ProjBuildingGeometries.Count} batiments, surface. calculée: {Math.Floor(result.ProjBuildingGeometries.Sum(x => x.Area))} m2");

                if (result.ProjBuildingGeometries.Any())
                {
                    foreach (var building in result.ProjBuildingGeometries)
                    {
                        reporter.Info($"    - {Math.Floor(building.Area)} m2");
                    }
                }
            }
        }
    }
}
