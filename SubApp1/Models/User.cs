namespace SubApp1.Models;

using System.ComponentModel.DataAnnotations;

// abstract class for shared attributed between student and teacher

public abstract class User
{
    private int Id; // set by DB

    [Required(ErrorMessage = "Firstname cannot be empty")]
    private String FirstName { get; set; }

    [Required(ErrorMessage = "Last name cannot be empty")]
    private String LastName { get; set; }

    [Required(ErrorMessage = "Email cannot be empyt"), EmailAddress]
    private String Email { get; set; }

    // password

    [Required(ErrorMessage = "Choose a role")]
    private Role Role { get; set; }

    // TODO: include study program or faculty?
}

public enum Role
{
    STUDENT,
    TEACHER
}