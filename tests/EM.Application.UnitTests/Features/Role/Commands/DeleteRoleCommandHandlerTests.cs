using EM.Application.Features.Role.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Role.Commands;

//[Collection("InMemoryDb")]
public sealed class DeleteRoleCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task DeleteRoleCommandHandler_ExistingRole_DeletesRole()
    {
        // Arrange
        var existingRole = new Domain.Entities.Role
        {
            ID = 2130,
            Name = "TestRole",
            Permissions = ["Create", "Read"]
        };
        provider.DbContext.Roles.Add(existingRole);
        provider.DbContext.SaveChanges();
        var command = new DeleteRoleCommand(2130);
        var handler = new DeleteRoleCommandHandler(provider.DbContext);

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
