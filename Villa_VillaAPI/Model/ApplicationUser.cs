using Microsoft.AspNetCore.Identity;

namespace Villa_VillaAPI.Model;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}
