using findmyzone.Geo;
using System;
using System.Collections.Generic;
using System.IO;

namespace findmyzone.IO
{
    class HtmlOutput : IOutput
    {
        private readonly string filepath;

        public HtmlOutput(string filepath)
        {
            this.filepath = filepath;
        }

        public void Render(IEnumerable<ZoneFinderResult> results)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine("<html><head></head><body><ul>");
                foreach (var result in results)
                {
                    writer.WriteLine($"<li><a href=\"https://www.google.fr/maps/search/{result.Feature.Geometry.Centroid.Y}+{result.Feature.Geometry.Centroid.X}\">" +
                        $"<ul><li class=\"item\">Parcelle {result.Feature.Attributes["id"]}</li>" +
                        $"<li>contenance: {result.Feature.Attributes["contenance"]} m2</li>" +
                        $"<li>surface calculée: {Math.Floor(result.ProjZoneGeometry.Area)} m2</li>" +
                        $"</a></li></ul>");
                }
                writer.WriteLine("</ul></body></html>");
            }
        }
    }
}
