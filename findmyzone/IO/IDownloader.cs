using System.Threading.Tasks;

namespace findmyzone.IO
{
    public interface IDownloader
    {
        string FilesDirectory { get; set; }

        Task<string> DownloadZone(string inseeCode);
        Task<string> DownloadBuilding(string inseeCode);
    }
}