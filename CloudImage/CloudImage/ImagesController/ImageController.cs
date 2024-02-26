using Microsoft.AspNetCore.Mvc;

namespace CloudImage.ImagesController
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly string _imageBaseUrl; 

        public ImageController(IHttpContextAccessor httpContextAccessor)
        {
            // Get the base URL dynamically from the incoming request
            _imageBaseUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/images/";
        }

        public class ImagesArray
        {
            public List<IFormFile> Images { get; set; }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] ImagesArray? files)
        {
            if (files == null || files.Images.Count == 0)
            {
                return BadRequest("No files uploaded.");
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

                foreach (var file in files.Images)
                {
                    // Generate a unique filename for the image
                    var uniqueFileName = GenerateRandomString(20) + Path.GetExtension(file.FileName);

                    // Get the path where the image will be stored
                    var filePath = Path.Combine(imagesDirectory, uniqueFileName);

                    // Save the image 
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Construct the URL for the uploaded image
                    var imageUrl = _imageBaseUrl + uniqueFileName;
                    imageUrls.Add(imageUrl);
                }

                // Return URLs to store in a database
                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}