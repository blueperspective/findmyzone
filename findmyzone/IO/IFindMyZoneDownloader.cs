using System.Threading.Tasks;

namespace findmyzone.IO
{
    public interface IFindMyZoneDownloader
    {
        string FilesDirectory { get; set; }

        Task<string> DownloadZone(string inseeCode);
        Task<string> DownloadBuilding(string inseeCode);
    }
}