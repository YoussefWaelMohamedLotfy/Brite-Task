using EM.Application.Features.Role.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Role.Commands;

//[Collection("InMemoryDb")]
public sealed class UpdateRoleCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task UpdateRoleCommandHandler_ExistingRole_UpdatesRole()
    {
        // Arrange
        var existingRole = new Domain.Entities.Role
        {
            ID = 314,
            Name = "OldName",
            Permissions = ["OldPermission"]
        };
        provider.DbContext.Roles.Add(existingRole);
        provider.DbContext.SaveChanges();
        var handler = new UpdateRoleCommandHandler(provider.DbContext);
        var command = new UpdateRoleCommand(existingRole.ID, "UpdatedName", ["Read"]);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<Domain.Entities.Role>>(result);
        var role = okResult.Value;
        Assert.Equal("UpdatedName", role.Name);
        Assert.Single(role.Permissions);
        Assert.Contains("Read", role.Permissions);
    }

    [Fact]
    public async Task UpdateRoleCommandHandler_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var handler = new UpdateRoleCommandHandler(provider.DbContext);
        var command = new UpdateRoleCommand(-1, "DoesNotExist", new List<string> { "None" });

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }
}
