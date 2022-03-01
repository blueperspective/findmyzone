using System;

namespace findmyzone.IO
{
    public class DownloadActions
    {
        public Action? BeforeDownload { get; set; }

        public Action? AfterDownload { get; set; }

        public Action<uint, long?, long?>? Progress { get; set; }

        public Action? Indeterminate { get; set; }
    }
}
