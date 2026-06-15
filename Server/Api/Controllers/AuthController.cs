namespace Server.Api.Controllers;

using Server.Core.Dtos.Auth;
using Server.Core.Interfaces;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger, IAuthService service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokensDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var tokens = await service.LoginAsync(dto);
            logger.LogInformation("Login for user {Username} is successful",
                    dto.login);
            return Ok(tokens);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed login attempt for {Username}: {Message}",
                    dto.login, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<TokensDto>> Register([FromBody] RegistrationDto dto)
    {
        try
        {
            var tokens = await service.RegisterAsync(dto);
            logger.LogInformation("Registration of user {Username} is successful",
                    dto.login);
            return Ok(tokens);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed registration attempt for {Username}: {Message}",
                    dto.login, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokensDto>> Refresh([FromBody] TokensDto dto)
    {
        try
        {
            var tokens = await service.RefreshTokenAsync(dto);
            logger.LogInformation("Tokens were refreshed succesfuly");
            return Ok(tokens);
        }
        catch (Exception ex) when (ex is InvalidDataException || ex is SecurityTokenException)
        {
            logger.LogWarning("Failed to regenerate tokens: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
    }
}
