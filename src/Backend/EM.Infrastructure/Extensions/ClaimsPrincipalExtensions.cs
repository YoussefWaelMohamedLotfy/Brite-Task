using System.Security.Claims;

namespace EM.Infrastructure.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : throw new Exception("User Identifier is unavailable!");
    }
}
