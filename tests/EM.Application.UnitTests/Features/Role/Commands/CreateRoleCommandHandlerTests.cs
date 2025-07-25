using EM.Application.Features.Role.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Role.Commands;

[Collection("InMemoryDb")]
public sealed class CreateRoleCommandHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public async Task CreateRoleCommandHandler_ValidRequest_CreatesRole()
    {
        // Arrange
        var handler = new CreateRoleCommandHandler(provider.DbContext);
        var command = new CreateRoleCommand("Manager", ["Read", "Write"]);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<Domain.Entities.Role>>(result);
        var role = createdResult.Value;
        Assert.NotNull(role);
        Assert.Equal("Manager", role.Name);
        Assert.Contains("Write", role.Permissions);
        Assert.True(provider.DbContext.Roles.Any(r => r.Name == "Manager"));
    }
}
