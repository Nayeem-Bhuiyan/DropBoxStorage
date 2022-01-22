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
    public class CreateDropBoxController : Controller
    {
        

        private IDropboxManager _dropBoxService;

        public CreateDropBoxController(IDropboxManager dropBoxService)
        {
            _dropBoxService = dropBoxService;
        }

        public IActionResult CreateFolder()
        {
            CreateDropBoxViewModel model = new CreateDropBoxViewModel();


            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateFolder(CreateDropBoxViewModel model)
        {

           await _dropBoxService.CreateFolder(model.FolderName);
            return View();
        }




    }
}
