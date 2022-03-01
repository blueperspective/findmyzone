using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using findmyzone.Core;
using findmyzone.Geo;
using findmyzone.IO;
using findmyzone.Model;
using findmyzoneui.Resources;
using findmyzoneui.Services;
using findmyzoneui.Views;
using ReactiveUI;
using Serilog;
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
        private const string InseeZipDataset = @"https://www.data.gouv.fr/fr/datasets/correspondance-entre-les-codes-postaux-et-codes-insee-des-communes-francaises/";

        private const string InseeZipUrl = @"https://public.opendatasoft.com/explore/dataset/correspondance-code-insee-code-postal/download/?format=csv&timezone=Europe/Berlin&lang=fr&use_labels_for_header=true&csv_separator=%3B";

        private const string InseeZipFilename = "correspondance-code-insee-code-postal.csv";

        private readonly IUiService uiService;
        private readonly IRepository repository;
        private readonly IZoneFinder zoneFinder;
        private readonly ICoreSettings coreSettings;
        private readonly IManagedNotificationManager notificationManager;
        private readonly SettingsVM settingsVM;
        private readonly IDownloader downloader;
        private readonly DownloadActions downloadActions;

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
                new ResultVM(
                    new ZoneFinderResult
                    {
                        ProjZoneGeometry = new NetTopologySuite.Geometries.Point(1, 1),
                        Feature = new NetTopologySuite.Features.Feature()
                    })
                {
                }
            };
        }

        public MainWindowViewModel(IUiService uiService, IRepository repository, IZoneFinder zoneFinder, ICoreSettings coreSettings, IManagedNotificationManager notificationManager, SettingsVM settingsVM, IDownloader downloader, DownloadActions downloadActions)
        {
            this.uiService = uiService;
            this.repository = repository;
            this.zoneFinder = zoneFinder;
            this.coreSettings = coreSettings;
            this.notificationManager = notificationManager;
            this.settingsVM = settingsVM;
            this.downloader = downloader;
            this.downloadActions = downloadActions;

            downloadActions.BeforeDownload = () =>
            {
                IsDownloading = true;
                IsIndeterminate = false;
                DownloadName = InseeZipFilename;
            };

            downloadActions.AfterDownload = () =>
            {
                IsDownloading = false;
            };

            downloadActions.Progress = (percent, downloaded, total) =>
            {
                DownloadProgress = percent;
                DownloadedKb = downloaded;
                TotalKb = totalKb;
            };

            downloadActions.Indeterminate = () => IsIndeterminate = true;
        }

        public async Task LoadRepository()
        {
            try
            {
                if (Cities != null)
                {
                    return;
                }

                IsLoading = true;

                string inseeZipFile = Path.Combine(coreSettings.DownloadDirectory, InseeZipFilename);

                if (!File.Exists(inseeZipFile))
                {
                    try
                    {
                        Log.Information("Insee file not found, downloading it");
                        notificationManager.Show(new Notification(UiMessages.FirstTimeUse, UiMessages.DownloadingZipInseeCodes));
                        await downloader.Download(InseeZipUrl, inseeZipFile);
                    }
                    catch (Exception ex)
                    {
                        await uiService.ShowException(UiMessages.Error, ex.Message, ex);
                        await uiService.ShowMessage(UiMessages.Error, string.Format(UiMessages.ErrorDownloadingZipInseeCodes, InseeZipDataset, InseeZipUrl));
                        return;
                    }
                    finally
                    {
                        IsDownloading = false;
                    }
                }

                await Task.Run(() =>
                {
                    using var reader = new StreamReader(inseeZipFile);
                    var cityReader = new CityInfoReader(reader);
                    cityReader.Fill(repository);
                });

                Cities = repository.Cities.OrderBy(x => x.Name).ToList();
            }
            catch (Exception e)
            {
                await uiService.ShowException(UiMessages.Error, e.Message, e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region download

        private bool isDownloading;

        public bool IsDownloading
        {
            get => isDownloading;
            set => this.RaiseAndSetIfChanged(ref isDownloading, value);
        }

        private string? downloadName;

        public string? DownloadName
        {
            get => downloadName;
            set => this.RaiseAndSetIfChanged(ref downloadName, value);
        }

        private bool isIndeterminate;

        public bool IsIndeterminate
        {
            get => isIndeterminate;
            set => this.RaiseAndSetIfChanged(ref isIndeterminate, value);
        }

        private uint downloadProgress;

        public uint DownloadProgress
        {
            get => downloadProgress;
            set => this.RaiseAndSetIfChanged(ref downloadProgress, value);
        }

        private long? downloadedKb;

        public long? DownloadedKb
        {
            get => downloadedKb;
            set => this.RaiseAndSetIfChanged(ref downloadedKb, value);
        }

        private long? totalKb;

        public long? TotalKb
        {
            get => totalKb;
            set => this.RaiseAndSetIfChanged(ref totalKb, value);
        }

        #endregion

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
                return city.Name != null && city.Name.StartsWith(search, StringComparison.InvariantCultureIgnoreCase)
                || (city.InseeCode != null && city.InseeCode.StartsWith(search))
                || (city.ZipCodes != null && city.ZipCodes.Any(x => x.StartsWith(search)));
            }

            return false;
        });

        public async void FindZones()
        {
            try
            {
                if (SelectedCity == null || SelectedCity.InseeCode == null)
                {
                    return;
                }

                IsSearching = true;
                Results.Clear();

                await Task.Run(async () =>
                {
                    var finderResults = zoneFinder.FindZone(
                        SelectedCity.InseeCode,
                        ZoneMin,
                        ZoneMax,
                        false,
                        BuildingMin,
                        BuildingMax,
                        false);


                    await foreach (var r in finderResults)
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            Results.Add(new ResultVM(r));
                        });
                    }

                    await Dispatcher.UIThread.InvokeAsync(async () => { ResultCount = await finderResults.CountAsync(); });
                });
            }
            catch (Exception e)
            {
                await uiService.ShowException(UiMessages.Error, e.Message, e);
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
