using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Data.Leyer.MyEntities;

public class Author
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }

    // Authentication properties
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }

    // Authorization properties
    // Navigation property for roles
    public ICollection<Role> Roles { get; set; }

    public ICollection<Permissions> Permissions { get; set; }

    // Foreign key for permission
    public int PermissionId { get; set; }
    public Permissions Permission { get; set; }
    // Foreign key for role
    public int RoleId { get; set; }
    public Role Role { get; set; }
    // Methods for token management
    public string GenerateJwtToken(string secretKey, string issuer, string audience, int accessTokenExpirationMinutes, IEnumerable<Claim> additionalClaims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, Username),
                // You can include additional claims as needed (e.g., roles, permissions)
            }.Concat(additionalClaims)),
            Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public string GenerateRefreshToken(int refreshTokenExpirationMinutes)
    {
        // Generate a random refresh token with expiration time
        var refreshToken = Guid.NewGuid().ToString();
        RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(refreshTokenExpirationMinutes);
        return refreshToken;
    }
    public bool ValidateAccessToken(string token, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // Set to true if you want to validate the issuer
                ValidateAudience = false, // Set to true if you want to validate the audience
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Adjust the tolerance for expiration time validation (optional)
            }, out _);
            return true;
        }
        catch
        {
            // Token validation failed
            return false;
        }
    }
    public string RefreshAccessToken(string refreshToken, string secretKey, string issuer, string audience, int accessTokenExpirationMinutes)
    {
        // Check if the refresh token is valid and not expired
        if (DateTime.UtcNow > RefreshTokenExpiration)
        {
            throw new Exception("Refresh token has expired.");
        }

        // Generate a new JWT access token with the same claims as the original token
        var accessToken = GenerateJwtToken(secretKey, issuer, audience, accessTokenExpirationMinutes, new List<Claim>());
        return accessToken;
    }
}