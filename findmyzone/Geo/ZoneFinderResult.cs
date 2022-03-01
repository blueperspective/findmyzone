using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace findmyzone.Geo
{
    public class ZoneFinderResult
    {
        public IFeature? Feature { get; set; }
        public Geometry? ProjZoneGeometry { get; set; }
        public IList<Geometry> ProjBuildingGeometries { get; set; } = new List<Geometry>();

        public override string ToString()
        {
            try
            {
                var builder = new StringBuilder();
                builder.AppendLine($"\nParcelle {Feature?.Attributes["id"]}, " +
                        $"prefixe: {Feature?.Attributes["prefixe"]}, " +
                        $"section: {Feature?.Attributes["section"]}, " +
                        $"numero: {Feature?.Attributes["numero"]}");
                builder.AppendLine($"  position géographique: {Feature?.Geometry.Centroid.Y} {Feature?.Geometry.Centroid.X}");
                builder.AppendLine($"  contenance: {Feature?.Attributes["contenance"]} m2");
                builder.AppendLine($"  surface calculée: {(ProjZoneGeometry != null ? Math.Floor(ProjZoneGeometry.Area) : string.Empty)} m2");
                builder.AppendLine($"  {ProjBuildingGeometries.Count} batiments, surface. calculée: {Math.Floor(ProjBuildingGeometries.Sum(x => x.Area))} m2");

                if (ProjBuildingGeometries.Any())
                {
                    foreach (var building in ProjBuildingGeometries)
                    {
                        builder.AppendLine($"    - {Math.Floor(building.Area)} m2");
                    }
                }

                return builder.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
