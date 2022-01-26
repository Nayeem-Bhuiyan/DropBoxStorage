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
using System.Net.Http;
using System.Threading;
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
        [RequestFormLimits(MultipartBodyLengthLimit = 5368709120)]
        [RequestSizeLimit(5368709120)]
        public async Task<IActionResult> Upload([FromForm] UploadDropBoxViewModel model)
        {
            
            try
            {
                foreach (var file in model.files)
                {

                    string fileName = Path.GetFileName(file.FileName);
                    string FullPath = Path.Combine(_environment.WebRootPath, "Upload", fileName);
                    var inputStream = file.OpenReadStream();
                    using (var fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
                    {
                        inputStream.CopyTo(fileStream);
                    }
                    await _uploadService.UploadToDropBoxAsync("Upload-22-01-2022", fileName, FullPath);

                }
                model.responseMessage = "success";
            }
            catch (Exception ex)
            {
                model.responseMessage = ex.Message;
                throw;
            }


            return Json(model);
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

                //List<string> LocalSoucePathList = new List<string>();

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
                        //LocalSoucePathList.Add(fullSourcePath);
                        await _uploadService.ChunkUpload(fullSourcePath, $"/Upload-22-01-2022",fileName);
                    }


                }

                //foreach (var localSoucePath in LocalSoucePathList)
                //{
                    
                   

                //}
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return View(model);
        }






    }
}
