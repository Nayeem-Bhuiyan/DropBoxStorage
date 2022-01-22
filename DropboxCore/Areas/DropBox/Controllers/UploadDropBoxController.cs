using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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




    }
}
