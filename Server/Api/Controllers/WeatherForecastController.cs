namespace Server.Api.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class AuthController(ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost()]
    public async Task<IActionResult> PostLogin()
    {
    }
}
