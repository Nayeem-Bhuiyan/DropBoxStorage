using Dropbox.Api;
using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Controllers
{
    [Area("DropBox")]
    public class DownloadDropBoxController : Controller
    {

        private IDropboxManager _dropBoxService;

        public DownloadDropBoxController(IDropboxManager dropBoxService)
        {
            _dropBoxService = dropBoxService;
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


    }
}
