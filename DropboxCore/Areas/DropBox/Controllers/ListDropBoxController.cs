using DropboxCore.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Controllers
{
    public class ListDropBoxController : Controller
    {

        private IDropboxManager _dropBoxService;

        public ListDropBoxController(IDropboxManager dropBoxService)
        {
            _dropBoxService = dropBoxService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
