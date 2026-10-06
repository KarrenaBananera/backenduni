using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AngularApp3.Server.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace AngularApp3.Server.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public UserRole Role { get; set; } = UserRole.Customer;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Cart? Cart { get; set; }
}
