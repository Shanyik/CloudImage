﻿using CloudImage.Service;
using Microsoft.AspNetCore.Mvc;

namespace CloudImage.ImagesController
{
    
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        // Get the base URL dynamically from the incoming request
        private readonly IApiKeyService _apiKeyService;
        private readonly string _baseUrl;

        public ImageController(IApiKeyService apiKeyService, IHttpContextAccessor httpContextAccessor)
        {
            _apiKeyService = apiKeyService;
            _baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext!.Request.Host}/images/";
        }


        [HttpGet("StatusCheck")]
        public Task<IActionResult> StatusCheck()
        {
            return Task.FromResult<IActionResult>(Ok("Connection is good."));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromHeader(Name = "ApiKey")] string apiKey)
        {
            
            // Check if the API key is provided
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("API key is missing.");
            }

            // Check if the provided API key is valid
            if (!_apiKeyService.IsValidApiKey(apiKey))
            {
                return Unauthorized("Invalid API key.");
            }
            
            if (!Request.HasFormContentType)
            {
                return BadRequest("Invalid request. Expected multipart form data.");
            }
            
            var files = Request.Form.Files;

            if (files.Count == 0)
            {
                return BadRequest("No images were uploaded.");
            }
            
            try
            {
                // Ensure that the Images directory exists, create it if it doesn't
                var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                var imageUrls = new List<string>();

                foreach (var file in files)
                {
                    // Generate a unique filename for the image
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Get the path where the image will be stored
                    var filePath = Path.Combine(imagesDirectory, uniqueFileName);
                    
                    // Save the image 
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Construct the URL for the uploaded image and add it to the response
                    var imageUrl = _baseUrl + uniqueFileName;
                    imageUrls.Add(imageUrl);
                }

                // Return URLs to store in a database
                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }
    }
}