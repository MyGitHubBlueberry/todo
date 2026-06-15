namespace Server.Services;

using Server.Core.Dtos.Auth;
using Server.Core.Interfaces;
using Server.Core.Models;
using Server.Data;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
{
    public async Task<TokensDto> LoginAsync(LoginDto loginDto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == loginDto.login)
            ?? throw new InvalidDataException("Invalid login: No user with this username exists.");

        if (!BCrypt.Verify(loginDto.password, user.Password))
            throw new InvalidDataException("Invalid login: Password is invalid.");

        var tokens = new TokensDto(GenerateJwtToken(user), GenerateRefreshToken());

        user.RefreshToken = tokens.refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        return tokens;
    }

    public async Task<TokensDto> RefreshTokenAsync(TokensDto tokensDto)
    {
        var username = GetPrincipalFromExpiredToken(tokensDto.accessToken).Identity?.Name;
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Username == username
                    && u.RefreshToken == tokensDto.refreshToken);

        if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidDataException("Invalid client request: Refresh token is missing or expired.");
        }

        var tokens = new TokensDto(GenerateJwtToken(user), GenerateRefreshToken());
        user.RefreshToken = tokens.refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync();

        return tokens;
    }

    public async Task<TokensDto> RegisterAsync(RegistrationDto registrationDto)
    {
        if (await db.Users.AnyAsync(u => u.Username == registrationDto.login))
            throw new InvalidDataException("Registration failed: This username is already in use.");

        var pwd = BCrypt.HashPassword(registrationDto.password);
        var user = new User
        {
            Username = registrationDto.login,
            Password = pwd
        };

        await db.Users.AddAsync(user);

        await db.SaveChangesAsync();

        var tokens = new TokensDto(GenerateJwtToken(user), GenerateRefreshToken());
        user.RefreshToken = tokens.refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync();

        return tokens;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT Secret Key is missing from configuration.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var token = new byte[64];
        RandomNumberGenerator.Create().GetBytes(token);
        return Convert.ToBase64String(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
