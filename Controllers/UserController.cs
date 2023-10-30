using HumanAddictionServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HumanAddictionServer.Controllers;

[Authorize]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("user_data")]
    public IActionResult GetUserData()
    {
        // Получение данных пользователя из базы данных или другого источника.
         //var userData = _context.UserData.SingleOrDefault(u => u.UserId == UserId);
        // ...

        return Ok(new { UserData = "user_data_here" });
    }

    [HttpPost("user_data")]
    public IActionResult UpdateUserData([FromBody] UserDataViewModel model)
    {
        // Обновление данных пользователя в базе данных или другом источнике.
        // Пример: var user = _userManager.GetUserAsync(User).Result;
        //         user.UserDataField = model.UserDataField;
        //         _context.SaveChanges();
        // ...

        return Ok(new { Message = "User data updated successfully" });
    }
}
