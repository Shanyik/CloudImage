using CloudImage.Model;
using CloudImage.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CloudImage.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IApiRepository _apiRepository;
        private readonly IImageRepository _imageRepository;
        private readonly string _baseUrl;
        private readonly string _imagesDirectory;
        private readonly int _maxSlots = 10;

        public ApiController(IHttpContextAccessor httpContextAccessor, IApiRepository apiRepository, IImageRepository imageRepository)
        {
            _apiRepository = apiRepository;
            _imageRepository = imageRepository;
            _baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext!.Request.Host}/images/";
            _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var apiKeys = await _apiRepository.GetAll();
                if (!apiKeys.Any())
                {
                    return NotFound();
                }

                return Ok(apiKeys);
            }
            catch (Exception e)
            {
                return BadRequest("not found");
            }
        }
        
        [HttpGet("StatusCheck")]
        public IActionResult StatusCheck()
        {
            return Ok("Connection is good.");
        }
        
        [HttpGet("CheckAllStorage")]
        
        public IActionResult CheckAllStorage()
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
        public async Task<IActionResult> GetApiKeyInfo(string key)
        {

            try
            {
                var apiKey = await _apiRepository.GetByKey(key);
                if (apiKey == null)
                {
                    return NotFound("ApiKey not found in database.");
                }
                return Ok(apiKey);
            }
            catch (Exception e)
            {
                return BadRequest("Error connecting to the database! Try again later!");
            }
            
            
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromHeader(Name = "ApiKey")] string key)
        {

            var apiKey = await _apiRepository.GetByKey(key);
            if (apiKey == null)
            {
                return BadRequest("No / Invalid Apikey!");
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
                var remainingStorage = apiKey.AllocatedStorageGB - apiKey.UsedStorageGB;
                if (totalUploadedSize / 1024 / 1024 / 1024 > remainingStorage)
                {
                    return BadRequest("Upload exceeds remaining storage limit.");
                }

                apiKey.UsedStorageGB += totalUploadedSize / 1024 / 1024 / 1024;
                
                await _apiRepository.Update(apiKey);

                foreach (var file in files)
                {
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_imagesDirectory, uniqueFileName);
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    var imageUrl = _baseUrl + uniqueFileName;
                    
                    
                    
                    var image = new Image
                    {
                        ImageUrl = imageUrl,
                        Byte = file.Length,
                        ApiKeyId = apiKey.Id
                    };
                    
                    await _imageRepository.Add(image);
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
        public async Task<IActionResult> Delete([FromHeader(Name = "ApiKey")] string key, IList<string> urlList)
        {
            
            var apiKey = await _apiRepository.GetByKey(key);
            if (apiKey == null)
            {
                return BadRequest("No / Invalid Apikey!");
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
                        await _imageRepository.Delete(url);
                    }
                }
                apiKey.UsedStorageGB -= totalDeletedSize / (1024 * 1024 * 1024);
                await _apiRepository.Update(apiKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpGet("remainingSlots")]
        public async Task<IActionResult> GetRemainingSlots()
        {
            
            var apiKeyList = await _apiRepository.GetAll();
            return Ok(_maxSlots - apiKeyList.Count());
        }
        
        private string GetFileNameFromUrl(string url)
        {
            return url.Split('/').Last(); 
        }
    }
}