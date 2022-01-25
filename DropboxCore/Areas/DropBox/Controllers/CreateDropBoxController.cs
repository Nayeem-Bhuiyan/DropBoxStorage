using DropboxCore.Areas.DropBox.Models;
using DropboxCore.Service.Interface;
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

                string dropboxFolderPath = "/" + model.FolderName+"-" + DateTime.Now.ToString("dd-MM-yyyy");
                string response =await _dropBoxService.CreateFolder1(dropboxFolderPath);
                if (response== "success")
                {
                    model.message = "Successfully Folder Created";
                    model.FolderLink = "https://www.dropbox.com/home"+ dropboxFolderPath;
                }
                else
                {
                    model.message = "Duplicate Folder Found";
                    model.FolderLink = "https://www.dropbox.com/home" + dropboxFolderPath;
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
