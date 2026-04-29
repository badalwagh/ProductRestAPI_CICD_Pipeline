using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewWebAPICore.Service;
using System;
using System.Collections.Generic;

namespace NewWebAPICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadedController : ControllerBase
    {
        private readonly BlobService _blobService;

        public FileUploadedController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded");

            List<string> urls = new List<string>();

            foreach (var file in files)
            {
                var url = await _blobService.UploadFileAsync(file);
                urls.Add(url);
            }

            return Ok(urls);
        }
    }
}
