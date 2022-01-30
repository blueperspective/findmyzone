using System;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace findmyzone.IO;

public class Downloader2
{
    private const int BufferSize = 4096;

    public async Task Download(string url, string destination, Action<uint, long?, long?> progress, Action indeterminate)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        var totalBytes = response.Content.Headers.ContentLength;

        if (totalBytes == null)
        {
            indeterminate.Invoke();
        }

        using var contentStream = await response.Content.ReadAsStreamAsync() ;
        await ProcessContentStream(totalBytes, contentStream, destination, progress);
    }

    private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream, string destinationFilePath, Action<uint, long?, long?> progress)
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
                var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
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
                        progress.Invoke(percent, totalBytesRead / 1024, totalDownloadSize / 1024);
                    }
                }
                else
                {
                    progress.Invoke(0, totalBytesRead / 1024, totalDownloadSize / 1024);
                }
            }
            while (isMoreToRead);
        }

        ArrayPool<byte>.Shared.Return(buffer);
    }
}

