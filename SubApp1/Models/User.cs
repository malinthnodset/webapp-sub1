using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

// abstract class for shared attributed between student and teacher

public abstract class User
{
    public int Id { get; set; } // set by DB

    [Required(ErrorMessage = "Name cannot be empty")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email cannot be empyt"), EmailAddress]
    public string Email { get; set; } = string.Empty;

    // password

    [Required(ErrorMessage = "Choose a role")]
    public Role Role { get; set; }

    // TODO: include study program or faculty?
}

public enum Role
{
    STUDENT,
    TEACHER
}