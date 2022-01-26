using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Models
{
    public class UploadDropBoxViewModel
    {
        public string filesName { get; set; }
        public IEnumerable<IFormFile> files { get; set; }
        public string fileSourcePath { get; set; }
        public string responseMessage { get; set; }
    }
}
