using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Models
{
    public class DownloadDropBoxViewModel
    {
        public string dropboxFolderPath { get; set; }
        public string dropboxFilePath { get; set; }
        public string localFolderPath { get; set; }
        public string message { get; set; }
    }
}
