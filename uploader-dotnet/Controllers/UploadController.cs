using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Azure.Uploader.Models;
using Azure.Uploader.Services;

namespace Azure.Uploader.Controllers
{
    /// <summary>
    /// Upload User-Agents files that are published to Azure blob storage.
    /// </summary>
    public class UploadController : Controller
    {
        #region Instance variables

        private IUploadService _uploadService;

        #endregion

        #region Controller methods

        /// <summary>
        /// Constructor for the Upload Controlled form
        /// </summary>
        /// /// <param name="iConfig"> Pull the values required from AppSettings
        /// e.g Azure connection string, SMTP settings </param>
        public UploadController(IUploadService uploadService)
        {
			_uploadService = uploadService;
        }

        /// <summary>
        /// Returns the default view for the upload page
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Sends a post request using the form supplied in the view
        /// </summary>
        /// <param name="uploadForm">The data from the form captured in the model</param>
        [HttpPost("[action]")]
        [RequestSizeLimit(250_000_000)]
        public ViewResult Post(Upload uploadForm)
        {
            // Confirm the validation was successful before using the form data
            if (ModelState.IsValid)
            {
				// Collate any responses to display on the form
				var responses = _uploadService.ProcessRequest(uploadForm).Result;
				// Format the responses
				string response = string.Join("<br /> ", responses.ToArray());
				// Send the response to the view bag
				ViewBag.Response = response;
			}
            return View("Index");
        }

        #endregion

    }
}