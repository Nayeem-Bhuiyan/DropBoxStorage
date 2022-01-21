using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropboxCore.Areas.DropBox.Controllers
{
    public class CreateDropBoxController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
