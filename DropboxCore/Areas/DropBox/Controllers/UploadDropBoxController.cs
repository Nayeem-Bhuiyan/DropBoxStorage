using Dropbox.Api.Files;
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

        [RequestFormLimits(MultipartBodyLengthLimit = 85899345920)]
        [RequestSizeLimit(85899345920)]
        public async Task<IActionResult> Upload([FromForm] UploadDropBoxViewModel model)
        {
            
            try
            {

              


                foreach (var file in model.files)
                {

                    string fileName = Path.GetFileName(file.FileName);
                    string FullPath = Path.Combine(_environment.WebRootPath, "Upload", fileName);
                    string DropBoxUploadPath = "/" + "Upload-22-01-2022" + "/" + fileName;
                    if (file.Length > 0)
                    {
                        var inputStream = file.OpenReadStream();
                        using (var fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
                        {
                            inputStream.CopyTo(fileStream);
                        }

                        //if (file.Length < 134000000)
                        //{
                        //    await _uploadService.UploadToDropboxSmallFileSystem(@"/Upload-22-01-2022",FullPath, fileName);
                        //    //await _uploadService.UploadToDropBoxSmallFile(FullPath, "/" + "Upload-22-01-2022" + "/"+fileName);
                        //}
                        //else
                        //{
                        //    await _uploadService.LargeFileUpload("Upload-22-01-2022", fileName, FullPath);
                        //}

                        await _uploadService.UploadToDropBox(FullPath,DropBoxUploadPath);
                    }
          
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

        public static Byte[] ToByteArray(Stream stream)
        {
            Int32 length = stream.Length > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(stream.Length);
            Byte[] buffer = new Byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

    }
}
