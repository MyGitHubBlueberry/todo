namespace Server.Services;

using Server.Core.Interfaces;
using Server.Core.Models;
using Server.Data;

using System.Threading.Tasks;

public class AuthService(AppDbContext db) : IAuthService
{
    Task<string?> IAuthService.LoginAsync(string username, string password)
    {
        throw new System.NotImplementedException();
    }

    Task<User> IAuthService.RegisterAsync(string username, string password)
    {
        throw new System.NotImplementedException();
    }
}
