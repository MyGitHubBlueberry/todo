namespace Server.Core.Interfaces;

using Server.Core.Models;
using Server.Core.Dtos.Auth;

using System.Threading.Tasks;

public interface IAuthService
{
    Task<User> RegisterAsync(string username, string password);
    Task<TokensDto> LoginAsync(string username, string password);
    Task<TokensDto> RefreshTokenAsync(string expiredToken, string refreshToken);
}
