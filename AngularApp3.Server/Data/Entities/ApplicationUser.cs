using Microsoft.AspNetCore.Identity;

namespace AngularApp3.Server.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Cart? Cart { get; set; }
}
