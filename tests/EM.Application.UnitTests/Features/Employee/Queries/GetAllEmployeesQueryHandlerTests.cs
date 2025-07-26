using Bogus;

using EM.Application.Features.Employee.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Queries;

//[Collection("InMemoryDb")]
public sealed class GetAllEmployeesQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_EmptyQuery_ReturnsAllEmployees()
    {
        //var allEntities = provider.DbContext.Employees.ToList();
        //provider.DbContext.Employees.RemoveRange(allEntities);
        //provider.DbContext.SaveChanges();

        var employeeFaker = new Faker<Domain.Entities.Employee>()
            .RuleFor(e => e.Name, f => f.Name.FullName())
            .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.Name))
            .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(e => e.DateOfJoining, f => f.Date.PastOffset(5))
            .RuleFor(e => e.IsActive, f => f.Random.Bool())
            //.RuleFor(e => e.Department, f => f.PickRandom(departments))
            //.RuleFor(e => e.Role, f => f.PickRandom(roles))
            .RuleFor(e => e.CreatedBy, f => f.Random.Guid())
            .RuleFor(e => e.CreatedAt, f => f.Date.PastOffset(3));
        var employees = employeeFaker.Generate(50);
        provider.DbContext.Employees.AddRange(employees);
        provider.DbContext.SaveChanges();
        var query = new GetAllEmployeesQuery(null, null, null, null, null);
        var handler = new GetAllEmployeesQueryHandler(provider.DbContext);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        var okResult = Assert.IsType<Ok<List<EM.Domain.Entities.Employee>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(50, okResult.Value.Count);
    }

    [Fact]
    public async Task Handle_WithNameFilter_ReturnsFilteredEmployees()
    {
        var employee = new Domain.Entities.Employee { Name = "Alice Smith", Email = "test@test.com", Phone = "0123456" };
        provider.DbContext.Employees.Add(employee);
        provider.DbContext.SaveChanges();
        var query = new GetAllEmployeesQuery("Alice Smith", null, null, null, null);
        var handler = new GetAllEmployeesQueryHandler(provider.DbContext);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        var okResult = Assert.IsType<Ok<List<EM.Domain.Entities.Employee>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Single(okResult.Value);
        Assert.Equal("Alice Smith", okResult.Value[0].Name);
    }
}
