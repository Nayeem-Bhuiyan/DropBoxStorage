using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Models
{
    public class DropboxDownloadResult
    {
        public string FileName { get; internal set; }
        public string FileMypeType { get; internal set; }
        public ulong FileSize { get { return (ulong)(Content != null ? Content.Length : 0); } }
        public byte[] Content { get; internal set; }
        public byte[] ByteArray { get; internal set; }
    }
}
