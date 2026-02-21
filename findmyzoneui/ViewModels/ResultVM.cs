using findmyzone.Geo;
using ReactiveUI;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace findmyzoneui.ViewModels
{
    public class ResultVM : ViewModelBase
    {
        private readonly ZoneFinderResult result;

        public ResultVM(ZoneFinderResult result)
        {
            this.result = result;

            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            if (result.Feature.Geometry != null)
            {
                gmapLink = $"https://www.google.fr/maps/search/{result.Feature.Geometry.Centroid.Y}+{result.Feature.Geometry.Centroid.X}";
            }
        }

        private bool isVisited;

        public bool IsVisited
        {
            get => isVisited;
            set { this.RaiseAndSetIfChanged(ref isVisited, value); }
        }

        public ZoneFinderResult Result { get => result; }

        private string? gmapLink;

        public string? GmapLink
        {
            get => gmapLink;
            set { this.RaiseAndSetIfChanged(ref gmapLink, value); }
        }

        private string geoportailLink = string.Empty;

        public string GeoportailLink
        {
            get => geoportailLink;
            set { this.RaiseAndSetIfChanged(ref geoportailLink, value); }
        }

        public void Open()
        {
            var url = GmapLink;

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            try
            {
                IsVisited = true;
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
