using EM.Application.Features.Role.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Roles.Commands;

[Collection("InMemoryDb")]
public sealed class DeleteRoleCommandHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public async Task DeleteRoleCommandHandler_ExistingRole_DeletesRole()
    {
        // Arrange
        var existingRole = provider.DbContext.Roles.First();
        var handler = new DeleteRoleCommandHandler(provider.DbContext);
        var command = new DeleteRoleCommand(existingRole.ID);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NoContent>(result);
        Assert.False(provider.DbContext.Roles.Any(r => r.ID == existingRole.ID));
    }

    [Fact]
    public async Task DeleteRoleCommandHandler_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var handler = new DeleteRoleCommandHandler(provider.DbContext);
        var command = new DeleteRoleCommand(-1);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }
}
