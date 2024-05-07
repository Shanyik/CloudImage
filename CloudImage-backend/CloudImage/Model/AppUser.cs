using Microsoft.AspNetCore.Identity;

namespace CloudImage.Model;

public class AppUser : IdentityUser
{
    public ApiKey ApiKey;
}