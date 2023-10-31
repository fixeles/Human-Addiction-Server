using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HumanAddictionServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace HumanAddictionServer.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;

        var s1 = context.Database.GetDbConnection().State;
        if (s1 == System.Data.ConnectionState.Closed)
            context.Database.OpenConnection();

        context.Database.EnsureCreated();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var existingUser = await _userManager.FindByNameAsync(model.Email);
        if (existingUser != null)
            return BadRequest(new { Message = "User with this name already exists" });

        var user = new ApplicationUser
        {
            UserName = model.Email, Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // В этом месте вы можете выполнить дополнительные действия после успешной регистрации,
            // например, создание профиля пользователя в базе данных.

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
            var user = await _userManager.FindByNameAsync(model.Email);
            var token = GenerateJwtToken(user!);

            return Ok(new { Token = token, Message = "Login successful" });
        }

        return BadRequest(new { Message = "Login failed", Error = "Invalid username or password" });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
            // Другие клеймы, которые вы хотите добавить в токен
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key));
        var token = new JwtSecurityToken(
            JwtSettings.Issuer,
            JwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}