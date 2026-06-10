namespace Server.Services;

using Server.Core.Interfaces;
using Server.Core.Models;
using Server.Data;

using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

public class AuthService(AppDbContext db) : IAuthService
{
    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username)
            ?? throw new InvalidDataException("Invalid login: No user with this username exists.");

        if (!BCrypt.Verify(password, user.Password))
            throw new InvalidDataException("Invalid login: Password is invalid.");

        return "token"; //todo return jwt?;
    }

    public async Task<User> RegisterAsync(string username, string password)
    {
        if (await db.Users.AnyAsync(u => u.Username == username))
            throw new InvalidDataException("Registration failed: This username is already in use.");

        var pwd = BCrypt.HashPassword(password, BCrypt.GenerateSalt());
        var user = new User
        {
            Username = username,
            Password = pwd
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        return user;
    }
}
