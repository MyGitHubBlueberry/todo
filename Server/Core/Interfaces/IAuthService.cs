namespace Server.Core.Interfaces;

using Server.Core.Models;

using System.Threading.Tasks;

public interface IAuthService
{
    Task<User> RegisterAsync(string username, string password);
    Task<string?> LoginAsync(string username, string password);
}
