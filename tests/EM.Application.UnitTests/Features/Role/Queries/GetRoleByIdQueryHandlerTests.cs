using EM.Application.Features.Role.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Role.Queries;

//[Collection("InMemoryDb")]
public sealed class GetRoleByIdQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task GetRoleByIdQueryHandler_ExistingId_ReturnsRole()
    {
        // Arrange
        var existingRole = new Domain.Entities.Role
        {
            ID = 70,
            Name = "Admin",
            Permissions = ["Create", "Read", "Update", "Delete"],
        };
        provider.DbContext.Roles.Add(existingRole);
        provider.DbContext.SaveChanges();
        var handler = new GetRoleByIdQueryHandler(provider.DbContext);
        var query = new GetRoleByIdQuery(existingRole.ID);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<Domain.Entities.Role>>(result);
        var role = okResult.Value;
        Assert.NotNull(role);
        Assert.Equal(existingRole.ID, role.ID);
    }

    [Fact]
    public async Task GetRoleByIdQueryHandler_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var handler = new GetRoleByIdQueryHandler(provider.DbContext);
        var query = new GetRoleByIdQuery(-1);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }
}
