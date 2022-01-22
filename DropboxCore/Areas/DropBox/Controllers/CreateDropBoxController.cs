using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dropboxApi = global::Dropbox.Api;
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
            try
            {
                dropboxApi.Files.CreateFolderResult response =await _dropBoxService.CreateFolder("/" + model.FolderName+ DateTime.Now.ToString("dd-MM-yyyy"));
                if (response!=null)
                {
                    model.message = "Successfully Folder Created";
                    model.FolderLink = "https://www.dropbox.com/home/" + model.FolderName + DateTime.Now.ToString("dd-MM-yyyy");
                }
                else
                {
                    model.message = "Duplicate Folder Found";
                    model.FolderLink = "https://www.dropbox.com/home/" + model.FolderName + DateTime.Now.ToString("dd-MM-yyyy");

                }
            }
            catch (Exception ex)
            {
                model.message = ex.Message;
                throw;
            }
            
            return View(model);
        }




    }
}
