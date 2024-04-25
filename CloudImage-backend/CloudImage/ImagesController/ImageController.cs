using CloudImage.Service;
using Microsoft.AspNetCore.Mvc;

namespace CloudImage.ImagesController
{
    
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly string _baseUrl;
        private readonly string _imagesDirectory;

        public ImageController(IApiKeyService apiKeyService, IHttpContextAccessor httpContextAccessor)
        {
            _apiKeyService = apiKeyService;
            _baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext!.Request.Host}/images/";
            _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }


        [HttpGet("StatusCheck")]
        public IActionResult StatusCheck()
        {
            return Ok("Connection is good.");
        }
        
        [HttpGet("CheckStorage")]
        
        public IActionResult CheckStorage()
        {
            var allDrives = DriveInfo.GetDrives();
            foreach (var drive in allDrives)
            {
                if (drive.IsReady)
                {
                    var availableSpaceGb = (double)drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                    var totalSpaceGb = (double)drive.TotalSize / (1024 * 1024 * 1024);
                    return Ok($"Available space: {availableSpaceGb:N2} GB / {totalSpaceGb:N2} GB");
                }
            }
            return Ok("No driver.");
        }
        
        [HttpGet("apiKeyInfo")]
        public IActionResult GetApiKeyInfo( string apiKey)
        {
            IActionResult validationResult = ValidateApiKeyAndRequest(apiKey);
            if (ValidateApiKeyAndRequest(apiKey) != null)
            {
                return validationResult;
            }

            var apiKeyInfo = _apiKeyService.GetApiKeyInfo(apiKey);
            return Ok(apiKeyInfo);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromHeader(Name = "ApiKey")] string apiKey)
        {

            IActionResult validationResult = ValidateApiKeyAndRequest(apiKey);
            if (ValidateApiKeyAndRequest(apiKey) != null)
            {
                return validationResult;
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
                double totalUploadedSize = files.Sum(file => file.Length);
                var remainingStorage = _apiKeyService.GetRemainingStorage(apiKey);
                if (totalUploadedSize / 1024 / 1024 / 1024 > remainingStorage)
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
        public IActionResult Delete([FromHeader(Name = "ApiKey")] string apiKey, IList<string> urlList)
        {
            
            IActionResult validationResult = ValidateApiKeyAndRequest(apiKey);
            if (ValidateApiKeyAndRequest(apiKey) != null)
            {
                return validationResult;
            }
            
            try
            {
                double totalDeletedSize = 0;
                foreach (var url in urlList)
                {
                    var fileName = GetFileNameFromUrl(url);
                    var filePath = Path.Combine(_imagesDirectory, fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        var fileInfo = new FileInfo(filePath);
                        totalDeletedSize += fileInfo.Length;
                        System.IO.File.Delete(filePath);
                    }
                }
                _apiKeyService.UpdateUsedStorage(apiKey, - totalDeletedSize);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpGet("remainingSlots")]
        public IActionResult GetRemainingSlots()
        {
            int remainingSlots = _apiKeyService.GetRemainingSlots();
            return Ok(remainingSlots);
        }
        
        [HttpPost("GenerateKey")]
        public IActionResult GenerateApiKey()
        {
            if (_apiKeyService.GetRemainingSlots() == 0)
            {
                return BadRequest("No available slots.");
            }
            
            var newApiKey = GenerateRandomApiKey();
            _apiKeyService.AddApiKey(newApiKey);
            return Ok(new { ApiKey = newApiKey });
        }
        
        private IActionResult ValidateApiKeyAndRequest(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("API key is missing.");
            }

            if (!_apiKeyService.IsValidApiKey(apiKey))
            {
                return Unauthorized("Invalid API key.");
            }

            return null; // Indicates validation success
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