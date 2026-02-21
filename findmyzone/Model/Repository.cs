using findmyzone.IO;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;

namespace findmyzone.Model
{
    class Repository : IRepository
    {
        private readonly IFeatureCollectionReader reader;

        private readonly IDictionary<string, RepoElement> codeElement = new Dictionary<string, RepoElement>();

        public Repository(IFeatureCollectionReader reader)
        {
            this.reader = reader;
        }

        public void AddZoneFile(string code, string zoneFile)
        {
            RepoElement repoElement;

            if (!codeElement.TryGetValue(code, out repoElement))
            {
                repoElement = new RepoElement();
                codeElement[code] = repoElement;
            }

            repoElement.ZoneFile = zoneFile;
        }

        public void AddBuildingFile(string code, string buildingFile)
        {
            RepoElement repoElement;

            if (!codeElement.TryGetValue(code, out repoElement))
            {
                repoElement = new RepoElement();
                codeElement[code] = repoElement;
            }

            repoElement.BuildingFile = buildingFile;
        }

        public FeatureCollection GetBuildingFeatures(string code)
        {
            if (!codeElement.TryGetValue(code, out RepoElement repoElement))
            {
                throw new ArgumentException("code not found");
            }

            if (repoElement.BuildingCollection == null)
            {
                repoElement.BuildingCollection = reader.Read(repoElement.BuildingFile);
            }

            return repoElement.BuildingCollection;
        }

        public FeatureCollection GetZoneFeatures(string code)
        {
            if (!codeElement.TryGetValue(code, out RepoElement repoElement))
            {
                throw new ArgumentException("code not found");
            }

            if (repoElement.ZoneCollection == null)
            {
                repoElement.ZoneCollection = reader.Read(repoElement.ZoneFile);
            }

            return repoElement.ZoneCollection;
        }

        class RepoElement
        {
            public string ZoneFile { get; set; }

            public FeatureCollection ZoneCollection { get; set; }

            public string BuildingFile { get; set; }
            public FeatureCollection BuildingCollection { get; set; }
        }
    }    
}
