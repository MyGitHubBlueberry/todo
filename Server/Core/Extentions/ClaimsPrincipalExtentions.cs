namespace Server.Core.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
    {
        userId = -1;
        var idString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idString))
            return false;

        return int.TryParse(idString, out userId);
    }
}
