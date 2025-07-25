using EM.Application.Features.Department.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Department.Queries;

[Collection("InMemoryDb")]
public sealed class GetAllDepartmentsQueryHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public async Task Handle_ReturnsAllDepartments()
    {
        var query = new GetAllDepartmentsQuery();
        var handler = new GetAllDepartmentsQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<List<Domain.Entities.Department>>>(result);
        Assert.Equal(2, okResult.Value.Count);
    }
}
