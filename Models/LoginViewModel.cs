using System.ComponentModel.DataAnnotations;
namespace HumanAddictionServer.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Key]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}