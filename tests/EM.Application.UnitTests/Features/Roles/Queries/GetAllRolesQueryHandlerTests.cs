using EM.Application.Features.Role.Queries;
using EM.Domain.Entities;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Roles.Queries;

[Collection("InMemoryDb")]
public sealed class GetAllRolesQueryHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public async Task GetAllRolesQueryHandler_SendEmptyQuery_ReturnsAllRoles()
    {
        // Arrange
        var query = new GetAllRolesQuery();
        var handler = new GetAllRolesQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<List<Role>>>(result);
        var roles = okResult.Value;
        Assert.NotNull(roles);
        Assert.Equal(2, roles.Count);
    }
}
