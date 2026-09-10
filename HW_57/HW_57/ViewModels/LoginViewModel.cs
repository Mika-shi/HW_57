using System.ComponentModel.DataAnnotations;

namespace HW_57.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Enter email")]
    [EmailAddress(ErrorMessage = "Enter correct email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}