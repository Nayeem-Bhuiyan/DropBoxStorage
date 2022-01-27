using Dropbox.Api;
using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Service;
using DropboxCore.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Controllers
{
    [Area("DropBox")]
    public class DownloadDropBoxController : Controller
    {

        private IDropboxManager _dropBoxService;
        private IDownloadDropBoxService _downloadDropBoxService;
        private IWebHostEnvironment _environment;
        public DownloadDropBoxController(IDropboxManager dropBoxService, IDownloadDropBoxService downloadDropBoxService, IWebHostEnvironment environment)
        {
            _dropBoxService = dropBoxService;
            _downloadDropBoxService= downloadDropBoxService; ;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult DownloadZip()
        {
            DownloadDropBoxViewModel model = new DownloadDropBoxViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DownloadZip(DownloadDropBoxViewModel model)
        {
            try
            {
                bool response = false;
                response = await _dropBoxService.DownloadFolder(model.dropboxFolderPath, model.localFolderPath);
                if (response)
                {
                    model.message = "Success";
                }
                else
                {
                    model.message = "Error";
                }
            }
            catch (Exception ex)
            {
                model.message = ex.Message;
                throw;
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult DownloadFiles()
        {
            DownloadDropBoxViewModel model = new DownloadDropBoxViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DownloadFiles(DownloadDropBoxViewModel model)
        {
            try
            {
                string response = null;

                response = await _dropBoxService.DownloadFile1(model.dropboxFolderPath, model.localFolderPath);

                model.message = response;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return View(model);
        }


        //public async Task<List<string>> GetFiles()
        //{
        //    var dbx = new DropboxClient("ddnqP8VkuRIAAAAAAAAAAbakpkt52c-Nr4oznaXrc368Z2HxMu5Nhb_GQeFAJM26");
        //    var list = await dbx.Files.ListFolderAsync("/Upload-22-01-2022/");
        //    List<string> FilePaths = new List<string>();
        //    foreach (var file in list.Entries.Where(i => i.IsFile))
        //    {
        //        FilePaths.Add(file.AsFile.PathLower);
        //    }
        //    return FilePaths;
        //}
        //public async Task<FileResult> Download()
        //{

        //    var dbx = new DropboxClient("ddnqP8VkuRIAAAAAAAAAAbakpkt52c-Nr4oznaXrc368Z2HxMu5Nhb_GQeFAJM26");
        //    var list = await dbx.Files.ListFolderAsync("/Upload-22-01-2022/");
        //    List<string> paths = new List<string>();
        //    foreach (var file in list.Entries.Where(i => i.IsFile))
        //    {
        //        paths.Add(file.AsFile.PathLower);
        //    }


        //    byte[] fileBytes = null;
        //    string path = paths.FirstOrDefault();
        //    int index = path.LastIndexOf('/');
        //    string fileName = path.Substring(index + 1);

        //    using (var dbx1 = new DropboxClient("ddnqP8VkuRIAAAAAAAAAAbakpkt52c-Nr4oznaXrc368Z2HxMu5Nhb_GQeFAJM26"))
        //    {

        //        using (var response = dbx1.Files.DownloadAsync(path).Result)
        //        {
        //            fileBytes = response.GetContentAsByteArrayAsync().Result;
        //        }
        //    }

        //    if (fileBytes == null)
        //    {
        //        return null;
        //    }

        //    var contentDispositionHeader = new System.Net.Mime.ContentDisposition
        //    {
        //        Inline = false,
        //        FileName = fileName
        //    };

        //    Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet);
        //}


        [HttpPost]
        public async Task<IActionResult> DownloadFile(DownloadDropBoxViewModel model)
        {
            try
            {
                //Downloader data = new Downloader(@"D:\ExceedSystemTest\DropBoxStorage\DropboxCore\wwwroot\Upload\WebDevelopment.zip", @"D:\ExceedSystemTest\DropBoxStorage\DropboxCore\wwwroot\Download",2000);
                DropboxDownloadResult response = await _downloadDropBoxService.GetFileDownload(model.dropboxFilePath);
                return File(response.Content, response.FileMypeType, response.FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
    {
    {"ai", "application/postscript"},
    {"aif", "audio/x-aiff"},
    {"aifc", "audio/x-aiff"},
    {"aiff", "audio/x-aiff"},
    {"asc", "text/plain"},
    {"atom", "application/atom+xml"},
    {"au", "audio/basic"},
    {"avi", "video/x-msvideo"},
    {"bcpio", "application/x-bcpio"},
    {"bin", "application/octet-stream"},
    {"bmp", "image/bmp"},
    {"cdf", "application/x-netcdf"},
    {"cgm", "image/cgm"},
    {"class", "application/octet-stream"},
    {"cpio", "application/x-cpio"},
    {"cpt", "application/mac-compactpro"},
    {"csh", "application/x-csh"},
    {"css", "text/css"},
    {"dcr", "application/x-director"},
    {"dif", "video/x-dv"},
    {"dir", "application/x-director"},
    {"djv", "image/vnd.djvu"},
    {"djvu", "image/vnd.djvu"},
    {"dll", "application/octet-stream"},
    {"dmg", "application/octet-stream"},
    {"dms", "application/octet-stream"},
    {"doc", "application/msword"},
    {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
    {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
    {"docm","application/vnd.ms-word.document.macroEnabled.12"},
    {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
    {"dtd", "application/xml-dtd"},
    {"dv", "video/x-dv"},
    {"dvi", "application/x-dvi"},
    {"dxr", "application/x-director"},
    {"eps", "application/postscript"},
    {"etx", "text/x-setext"},
    {"exe", "application/octet-stream"},
    {"ez", "application/andrew-inset"},
    {"gif", "image/gif"},
    {"gram", "application/srgs"},
    {"grxml", "application/srgs+xml"},
    {"gtar", "application/x-gtar"},
    {"hdf", "application/x-hdf"},
    {"hqx", "application/mac-binhex40"},
    {"htm", "text/html"},
    {"html", "text/html"},
    {"ice", "x-conference/x-cooltalk"},
    {"ico", "image/x-icon"},
    {"ics", "text/calendar"},
    {"ief", "image/ief"},
    {"ifb", "text/calendar"},
    {"iges", "model/iges"},
    {"igs", "model/iges"},
    {"jnlp", "application/x-java-jnlp-file"},
    {"jp2", "image/jp2"},
    {"jpe", "image/jpeg"},
    {"jpeg", "image/jpeg"},
    {"jpg", "image/jpeg"},
    {"js", "application/x-javascript"},
    {"kar", "audio/midi"},
    {"latex", "application/x-latex"},
    {"lha", "application/octet-stream"},
    {"lzh", "application/octet-stream"},
    {"m3u", "audio/x-mpegurl"},
    {"m4a", "audio/mp4a-latm"},
    {"m4b", "audio/mp4a-latm"},
    {"m4p", "audio/mp4a-latm"},
    {"m4u", "video/vnd.mpegurl"},
    {"m4v", "video/x-m4v"},
    {"mac", "image/x-macpaint"},
    {"man", "application/x-troff-man"},
    {"mathml", "application/mathml+xml"},
    {"me", "application/x-troff-me"},
    {"mesh", "model/mesh"},
    {"mid", "audio/midi"},
    {"midi", "audio/midi"},
    {"mif", "application/vnd.mif"},
    {"mov", "video/quicktime"},
    {"movie", "video/x-sgi-movie"},
    {"mp2", "audio/mpeg"},
    {"mp3", "audio/mpeg"},
    {"mp4", "video/mp4"},
    {"mpe", "video/mpeg"},
    {"mpeg", "video/mpeg"},
    {"mpg", "video/mpeg"},
    {"mpga", "audio/mpeg"},
    {"ms", "application/x-troff-ms"},
    {"msh", "model/mesh"},
    {"mxu", "video/vnd.mpegurl"},
    {"nc", "application/x-netcdf"},
    {"oda", "application/oda"},
    {"ogg", "application/ogg"},
    {"pbm", "image/x-portable-bitmap"},
    {"pct", "image/pict"},
    {"pdb", "chemical/x-pdb"},
    {"pdf", "application/pdf"},
    {"pgm", "image/x-portable-graymap"},
    {"pgn", "application/x-chess-pgn"},
    {"pic", "image/pict"},
    {"pict", "image/pict"},
    {"png", "image/png"},
    {"pnm", "image/x-portable-anymap"},
    {"pnt", "image/x-macpaint"},
    {"pntg", "image/x-macpaint"},
    {"ppm", "image/x-portable-pixmap"},
    {"ppt", "application/vnd.ms-powerpoint"},
    {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
    {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
    {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
    {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
    {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
    {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
    {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
    {"ps", "application/postscript"},
    {"qt", "video/quicktime"},
    {"qti", "image/x-quicktime"},
    {"qtif", "image/x-quicktime"},
    {"ra", "audio/x-pn-realaudio"},
    {"ram", "audio/x-pn-realaudio"},
    {"ras", "image/x-cmu-raster"},
    {"rdf", "application/rdf+xml"},
    {"rgb", "image/x-rgb"},
    {"rm", "application/vnd.rn-realmedia"},
    {"roff", "application/x-troff"},
    {"rtf", "text/rtf"},
    {"rtx", "text/richtext"},
    {"sgm", "text/sgml"},
    {"sgml", "text/sgml"},
    {"sh", "application/x-sh"},
    {"shar", "application/x-shar"},
    {"silo", "model/mesh"},
    {"sit", "application/x-stuffit"},
    {"skd", "application/x-koan"},
    {"skm", "application/x-koan"},
    {"skp", "application/x-koan"},
    {"skt", "application/x-koan"},
    {"smi", "application/smil"},
    {"smil", "application/smil"},
    {"snd", "audio/basic"},
    {"so", "application/octet-stream"},
    {"spl", "application/x-futuresplash"},
    {"src", "application/x-wais-source"},
    {"sv4cpio", "application/x-sv4cpio"},
    {"sv4crc", "application/x-sv4crc"},
    {"svg", "image/svg+xml"},
    {"swf", "application/x-shockwave-flash"},
    {"t", "application/x-troff"},
    {"tar", "application/x-tar"},
    {"tcl", "application/x-tcl"},
    {"tex", "application/x-tex"},
    {"texi", "application/x-texinfo"},
    {"texinfo", "application/x-texinfo"},
    {"tif", "image/tiff"},
    {"tiff", "image/tiff"},
    {"tr", "application/x-troff"},
    {"tsv", "text/tab-separated-values"},
    {"txt", "text/plain"},
    {"ustar", "application/x-ustar"},
    {"vcd", "application/x-cdlink"},
    {"vrml", "model/vrml"},
    {"vxml", "application/voicexml+xml"},
    {"wav", "audio/x-wav"},
    {"wbmp", "image/vnd.wap.wbmp"},
    {"wbmxl", "application/vnd.wap.wbxml"},
    {"wml", "text/vnd.wap.wml"},
    {"wmlc", "application/vnd.wap.wmlc"},
    {"wmls", "text/vnd.wap.wmlscript"},
    {"wmlsc", "application/vnd.wap.wmlscriptc"},
    {"wrl", "model/vrml"},
    {"xbm", "image/x-xbitmap"},
    {"xht", "application/xhtml+xml"},
    {"xhtml", "application/xhtml+xml"},
    {"xls", "application/vnd.ms-excel"},
    {"xml", "application/xml"},
    {"xpm", "image/x-xpixmap"},
    {"xsl", "application/xml"},
    {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
    {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
    {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
    {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
    {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
    {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
    {"xslt", "application/xslt+xml"},
    {"xul", "application/vnd.mozilla.xul+xml"},
    {"xwd", "image/x-xwindowdump"},
    {"xyz", "chemical/x-xyz"},
    {"zip", "application/zip"}
  };

        public static string GetMIMEType(string fileName)
        {
            //get file extension
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension.Length > 0 &&
                MIMETypesDictionary.ContainsKey(extension.Remove(0, 1)))
            {
                return MIMETypesDictionary[extension.Remove(0, 1)];
            }
            return "unknown/unknown";
        }



    /// </summary>
    }


    /// <summary>
    /// Implementation of the downloader module
   //public class Downloader
   // {
   //     //Units in bytes
   //     public const int MEGA_BYTE = 1048576;
   //     public const int KILO_BYTE = 1024;
   //     public const int CHUNK_BUFFER = 512;

   //     //Sources and Destination
   //     private string SourceURL { get; set; }
   //     private string DestinationPath { get; set; }

   //     //Data for tracking the download progress and status
   //     private enum DownloadChunkResults { DownloadComplete, ContinueDownload, AbortDownload, DownloadError }
   //     public long DownloadedSize { get; private set; }
   //     public int TempPercentage { get; private set; }
   //     private long TempFileSize { get; set; }
   //     private int MaxLimitSize { get; set; }
   //     private volatile bool abortFlag;

   //     //Data for tracking download speed
   //     private DateTime timeMills;
   //     private long lastDownloadSize;
   //     private float dwnlSpeed;

   //     //The thread that downloads the file
   //     private Thread DownloadThread;

   //     /// <summary>
   //     /// Initializes the download
   //     /// </summary>
   //     /// <param name="sourceUrl">The source from which the file is to be downloaded from</param>
   //     /// <param name="destinationPath">The path where the file is to be saved</param>
   //     /// <param name="maxLimitSize">The maximum size of the file</param>
   //     public Downloader(string sourceUrl, string destinationPath, int maxLimitSize)
   //     {
   //         //Set the variables based on user inputs
   //         this.SourceURL = sourceUrl;
   //         this.DestinationPath = destinationPath;
   //         this.MaxLimitSize = maxLimitSize * MEGA_BYTE;
   //         abortFlag = false;

   //         //Initialize the downloader
   //         DownloadThread = new Thread(new ThreadStart(DownloadFile));
   //     }

   //     /// <summary>
   //     /// Starts the downloader by starting the downloader thread
   //     /// and also sets the initial state
   //     /// </summary>
   //     public void StartDownload()
   //     {
   //         //Assume the parameters for the download
   //         TempFileSize = 0;
   //         DownloadedSize = 0;
   //         timeMills = DateTime.Now;
   //         lastDownloadSize = 0;

   //         //Create the file and close it
   //         //Doing so deletes any previous copies existing
   //         File.Create(DestinationPath).Close();

   //         //Start the download
   //         DownloadThread.Start();
   //     }

   //     /// <summary>
   //     /// Aborts the thread by setting the flag variable to true
   //     /// </summary>
   //     public void AbortDownload() { abortFlag = true; }

   //     /// <summary>
   //     /// This will download the file in chunks
   //     /// </summary>
   //     private void DownloadFile()
   //     {
   //         //Download one chunk at a time and get its results
   //         //Notify the user of the final results
   //         //Provide a way for the other threads to know when to reset to their initial state
   //         while (true)
   //         {
   //             //I set 100% to all the fields : this can be considered as a communication
   //             //i.e., other threads might reset their state as if the download is complete
   //             switch (DownloadChunk(DownloadedSize))
   //             {
   //                 case DownloadChunkResults.DownloadComplete:
   //                     TempPercentage = 100;
   //                     Console.WriteLine("Download Completed \n  - File Name : " + Path.GetFileNameWithoutExtension(DestinationPath) + " \n  - File Size    : " + ((float)DownloadedSize / MEGA_BYTE).ToString("0.## MB"), "Download Complete");
   //                     return;
   //                 case DownloadChunkResults.DownloadError:
   //                     TempPercentage = 100;
   //                     File.Delete(DestinationPath);
   //                     return;
   //                 case DownloadChunkResults.AbortDownload:
   //                     TempPercentage = 100;
   //                     Console.WriteLine("The download has been aborted by the user", "Download Aborted");
   //                     File.Delete(DestinationPath);
   //                     return;
   //             }
   //         }
   //     }

   //     /// <summary>
   //     /// Downloads one chunk worth of data from the Internet
   //     /// </summary>
   //     /// <param name="startPos">The position where we want to start the download</param>
   //     /// <returns>The enumerated results which shows the state of this chunk</returns>
   //     DownloadChunkResults DownloadChunk(long startPos)
   //     {
   //         //IO streams
   //         Stream dwnlRes = null;
   //         BufferedStream dwnlStream = null;

   //         try
   //         {
   //             //Set the request for downloading the new chunk
   //             HttpWebRequest dwnlReq = (HttpWebRequest)WebRequest.Create(SourceURL);
   //             dwnlReq.AddRange(startPos, startPos + MaxLimitSize);
   //             dwnlReq.AllowAutoRedirect = true;

   //             //Get the response stream and open an output stream
   //             dwnlRes = dwnlReq.GetResponse().GetResponseStream();
   //             dwnlStream = new BufferedStream(new FileStream(DestinationPath, FileMode.Append, FileAccess.Write));

   //             //Get the content length and update the new file size with this value
   //             long contentLength = dwnlReq.GetResponse().ContentLength;
   //             TempFileSize += contentLength;

   //             //while there is data or the download is not aborted download the current chunk
   //             int partlen;
   //             byte[] chunkBuffer = new byte[CHUNK_BUFFER];
   //             while (!abortFlag && (partlen = dwnlRes.Read(chunkBuffer, 0, CHUNK_BUFFER)) > 0)
   //             {
   //                 //write the contents to the output file
   //                 dwnlStream.Write(chunkBuffer, 0, partlen);

   //                 //Update the progress parameters
   //                 DownloadedSize += partlen;
   //                 TempPercentage = Convert.ToInt32((DownloadedSize * 100) / TempFileSize);
   //             }

   //             //Close all the streams
   //             dwnlRes.Close();
   //             dwnlStream.Close();

   //             //Message the callee by returning an enumerated result
   //             //The message depends on the download state
   //             if (abortFlag) { return DownloadChunkResults.AbortDownload; }
   //             else if (contentLength != MaxLimitSize + 1) { return DownloadChunkResults.DownloadComplete; }
   //             else { return DownloadChunkResults.ContinueDownload; }
   //         }
   //         catch (Exception e)
   //         {
   //             //Initmate the user of the reason and release all the resources
   //             //Also delete the partial download file
   //             //Message the callee about the error
   //             Console.WriteLine(e.Message, "Download Failed");
   //             try { dwnlRes.Close(); dwnlStream.Close(); }
   //             catch { }
   //             return DownloadChunkResults.DownloadError;
   //         }
   //     }

   //     /// <summary>
   //     /// Calculates and returns the download speed with units either in MBps or KBps
   //     /// </summary>
   //     /// <returns>The formatted string with download speed</returns>
   //     public string DownloadSpeed()
   //     {
   //         //Calculate the download speed
   //         long sizeDiff = lastDownloadSize - (lastDownloadSize = DownloadedSize);
   //         int timeDiff = (timeMills - (timeMills = DateTime.Now)).Milliseconds;

   //         //If download speed is greater than 1MB then display it in MBps else in KBps
   //         dwnlSpeed = dwnlSpeed + (float)sizeDiff / timeDiff;
   //         dwnlSpeed /= 2;
   //         if (dwnlSpeed > KILO_BYTE) { return (dwnlSpeed / KILO_BYTE).ToString("0.00 MBps"); }
   //         else { return (dwnlSpeed).ToString("0.00 KBps"); }
   //     }

   //     /// <summary>
   //     /// Calculates the downloaded file size in MB or KB and returns it
   //     /// </summary>
   //     /// <returns>The formatted string with the downloaded size</returns>
   //     public string SizeDownloaded()
   //     {
   //         if (DownloadedSize > MEGA_BYTE) { return ((float)DownloadedSize / MEGA_BYTE).ToString("0.00 MB"); }
   //         else { return ((float)DownloadedSize / KILO_BYTE).ToString("0.00 KB"); }
   //     }
   // }

    /// <summary>
    /// Implementation of the module to test whether the download is supported or not
    /// </summary>
    //class TestDownload
    //{
    //    //Parameters
    //    string SourceURL;
    //    int MaxLimitSize;

    //    /// <summary>
    //    /// Initializes the download
    //    /// </summary>
    //    /// <param name="sourceURL">The URL to test</param>
    //    /// <param name="maxLimitSize">The maximum file size</param>
    //    public TestDownload(string sourceURL, int maxLimitSize)
    //    {
    //        //Assign the data for the local variables
    //        SourceURL = sourceURL;
    //        MaxLimitSize = maxLimitSize * Downloader.MEGA_BYTE;

    //        //Start the download tester
    //        new Thread(new ThreadStart(Test)).Start();
    //    }

    //    /// <summary>
    //    /// This will test the link whether it accepts byte range or not
    //    /// </summary>
    //    private void Test()
    //    {
    //        try
    //        {
    //            //Create request to download one chunk of data
    //            //By applying a range to it
    //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SourceURL);
    //            request.AddRange(0, MaxLimitSize);
    //            request.Timeout = 10000;

    //            //Get the response from the server
    //            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

    //            //Check to see if the AcceptRanges field is "bytes"
    //            //If so the downloads are byte addressed and can be downloaded by this downloader
    //            if (response.Headers[HttpResponseHeader.AcceptRanges] == "bytes")
    //            {
    //                Console.WriteLine("Download Supported");
    //            }
    //            else
    //            {
    //                Console.WriteLine("Download Not Supported");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message, "Network Error");
    //        }
    //    }
    //}
}
