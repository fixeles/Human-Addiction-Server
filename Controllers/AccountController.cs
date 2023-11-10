using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using HumanAddictionServer.Models;
using Microsoft.AspNetCore.Authorization;
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
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(new { Message = "Registration failed", result.Errors });

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);
        if (confirmationLink != null)
            await SendConfirmationEmailAsync(model.Email, confirmationLink);
        return Ok(new { Message = "Registration successful. Confirmation email sent." });
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
    {
        if (userId == null || token == null)
            return BadRequest(new { Message = "Invalid confirmation link" });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest(new { Message = "User not found" });

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return Ok(new { Message = "Email confirmation successful" });

        return BadRequest(new { Message = "Email confirmation failed", result.Errors });
    }

    private async Task SendConfirmationEmailAsync(string email, string confirmationLink)
    {
        const string smtpHost = "smtp.gmail.com";
        const int smtpPort = 587;
        const string smtpUsername = "VnA.Games.HA@gmail.com";
        const string smtpPassword = "xcoi huck hiqe buki";

        using var client = new SmtpClient(smtpHost, smtpPort);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        client.EnableSsl = true;

        var message = new MailMessage
        {
            Subject = "Confirm your email",
            Body = $"Please confirm your email by clicking on the link: {confirmationLink} ",
            IsBodyHtml = false,
            From = new MailAddress(smtpUsername, "Human Addiction"),
            To = { new MailAddress(email) }
        };

        await client.SendMailAsync(message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Email);

        if (user == null)
            return BadRequest(new { Message = "Login failed", Error = "Invalid username or password" });
        
        if (!await _userManager.IsEmailConfirmedAsync(user))
            return BadRequest(new { Message = "Login failed", Error = "Email not confirmed" });
        
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return BadRequest(new { Message = "Login failed", Error = "Invalid username or password" });
        
        return Ok(new { Token = GenerateJwtToken(user), Message = "Login successful" });
    }


    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
            //other claims
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