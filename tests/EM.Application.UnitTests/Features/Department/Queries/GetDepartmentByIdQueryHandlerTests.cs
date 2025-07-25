using EM.Application.Features.Department.Queries;
using EM.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace EM.Application.UnitTests.Features.Department.Queries;

[Collection("InMemoryDb")]
public sealed class GetDepartmentByIdQueryHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public async Task Handle_ExistingId_ReturnsDepartment()
    {
        // Arrange
        var query = new GetDepartmentByIdQuery(1);
        var handler = new GetDepartmentByIdQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<Domain.Entities.Department>>(result);
        Assert.Equal(1, okResult.Value.ID);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var query = new GetDepartmentByIdQuery(999);
        var handler = new GetDepartmentByIdQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }
}
