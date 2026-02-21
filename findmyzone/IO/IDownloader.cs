using System.Threading.Tasks;

namespace findmyzone.IO
{
    public interface IDownloader
    {
        Task<string> Download(string url, string destinationFile);
    }
}