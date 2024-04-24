using CloudImage.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CloudImage.ImagesController
{
    
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly string _baseUrl;
        private readonly string _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");

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
        
        [HttpGet("CheckStorage")]
        
        public IActionResult CheckStorage()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            double availableSpaceGb = 0;
            double totalSpaceGb = 0;
                
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    availableSpaceGb = (double)drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                    totalSpaceGb = (double)drive.TotalSize / (1024 * 1024 * 1024);
                    
                }
                else
                {
                    return Ok("No driver.");
                }
            }

            return Ok($"Available space: {availableSpaceGb:N2} GB / {totalSpaceGb:N2} GB");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromHeader(Name = "ApiKey")] string apiKey)
        {

            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("API key is missing.");
            }
            
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
                if (!Directory.Exists(_imagesDirectory))
                {
                    Directory.CreateDirectory(_imagesDirectory);
                }

                var imageUrls = new List<string>();
                
                long totalUploadedSize = files.Sum(file => file.Length);
                
                double remainingStorage = _apiKeyService.GetRemainingStorage(apiKey);
                if (totalUploadedSize > remainingStorage * 1024 * 1024) // Convert GB to bytes
                {
                    return BadRequest("Upload exceeds remaining storage limit.");
                }

                foreach (var file in files)
                {
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    
                    var filePath = Path.Combine(_imagesDirectory, uniqueFileName);
                    
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        
                    }
                    
                    _apiKeyService.UpdateUsedStorage(apiKey, file.Length);

                    var imageUrl = _baseUrl + uniqueFileName;
                    imageUrls.Add(imageUrl);
                }
                
                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(IList<string> urlList)
        {
            try
            {
                foreach (var url in urlList)
                {
                    var fileName = GetFileNameFromUrl(url);
                    var filePath = Path.Combine(_imagesDirectory, fileName);
                
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("GenerateKey")]
        public IActionResult GenerateApiKey()
        {
            string newApiKey = GenerateRandomApiKey();
            
            _apiKeyService.AddApiKey(newApiKey);

            return Ok(new { ApiKey = newApiKey });
        }

        private string GenerateRandomApiKey()
        {
            return Guid.NewGuid().ToString("N");
        }
        
        private string GetFileNameFromUrl(string url)
        {
            return url.Split('/').Last(); 
        }
    }
}