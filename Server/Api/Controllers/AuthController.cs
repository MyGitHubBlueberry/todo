namespace Server.Api.Controllers;

using Server.Core.Dtos.Auth;
using Server.Core.Interfaces;

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger, IAuthService service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokensDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            return Ok(await service.LoginAsync(loginDto));
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed login attempt for {Username}: {Message}",
                    loginDto.login, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<TokensDto>> Register([FromBody] RegistrationDto registrationDto)
    {
        try
        {
            return Ok(await service.RegisterAsync(registrationDto));
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed registration attempt for {Username}: {Message}",
                    registrationDto.login, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokensDto>> Refresh([FromBody] TokensDto tokensDto)
    {
        try
        {
            return Ok(await service.RefreshTokenAsync(tokensDto));
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed to regenerate tokens: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
    }
}
