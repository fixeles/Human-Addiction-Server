using System.ComponentModel.DataAnnotations;
namespace HumanAddictionServer.Models;

public class UserDataViewModel
{
    [Key]
    public string UserId { get; set; } = string.Empty;

    public string UserData { get; set; } = string.Empty;
}