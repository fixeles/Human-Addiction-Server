using Microsoft.AspNetCore.Identity;
namespace HumanAddictionServer.Models;

public class ApplicationUser : IdentityUser
{
     public string UserData = string.Empty;
}