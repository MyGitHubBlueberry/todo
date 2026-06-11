namespace Server.Services;

using Server.Core.Interfaces;
using Server.Core.Models;
using Server.Data;

using BCrypt.Net;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
{
    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username)
            ?? throw new InvalidDataException("Invalid login: No user with this username exists.");

        if (!BCrypt.Verify(password, user.Password))
            throw new InvalidDataException("Invalid login: Password is invalid.");

        return GenerateJwtToken(user);
    }

    public async Task<User> RegisterAsync(string username, string password)
    {
        if (await db.Users.AnyAsync(u => u.Username == username))
            throw new InvalidDataException("Registration failed: This username is already in use.");

        var pwd = BCrypt.HashPassword(password);
        var user = new User
        {
            Username = username,
            Password = pwd
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        return user;
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
}
