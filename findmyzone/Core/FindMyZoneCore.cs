using findmyzone.Geo;
using findmyzone.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace findmyzone.Core
{
    public class FindMyZoneCore
    {
        private readonly IRepository repository;

        public  FindMyZoneCore(IRepository repository)
        {
            this.repository = repository;
        }

        public IAsyncEnumerable<ZoneFinderResult> Find(IEnumerable<string> inseeCodes, bool useComputedArea, uint minLotArea, uint maxLotArea, uint minBuildingArea, uint maxBuildingArea, bool ignoreBuildings)
        {
            // force culture for google map coordinates
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            var finder = new ZoneFinder(repository);

            IAsyncEnumerable<ZoneFinderResult> results = AsyncEnumerable.Empty<ZoneFinderResult>();

            foreach (var inseeCode in inseeCodes)
            {
                var res = finder.FindZone(
                    inseeCode,
                    minLotArea,
                    maxLotArea,
                    useComputedArea,
                    minBuildingArea,
                    maxBuildingArea,
                    ignoreBuildings);

                results = results.Concat(res);
            }

            return results;
        }
    }
}
