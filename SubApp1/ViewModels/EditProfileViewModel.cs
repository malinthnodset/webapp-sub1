namespace SubApp1.ViewModels;

using SubApp1.Models;

// Profile overview, see your details - shared fields for student and teacher

public class EditProfileViewModel
{
    // send the role as parameter (student/teacher)
    public User User { get; set; } = null!;

    // User contains field: name, email, role, (password)
    // Teacher contains: courses
    // Student contains enrolled courses


    // change/update all fields + password
    // delete account

    // Student (optional): See stats
}