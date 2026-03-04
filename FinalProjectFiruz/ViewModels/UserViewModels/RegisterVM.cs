using System.ComponentModel.DataAnnotations;

namespace FinalProjectFiruz.ViewModels.UserViewModels;

public class RegisterVM
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(2)]
    public string Fullname { get; set; } = string.Empty;

    [Required, MinLength(2)]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

}
