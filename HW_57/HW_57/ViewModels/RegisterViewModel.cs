using System.ComponentModel.DataAnnotations;

namespace HW_57.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Enter username")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Enter email")]
    [EmailAddress(ErrorMessage = "Enter correct email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = "";
}