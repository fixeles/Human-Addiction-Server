using HumanAddictionServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HumanAddictionServer.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, 
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var existingUser = await _userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
            return BadRequest(new { Message = "User with this name already exists" });
    
        existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest(new { Message = "User with this email already exists" });
    
        var user = new ApplicationUser
        {
            UserName = model.UserName, Email = model.Email
        };
    
        var result = await _userManager.CreateAsync(user, model.Password);
    
        if (result.Succeeded)
        {
            // В этом месте вы можете выполнить дополнительные действия после успешной регистрации,
            // например, создание профиля пользователя в базе данных.
            // ...
    
            return Ok(new { Message = "Registration successful" });
        }
    
        return BadRequest(new { Message = "Registration failed", result.Errors });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // В этом месте вы можете создать и возвратить JWT-токен для пользователя.
            // ...

            return Ok(new { Token = "your_generated_token", Message = "Login successful" });
        }

        return BadRequest(new { Message = "Login failed", Error = "Invalid username or password" });
    }
}