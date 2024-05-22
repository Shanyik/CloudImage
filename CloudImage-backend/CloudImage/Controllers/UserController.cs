using CloudImage.Data;
using CloudImage.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CloudImage.ImagesController;

[Route("api/[controller]")]
[ApiController]


public class UserController : ControllerBase
{
    private readonly UserManager<AppUser> userManager;
    private readonly SignInManager<AppUser> signInManager;
    private readonly AppDbContext _context;

    public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
    {
        _context = context;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }
    
    [HttpPost("add-user")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var apiKey = CreateApiKey();
        var user = new AppUser()
        {
            Email = model.Email,
            UserName = model.UserName,
            PasswordHash = model.Password,
            ApiKey = apiKey
        };
        var result = await userManager.CreateAsync(user, user.PasswordHash!);
        if (result.Succeeded)
        {
            apiKey.User = user;
            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync(); 
            return Ok("Registration made successfully");
        }
            
        return BadRequest("Error occured");
    }
    
    private ApiKey CreateApiKey()
    {
        return new ApiKey { Key = GenerateRandomApiKey(), AllocatedStorageGB = 3.0, UsedStorageGB = 0};
    }
    
    private static string GenerateRandomApiKey()
    {
        return Guid.NewGuid().ToString("N");
    }
}