using EM.Application.Features.Employee.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Queries;

//[Collection("InMemoryDb")]
public sealed class GetEmployeeByIdQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{

    [Fact]
    public async Task Handle_ExistingId_ReturnsEmployee()
    {
        Guid x = Guid.Parse("4B904BB1-55FC-49D9-8145-D620B71AD780");
        var employee = new Domain.Entities.Employee {
            ID = x,
            Name = "New",
            Email = "test@test.com",
            Phone = "0123456",
            Role = new() { ID = 2011, Name = "Test", Permissions = ["Test"] },
            Department = new() { ID = 2012, Name = "Test" }
        };
        provider.DbContext.Employees.Add(employee); 
        provider.DbContext.SaveChanges();
        var query = new GetEmployeeByIdQuery(x);
        var handler = new GetEmployeeByIdQueryHandler(provider.DbContext);
        
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        
        var okResult = Assert.IsType<Ok<EM.Domain.Entities.Employee>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(x, okResult.Value.ID);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        var query = new GetEmployeeByIdQuery(Guid.NewGuid());
        var handler = new GetEmployeeByIdQueryHandler(provider.DbContext);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        Assert.IsType<NotFound>(result);
    }
}
