using System.Security.Claims;

using EM.Infrastructure.Extensions;

namespace EM.Infrastructure.UnitTests;

public sealed class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_ValidGuidClaim_ReturnsGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUserId();

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_MissingNameIdentifier_ThrowsException()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => principal.GetUserId());
        Assert.Equal("User Identifier is unavailable!", ex.Message);
    }

    [Fact]
    public void GetUserId_InvalidGuidClaim_ThrowsException()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "not-a-guid") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => principal.GetUserId());
        Assert.Equal("User Identifier is unavailable!", ex.Message);
    }
}
