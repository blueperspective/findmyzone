using Serilog;
using System;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace findmyzone.IO;

public class Downloader : IDownloader
{
    private const int BufferSize = 4096;

    private readonly DownloadActions downloadActions;

    public Downloader(DownloadActions downloadActions)
    {
        this.downloadActions = downloadActions;
    }

    public async Task<string> Download(string url, string destinationFile)
    {
        try
        {
            downloadActions.BeforeDownload?.Invoke();

            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            Log.Information("Downloading {url}", url);

            var totalBytes = response.Content.Headers.ContentLength;

            if (totalBytes == null)
            {
                downloadActions.Indeterminate?.Invoke();
            }

            using var contentStream = await response.Content.ReadAsStreamAsync();
            return await ProcessContentStream(totalBytes, contentStream, destinationFile);
        }
        finally
        {
            downloadActions.AfterDownload?.Invoke();
        }
    }

    private async Task<string> ProcessContentStream(long? totalDownloadSize, Stream contentStream, string destinationFilePath)
    {
        var totalBytesRead = 0L;
        var readCount = 0L;
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        var isMoreToRead = true;

        using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
        {
            uint percent = 0;
            uint prevPercent = 0;

            do
            {
                var bytesRead = await contentStream.ReadAsync(buffer);
                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                    continue;
                }

                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                readCount += 1;

                if (totalDownloadSize != null)
                {
                    percent = (uint)(bytesRead * 100 / totalDownloadSize);

                    if (percent > prevPercent)
                    {
                        prevPercent = percent;
                        downloadActions.Progress?.Invoke(percent, totalBytesRead / 1024, totalDownloadSize / 1024);
                    }
                }
                else
                {
                    downloadActions.Progress?.Invoke(0, totalBytesRead / 1024, totalDownloadSize / 1024);
                }
            }
            while (isMoreToRead);
        }

        Log.Information("Download complete");

        ArrayPool<byte>.Shared.Return(buffer);
        return destinationFilePath;
    }
}

