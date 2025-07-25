using EM.Application.Features.Employee.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Queries;

//[Collection("InMemoryDb")]
public sealed class GetEmployeeByIdQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        var query = new GetEmployeeByIdQuery(Guid.NewGuid());
        var handler = new GetEmployeeByIdQueryHandler(provider.DbContext);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsEmployee()
    {
        var employee = provider.DbContext.Employees.First();
        var query = new GetEmployeeByIdQuery(employee.ID);
        var handler = new GetEmployeeByIdQueryHandler(provider.DbContext);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        var okResult = Assert.IsType<Ok<EM.Domain.Entities.Employee>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(employee.ID, okResult.Value.ID);
    }
}
