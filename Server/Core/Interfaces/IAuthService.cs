namespace Server.Core.Interfaces;

using Server.Core.Dtos.Auth;

using System.Threading.Tasks;

public interface IAuthService
{
    Task<TokensDto> RegisterAsync(RegistrationDto registrationDto);
    Task<TokensDto> LoginAsync(LoginDto loginDto);
    Task<TokensDto> RefreshTokenAsync(TokensDto tokensDto);
}
