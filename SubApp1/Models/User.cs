using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SubApp1.Models;

public class User : IdentityUser<int>
{
    [Required(ErrorMessage = "Firstname cannot be empty")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email cannot be empty"), EmailAddress]
    public override string? Email { get; set; }
}

public enum Role
{
    STUDENT,
    TEACHER
}