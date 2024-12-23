using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username, List<string> permissions)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim("permission", string.Join(",", permissions)) // Add permissions claim
    };
        var secretKey = _configuration.GetValue<string>("SecretKey");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "default-issuer",  
            audience: "default-audience",
            claims: claims,
            expires: DateTime.Now.AddHours(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
