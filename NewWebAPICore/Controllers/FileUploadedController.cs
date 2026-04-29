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

        // Upload file to blob storage
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is required");

                var fileUrl = await _blobService.UploadFileAsync(file);

                return Ok(new
                {
                    success = true,
                    message = "File uploaded successfully",
                    fileUrl = fileUrl,
                    fileName = file.FileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // Delete file from blob storage
        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            try
            {
                await _blobService.DeleteFileAsync(fileName);
                return Ok(new
                {
                    success = true,
                    message = "File deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
