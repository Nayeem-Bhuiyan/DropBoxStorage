using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Service;
using DropboxCore.Services;
using Microsoft.AspNetCore.Hosting;
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
        private IUploadService _uploadService;
        private IWebHostEnvironment _environment;
        public UploadDropBoxController(IDropboxManager dropBoxService, IWebHostEnvironment environment, IUploadService uploadService)
        {
            _dropBoxService = dropBoxService;
            _environment = environment;
            _uploadService = uploadService;
        }

        public IActionResult Upload()
        {
            UploadDropBoxViewModel model = new UploadDropBoxViewModel();


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm]UploadDropBoxViewModel model)
        {

            List<string> ListPath = new List<string>();
            foreach (var file in model.files)
            {
                
                string fileName = Path.GetFileName(file.FileName);
                string FullPath = Path.Combine(_environment.WebRootPath, "Upload", fileName);
                ListPath.Add(FullPath);
                var inputStream = file.OpenReadStream();
   
                using (var fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
                {
                    inputStream.CopyTo(fileStream);
                }

                var localDirectory = Path.Combine(_environment.WebRootPath,"Upload");

                await _uploadService.UploadToDropBoxAsync(localDirectory, FullPath);
              
            }
            
            foreach (var filePath in ListPath)
            {
                await _dropBoxService.UploadMultipleFiles("/Upload-22-01-2022", filePath);
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

                foreach (var file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload")))
                {
                    string fullFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload");

                    await _dropBoxService.ChunkUpload(file, $"/Upload-22-01-2022");
                }


            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return View(model);
        }






    }
}
