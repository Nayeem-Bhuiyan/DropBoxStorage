using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Controllers
{
    [Area("DropBox")]
    public class UploadDropBoxController : Controller
    {


        private IDropboxManager _dropBoxService;

        public UploadDropBoxController(IDropboxManager dropBoxService)
        {
            _dropBoxService = dropBoxService;
        }

        public IActionResult Upload()
        {
            UploadDropBoxViewModel model = new UploadDropBoxViewModel();


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm]UploadDropBoxViewModel model)
        {


            foreach (var file in model.files)
            {
                string fullPath = Path.GetFullPath(file.FileName);
                await _dropBoxService.UploadMultipleFiles("/Upload-22-01-2022", fullPath);

            }
           
            return View(model);
        }

        public IActionResult UploadChunkFile()
        {
            UploadDropBoxViewModel model = new UploadDropBoxViewModel();
            return View(model);
        }

            [HttpPost]
        public async Task<IActionResult> UploadChunkFile([FromForm]UploadDropBoxViewModel model)
        {

            try
            {

                foreach (var file in model.files)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    
                    string fullSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload", fileName);
                    if (System.IO.File.Exists(fullSourcePath))
                    {
                        System.IO.File.Delete(fullSourcePath);
                    }
                    using (var localFile = System.IO.File.OpenWrite(fullSourcePath)) 
                    using (var uploadedFile =file.OpenReadStream())
                    {
                        uploadedFile.CopyTo(localFile);
                    }


                }
                string fullFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload");
                string finalPath = $"{fullFolderPath}.zip";
                FileInfo zip = new FileInfo(finalPath);
                if (!zip.Exists)
                {
                    ZipFile.CreateFromDirectory(model.fileSourcePath, finalPath);
                }

                await _dropBoxService.ChunkUpload(zip.FullName, $"/{zip.Name}");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return View(model);
        }






    }
}
