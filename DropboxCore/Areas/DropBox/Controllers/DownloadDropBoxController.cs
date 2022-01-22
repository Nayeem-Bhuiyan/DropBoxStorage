﻿using Dropbox.Api;
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
        public async Task<IActionResult> DownloadZip(DownloadDropBoxViewModel model)
        {
            try
            {
                bool response = false;
                response = await _dropBoxService.DownloadFolder(model.dropboxFolderPath, model.localFolderPath);
                if (response)
                {
                    model.message = "Success";
                }
                else
                {
                    model.message = "Error";
                }
            }
            catch (Exception ex)
            {
                model.message = ex.Message;
                throw;
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult DownloadFiles()
        {
            DownloadDropBoxViewModel model = new DownloadDropBoxViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DownloadFiles(DownloadDropBoxViewModel model)
        {
            try
            {
                string response = null;
                response = await _dropBoxService.DownloadFile1(model.dropboxFolderPath, model.localFolderPath);

                model.message = response;

            }
            catch (Exception ex)
            {

                throw;
            }
            return View(model);
        }

    }
}
