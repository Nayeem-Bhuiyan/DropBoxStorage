using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Controllers
{
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
        public IActionResult DownloadZip(DownloadDropBoxViewModel model)
        {
            
            return View(model);
        }

        [HttpGet]
        public IActionResult DownloadFiles()
        {
           
            return View();
        }




    }
}
