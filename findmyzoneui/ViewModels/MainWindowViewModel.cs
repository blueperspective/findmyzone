using Avalonia.Controls;
using findmyzone.Core;
using findmyzoneui.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace findmyzoneui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AvaloniaReporter reporter;

        private readonly IUiService uiService;

        public MainWindowViewModel(Window window, IUiService uiService)
        {
            reporter = new AvaloniaReporter(window);
            this.uiService = uiService;
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
                Results.Clear();

                var inseeCodes = InseeCode.Length > 0 ? InseeCode.Split(",") : Enumerable.Empty<string>();
                var zipCodes = ZipCode.Length > 0 ? ZipCode.Split(",") : Enumerable.Empty<string>();
                var cities = City.Length > 0 ? City.Split(",") : Enumerable.Empty<string>();

                if (!inseeCodes.Any() && !zipCodes.Any() && !cities.Any())
                {
                    return;
                }

                var findMyZoneCore = new FindMyZoneCore(reporter, null);

                var finderResults = await findMyZoneCore.Find(
                    inseeCodes,
                    zipCodes,
                    cities,
                    false,
                    ZoneMin,
                    ZoneMax,
                    BuildingMin,
                    BuildingMax,
                    false);

                foreach (var r in finderResults)
                {
                    Results.Add(new ResultVM(r));
                }
            }
            catch (Exception e)
            {
                await uiService.ShowException("Error", e.Message, e);
            }
        }

        private IList<ResultVM> results = new List<ResultVM>();

        public IList<ResultVM> Results
        {
            get => results;
            set => this.RaiseAndSetIfChanged(ref results, value);
        }
    }
}
