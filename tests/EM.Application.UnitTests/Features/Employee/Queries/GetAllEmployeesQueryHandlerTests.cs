using EM.Application.Features.Employee.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Queries;

//[Collection("InMemoryDb")]
public sealed class GetAllEmployeesQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_EmptyQuery_ReturnsAllEmployees()
    {
        var query = new GetAllEmployeesQuery(null, null, null, null, null);
        var handler = new GetAllEmployeesQueryHandler(provider.DbContext);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        var okResult = Assert.IsType<Ok<List<EM.Domain.Entities.Employee>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, okResult.Value.Count);
    }

    [Fact]
    public async Task Handle_WithNameFilter_ReturnsFilteredEmployees()
    {
        var query = new GetAllEmployeesQuery("Alice Smith", null, null, null, null);
        var handler = new GetAllEmployeesQueryHandler(provider.DbContext);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        var okResult = Assert.IsType<Ok<List<EM.Domain.Entities.Employee>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Single(okResult.Value);
        Assert.Equal("Alice Smith", okResult.Value[0].Name);
    }
}
