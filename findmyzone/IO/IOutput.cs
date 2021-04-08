using findmyzone.Geo;
using System.Collections.Generic;

namespace findmyzone.IO
{
    interface IOutput
    {
        void Render(IEnumerable<ZoneFinderResult> results);
    }
}