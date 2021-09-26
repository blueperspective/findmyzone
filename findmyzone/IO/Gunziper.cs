using findmyzone.Resources;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using System.IO;

namespace findmyzone.IO
{
    public class Gunziper : IGunziper
    {
        private readonly IReporter reporter;
        public Gunziper(IReporter reporter)
        {
            this.reporter = reporter;
        }

        public void UngzipFile(string gzipFile, string filesDirectory)
        {
            reporter?.StartOp(Messages.UngzipFile, gzipFile);

            try
            {
                // Use a 4K buffer. Any larger is a waste.    
                byte[] dataBuffer = new byte[4096];

                using (var fs = new FileStream(gzipFile, FileMode.Open, FileAccess.Read))
                {
                    using (var gzipStream = new GZipInputStream(fs))
                    {
                        // Change this to your needs
                        string fnOut = Path.Combine(filesDirectory, Path.GetFileNameWithoutExtension(gzipFile));

                        using (FileStream fsOut = File.Create(fnOut))
                        {
                            StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                        }
                    }
                }

                reporter?.OpEndSuccess();
            }
            catch
            {
                reporter?.OpEndError();
                throw;
            }
        }
    }
}
