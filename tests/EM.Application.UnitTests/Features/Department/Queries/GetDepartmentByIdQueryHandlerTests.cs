using EM.Application.Features.Department.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Department.Queries;

//[Collection("InMemoryDb")]
public sealed class GetDepartmentByIdQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_ExistingId_ReturnsDepartment()
    {
        // Arrange
        Domain.Entities.Department department = new() { ID = 60, Name = "Test" };
        provider.DbContext.Departments.Add(department);
        provider.DbContext.SaveChanges();
        var query = new GetDepartmentByIdQuery(department.ID);
        var handler = new GetDepartmentByIdQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<Domain.Entities.Department>>(result);
        Assert.Equal(department.ID, okResult.Value.ID);
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
