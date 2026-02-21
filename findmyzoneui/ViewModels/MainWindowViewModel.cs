using findmyzone.Core;
using findmyzone.Geo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace findmyzoneui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {

        }

        private string city = string.Empty;

        public string City
        {
            get => city;
            set => this.RaiseAndSetIfChanged(ref city, value);
        }

        private string zipCode = string.Empty;

        public string ZipCode
        {
            get => zipCode;
            set => this.RaiseAndSetIfChanged(ref zipCode, value);
        }

        private string inseeCode = string.Empty;

        public string InseeCode
        {
            get => inseeCode;
            set => this.RaiseAndSetIfChanged(ref inseeCode, value);
        }

        private uint zoneMin;

        public uint ZoneMin
        {
            get => zoneMin;
            set => this.RaiseAndSetIfChanged(ref zoneMin, value);
        }

        private uint zoneMax;

        public uint ZoneMax
        {
            get => zoneMax;
            set => this.RaiseAndSetIfChanged(ref zoneMax, value);
        }

        private uint buildingMin;

        public uint BuildingMin
        {
            get => buildingMin;
            set => this.RaiseAndSetIfChanged(ref buildingMin, value);
        }

        private uint buildingMax;

        public uint BuildingMax
        {
            get => buildingMax;
            set => this.RaiseAndSetIfChanged(ref buildingMax, value);
        }

        public async void FindZones()
        {
            try
            {
                var findMyZoneCore = new FindMyZoneCore(null, null);

                var en = await findMyZoneCore.Find(
                    InseeCode.Split(","),
                    ZipCode.Split(","),
                    City.Split(","),
                    false,
                    ZoneMin,
                    ZoneMax,
                    BuildingMin,
                    BuildingMax,
                    false);

                Results = en.ToList();
            }
            catch (Exception e)
            {

            }
        }

        private IList<ZoneFinderResult> results = new List<ZoneFinderResult>();

        public IList<ZoneFinderResult> Results
        {
            get => results;
            set => this.RaiseAndSetIfChanged(ref results, value);
        }
    }
}
