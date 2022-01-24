using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DropboxCore.Extensions;
using DropboxCore.Models;
using System.IO;
using System.Net;
using System.Xml;
using dropboxApi = global::Dropbox.Api;
using Microsoft.Extensions.Configuration;
using Dropbox.Api.Files;

namespace DropboxCore.Services
{
    public class DropboxManager : IDropboxManager
    {

        private IConfiguration _IConfiguration;
        public DropboxManager(IConfiguration IConfiguration)
        {
            _IConfiguration = IConfiguration;
        }


        private const string APP_ROOT_URI = "/IngenStudioAppFolder"; // Set your application root folder name

        /// <summary>
        /// Contains a list with downloaded files and its metadata.
        /// Key: Dropbox file full URI,
        /// Value: File metadata information.
        /// </summary>
        public static Dictionary<string, FileMetadataInfo> FilesToDownload = new Dictionary<string, FileMetadataInfo>();

        /// <summary>
        /// Currently downloading files count.
        /// </summary>
        public static int FilesToDownloadCount { get; set; }

        /// <summary>
        /// Returns access token
        /// </summary>
        //public static string AccessToken
        //{
        //    get
        //    {
        //        return ACCESS_TOKEN;
        //    }
        //}

        /// <summary>
        /// The root path where DwgOperations app files are stored.
        /// </summary>
        public static string AppRootUri
        {
            get
            {
                return APP_ROOT_URI;
            }
        }


        #region XmlSection
        /// <summary>
        /// Loads the XML from dropbox.
        /// </summary>
        /// <remarks>
        /// https://content.dropboxapi.com/2/files/download?authorization=Bearer ACCESS_TOKEN&arg={"path": "/folder/filename.xml"}
        /// </remarks>
        /// <param name="svcUri">Service URI to XML file</param>
        /// <returns>Null if not exists or failed to load</returns>
        public XmlDocument LoadXml(string svcUri)
        {
            string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
            svcUri = svcUri.ToUri();
            XmlDocument xmlDoc = new XmlDocument();
            string uri = new Uri(string.Format("https://content.dropboxapi.com/2/files/download?authorization=Bearer%20{1}&arg=%7B%22path%22%3A%22{0}%22%7D", svcUri, AccessToken)).AbsoluteUri;

            try
            {
                xmlDoc.Load(uri);
                return xmlDoc;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion


        #region CreateSection
        /// <summary>
        /// Creates a folder within specified Dropbox relative directory where the
        /// last part of this URI is the folder name.
        /// </summary>
        /// <param name="svcUri">Dropbox folder URI</param>
        /// <returns>Metadata for the created folder</returns>
        public async Task<dropboxApi.Files.CreateFolderResult> CreateFolder(string svcUri)
        {

            dropboxApi.Files.CreateFolderResult result = null;
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;


                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Files.CreateFolderV2Async(svcUri);

                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<string> CreateFolder1(string svcUri)
        {

            string responseMessage = null;
            try
            {
                dropboxApi.Files.CreateFolderResult result = null;
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;


                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Files.CreateFolderV2Async(svcUri);
                    if (result.Metadata.Name != null)
                    {
                        responseMessage = "success";
                    }
                }

                return responseMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                responseMessage = ex.Message;

                return responseMessage;
            }
        }

        #endregion

        #region DeleteSection

        /// <summary>
        /// Deletes file or folder by the specified Dropbox URI.
        /// </summary>
        /// <param name="svcUri">File or Folder URI</param>
        /// <returns>Deleted file or folder metadata</returns>
        public async Task<dropboxApi.Files.DeleteResult> DeleteFileOrFolder(string svcUri)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                dropboxApi.Files.DeleteResult result = null;

                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Files.DeleteV2Async(svcUri);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion

        #region EditSection
        /// <summary>
        /// Renames file or folder.
        /// </summary>
        /// <param name="oldSvcUri">Dropbox URI for the file or folder to rename</param>
        /// <param name="newSvcUri">The Dropbox URI for new file or folder name</param>
        /// <returns>Renamed file or folder metadata</returns>
        public async Task<dropboxApi.Files.RelocationResult> RenameFileOrFolder(string oldSvcUri, string newSvcUri)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                dropboxApi.Files.RelocationResult result = null;

                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Files.MoveV2Async(oldSvcUri, newSvcUri, autorename: false);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region UploadSection
        /// <param name="sourceFile">Byte array to upload.</param>
        /// <param name="location">Location on dropbox.</param>
        /// <returns></returns>
        public  async Task ChunkUpload(string sourceFile, string location)
        {

            string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
            var client = new dropboxApi.DropboxClient(AccessToken);

            const int chunkSize = 1024 * 1024 * 10;

            using (FileStream stream = File.OpenRead(sourceFile))
            {
                int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);

                byte[] buffer = new byte[chunkSize];
                string sessionId = null;

                for (int idx = 0; idx < numChunks; idx++)
                {
                    Console.WriteLine($"{DateTime.Now} Start uploading chunk {idx}");
                    int byteRead = stream.Read(buffer, 0, chunkSize);

                    using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                    {
                        if (idx == 0)
                        {
                            UploadSessionStartResult result = await client.Files.UploadSessionStartAsync(body: memStream);
                            sessionId = result.SessionId;
                        }
                        else
                        {
                            UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                            if (idx == numChunks - 1)
                            {
                                await client.Files.UploadSessionFinishAsync(cursor, new CommitInfo(location, WriteMode.Overwrite.Instance), memStream);
                            }
                            else
                            {
                                await client.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                            }
                        }
                    }
                }
            }
        }










        /// <summary>
        /// Uploads the files from PC local directory to Dropbox folder.
        /// </summary>
        /// <remarks>
        /// When providing the 'filePaths' list where the file sub directory does not exist in Dropbox,
        /// new folder will be automatically created in Dropbox.
        /// </remarks>
        /// <param name="svcPath">Dropbox folder URI where to upload files (Example: /app/UploadFolder)</param>
        /// <param name="localDirPath">Path from where to upload files</param>
        /// <param name="filePaths">File URIs (Example: [ "/app/UploadFolder/file.jpg", "/app/UploadFolder/SubFolder/file2.jpg" ] etc.)</param>
        /// <returns>List with uploaded file metadata</returns>
        public async Task<List<dropboxApi.Files.FileMetadata>> UploadFiles(string svcPath, string localDirPath, List<string> filePaths)
        {
            string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
            var uploadResults = new List<dropboxApi.Files.FileMetadata>();

            try
            {
                if (filePaths == null || filePaths.Count() == 0)
                {
                    throw new ArgumentNullException("UploadFiles: files list was empty");
                }

                foreach (var relativePath in filePaths)
                {
                    string fullLocalDir = Path.Combine(localDirPath, relativePath);
                    string fullSvcUri = svcPath.ToUri() + relativePath.ToUri();

                    if (File.Exists(fullLocalDir))
                    {
                        using (var client = new dropboxApi.DropboxClient(AccessToken))
                        using (Stream fileStream = File.OpenRead(fullLocalDir))
                        {
                            var result = await client.Files.UploadAsync(fullSvcUri, body: fileStream);
                            uploadResults.Add(result);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException($"DropboxManager: File with name: {relativePath} does not exists!");
                    }
                }

                return uploadResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<dropboxApi.Files.FileMetadata> UploadMultipleFiles(string DropBoxFolderPath, string LocalSourcePath)
        {
            string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
            var uploadResults = new dropboxApi.Files.FileMetadata();
            var client = new dropboxApi.DropboxClient(AccessToken);
            try
            {
                    const int chunkSize = 1024 * 1024 * 10;
                    using (FileStream stream = File.OpenRead(LocalSourcePath))
                    {
                        int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);
                        byte[] buffer = new byte[chunkSize];
                        string sessionId = null;
                        for (int idx = 0; idx < numChunks; idx++)
                        {
                            Console.WriteLine($"{DateTime.Now} Start uploading chunk {idx}");
                            int byteRead = stream.Read(buffer, 0, chunkSize);
                            using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                            {
                                if (idx == 0)
                                {
                                    UploadSessionStartResult result = await client.Files.UploadSessionStartAsync(body: memStream);
                                    sessionId = result.SessionId;
                                }
                                else
                                {
                                    UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                                    if (idx == numChunks - 1)
                                    {
                                        await client.Files.UploadSessionFinishAsync(cursor, new CommitInfo(DropBoxFolderPath, WriteMode.Overwrite.Instance), memStream);
                                    }
                                    else
                                    {
                                        await client.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                                    }
                                }
                            }
                        }
                    
                }
                return uploadResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region GetSection
        /// <summary>
        /// Gets the folder shared link which can be used, for example, 
        /// to download folder as .zip archive.
        /// </summary>
        /// <param name="svcUri">Dropbox folder URI</param>
        /// <returns>Folder shared link</returns>
        public async Task<string> GetFolderSharedLink(string svcUri)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                dropboxApi.Sharing.ListSharedLinksResult result = null;

                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Sharing.ListSharedLinksAsync(svcUri, directOnly: true);
                    if (result.Links.Count == 0)
                    {
                        var sharedLinkMeta = await client.Sharing.CreateSharedLinkWithSettingsAsync(svcUri);
                        return sharedLinkMeta.Url;
                    }
                }

                return result.Links[0].Url;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Gets file or folder metadata (file size, creation time, extension etc.).
        /// </summary>
        /// <param name="svcUri">Dropbox URI for the file or folder</param>
        /// <returns>File or folder metadata</returns>
        public async Task<dropboxApi.Files.Metadata> GetFileOrFolderMetadata(string svcUri)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                dropboxApi.Files.Metadata result = null;

                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Files.GetMetadataAsync(svcUri);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all the files that are within Dropbox folder by given URI.
        /// </summary>
        /// <param name="svcFolderUri">Dropbox folder URI</param>
        /// <returns>Result that contains list with file metadata</returns>
        public async Task<dropboxApi.Files.ListFolderResult> GetFilesInFolder(string svcFolderUri)
        {

            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                dropboxApi.Files.ListFolderResult result = null;

                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    result = await client.Files.ListFolderAsync(svcFolderUri);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion


        #region DownloadSection

        /// <summary>
        /// Downloads a file by given Dropbox source URI to local directory.
        /// </summary>
        /// <param name="svcFileUri">Dropbox file URI</param>
        /// <param name="localFilePath">Local directory where to download file</param>
        /// <returns>True if file download went successfully, else False</returns>
        public async Task<bool> DownloadFile(string svcFileUri, string localFilePath)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    var result = await client.Files.DownloadAsync(svcFileUri);
                    using (Stream sourceStream = await result.GetContentAsStreamAsync())
                    using (FileStream source = File.Open(localFilePath, FileMode.Create))
                    {
                        await sourceStream.CopyToAsync(source);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<string> DownloadFile1(string svcFileUri, string localFilePath)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                using (var client = new dropboxApi.DropboxClient(AccessToken))
                {
                    var result = await client.Files.DownloadAsync(svcFileUri);
                    using (Stream sourceStream = await result.GetContentAsStreamAsync())
                    using (FileStream source = File.Open(localFilePath, FileMode.Create))
                    {
                        await sourceStream.CopyToAsync(source);
                    }
                }

                return "Successfully Download";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
        /// <summary>
        /// Downloads a folder as a .zip archive.
        /// </summary>
        /// <param name="svcUri">Dropbox folder URI</param>
        /// <param name="localFilePath">Local directory where to download the folder</param>
        /// <returns>True if folder download went successfully, else False</returns>
        public async Task<bool> DownloadFolder(string svcUri, string localFilePath)
        {
            try
            {
                string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
                string sharedFolderUrl = await this.GetFolderSharedLink(svcUri);
                string fullPath = Path.Combine(localFilePath, $"{Path.GetFileName(svcUri)}.zip");

                using (var webClient = new WebClient())
                {
                    await Task.Run(() =>
                    {
                        // dl=1 flag shows that folder will be downloaded as .zip archieve
                        webClient.DownloadFile(new Uri(sharedFolderUrl.Replace("dl=0", "dl=1")), fullPath);
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Downloads files or folders from given Dropbox URI to specified local directory.
        /// </summary>
        /// <param name="files">List with download file routes</param>
        /// <param name="CompletedCallback">Callback delegate that is called on operation completion</param>
        public async Task Download(List<FileRequestInfo> files, Action<bool, bool> CompletedCallback)
        {
            // Resets download files count
            string AccessToken = _IConfiguration.GetSection("DropBoxAccessToken").Value;
            FilesToDownloadCount = files.Count;

            foreach (var item in files)
            {
                Directory.CreateDirectory(item.GetLocalFileFullPathWithoutFileName());
                var fileMetadata = new FileMetadataInfo(item.GetLocalFileFullPath());
                FilesToDownload.Add(item.GetServiceFileFullUri(), fileMetadata);
            }

            for (int i = 0; i < FilesToDownload.Count; i++)
            {
                KeyValuePair<string, FileMetadataInfo> itemToDownload = FilesToDownload.ElementAt(i);
                bool isDownloadSuccessful;
                if (itemToDownload.Value.IsFile)
                {
                    isDownloadSuccessful = await DownloadFile(itemToDownload.Key, itemToDownload.Value.Name);
                }
                else
                {
                    isDownloadSuccessful = await DownloadFolder(itemToDownload.Key, itemToDownload.Value.Name);
                }

                CompletedCallback.Invoke(isDownloadSuccessful, false);
            }
        }

        #endregion

    }
}
