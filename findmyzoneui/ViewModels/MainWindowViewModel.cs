using Avalonia.Controls;
using findmyzone.Core;
using findmyzone.Geo;
using findmyzone.IO;
using findmyzone.Model;
using findmyzone.Resources;
using findmyzone.Win;
using findmyzoneui.Services;
using findmyzoneui.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace findmyzoneui.ViewModels
{
    [DataContract]
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IUiService uiService;

        private readonly IRepository repository;

        private readonly IZoneFinder zoneFinder;

        private readonly ICoreSettings coreSettings;

        private readonly SettingsVM settingsVM;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainWindowViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (!Design.IsDesignMode)
            {
                throw new Exception("Not in design");
            }

            Results = new ObservableCollection<ResultVM>
            {
                new ResultVM(new ZoneFinderResult
                    {
                        ProjZoneGeometry = new NetTopologySuite.Geometries.Point(1, 1),
                        Feature = new NetTopologySuite.Features.Feature()
                    })
                {
                }
            };
        }

        public MainWindowViewModel(MainWindow window, IUiService uiService, IRepository repository, IZoneFinder zoneFinder, ICoreSettings coreSettings, SettingsVM settingsVM)
        {
            this.uiService = uiService;
            this.repository = repository;
            this.zoneFinder = zoneFinder;
            this.coreSettings = coreSettings;
            this.settingsVM = settingsVM;

            _ = LoadRepository();
        }

        [DataMember]
        public SettingsVM SettingsVM { get => settingsVM; }

        private bool isLoading = false;

        public bool IsLoading
        {
            get => isLoading;
            set => this.RaiseAndSetIfChanged(ref isLoading, value);
        }

        private bool isSearching = false;

        public bool IsSearching
        {
            get => isSearching;
            set => this.RaiseAndSetIfChanged(ref isSearching, value);
        }

        private uint zoneMin;

        [DataMember]
        public uint ZoneMin
        {
            get => zoneMin;
            set
            {
                if (value != zoneMin)
                {
                    zoneMin = value;
                    this.RaisePropertyChanged(nameof(ZoneMin));

                    if (ZoneMax < zoneMin)
                    {
                        ZoneMax = zoneMin + 100;
                    }
                }
            }
        }

        private uint zoneMax;

        [DataMember]
        public uint ZoneMax
        {
            get => zoneMax;
            set => this.RaiseAndSetIfChanged(ref zoneMax, value);
        }

        private uint buildingMin;

        [DataMember]
        public uint BuildingMin
        {
            get => buildingMin;
            set
            {
                if (value != buildingMin)
                {
                    buildingMin = value;
                    this.RaisePropertyChanged(nameof(BuildingMin));

                    if (BuildingMax < buildingMin)
                    {
                        BuildingMax = buildingMin + 100;
                    }
                }
            }
        }

        private uint buildingMax;

        [DataMember]
        public uint BuildingMax
        {
            get => buildingMax;
            set => this.RaiseAndSetIfChanged(ref buildingMax, value);
        }

        private CityInfo? selectedCity;

        public CityInfo? SelectedCity
        {
            get => selectedCity;
            set => this.RaiseAndSetIfChanged(ref selectedCity, value);
        }

        private List<CityInfo>? cities;

        public List<CityInfo>? Cities
        {
            get => cities;
            set => this.RaiseAndSetIfChanged(ref cities, value);
        }

        public AutoCompleteFilterPredicate<object> CityFilter { get; } = new AutoCompleteFilterPredicate<object>((search, item) =>
        {
            if (item is CityInfo city)
            {
                return city.Name.StartsWith(search, StringComparison.InvariantCultureIgnoreCase)
                || city.InseeCode.StartsWith(search)
                || city.ZipCodes.Any(x => x.StartsWith(search));
            }

            return false;
        });

        public async Task LoadRepository()
        {
            try
            {
                IsLoading = true;

                await Task.Run(() =>
                {
                    using (var reader = new StreamReader(Path.Combine(coreSettings.DownloadDirectory, "correspondance-code-insee-code-postal.csv")))
                    {
                        var cityReader = new CityInfoReader(reader);
                        cityReader.Fill(repository);
                    }
                });

                Cities = repository.Cities.OrderBy(x => x.Name).ToList();
            }
            catch (Exception e)
            {
                await uiService.ShowException("Error", e.Message, e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void FindZones()
        {
            try
            {
                IsSearching = true;
                Results.Clear();

                if (SelectedCity == null)
                {
                    return;
                }

                var finderResults = zoneFinder.FindZone(
                    SelectedCity.InseeCode,
                    ZoneMin,
                    ZoneMax,
                    false,
                    BuildingMin,
                    BuildingMax,
                    false);

                ResultCount = await finderResults.CountAsync();

                await foreach (var r in finderResults)
                {
                    Results.Add(new ResultVM(r));
                }
            }
            catch (Exception e)
            {
                await uiService.ShowException("Error", e.Message, e);
            }
            finally
            {
                IsSearching = false;
            }
        }

        private int resultCount;

        public int ResultCount
        {
            get => resultCount;
            set => this.RaiseAndSetIfChanged(ref resultCount, value);
        }

        private ObservableCollection<ResultVM> results = new();

        [IgnoreDataMember]
        public ObservableCollection<ResultVM> Results
        {
            get => results;
            set => this.RaiseAndSetIfChanged(ref results, value);
        }
    }
}
