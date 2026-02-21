using System.Threading.Tasks;

namespace findmyzone.IO
{
    public interface IFindMyZoneDownloader
    {
        Task<string> DownloadZone(string inseeCode);
        Task<string> DownloadBuilding(string inseeCode);
    }
}