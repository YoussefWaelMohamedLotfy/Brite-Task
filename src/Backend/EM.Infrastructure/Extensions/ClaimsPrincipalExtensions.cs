using System.Security.Claims;

namespace EM.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user identifier from the claims principal.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The user identifier as a <see cref="Guid"/>.</returns>
    /// <exception cref="Exception">Thrown if the user identifier is unavailable.</exception>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : throw new Exception("User Identifier is unavailable!");
    }
}
