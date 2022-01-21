using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Data.Entity.Product
{
    public class FolderUploadInfo:Base
    {
        public string folderName { get; set; }
        public string folderLink { get; set; }
    }
}
