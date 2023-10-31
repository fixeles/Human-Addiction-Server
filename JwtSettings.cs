using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace HumanAddictionServer;

public static class JwtSettings
{
    private static IConfigurationSection? _configurationSection;
    public static string Issuer => _configurationSection!["ValidIssuer"]!;
    public static string Audience => _configurationSection!["ValidAudience"]!;
    public static string Key => _configurationSection!["IssuerSigningKey"]!;

    public static SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes(Key));

    public static void Init(IConfigurationSection? configurationSection)
    {
        _configurationSection = configurationSection;
    }
}