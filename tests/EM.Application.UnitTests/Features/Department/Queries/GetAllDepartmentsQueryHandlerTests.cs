using Bogus;

using EM.Application.Features.Department.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Department.Queries;

//[Collection("InMemoryDb")]
public sealed class GetAllDepartmentsQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_ReturnsAllDepartments()
    {
        // Arrange
        //var allEntities = provider.DbContext.Departments.ToList();
        //provider.DbContext.Departments.RemoveRange(allEntities);
        //provider.DbContext.SaveChanges();

        var departmentFaker = new Faker<Domain.Entities.Department>()
            .RuleFor(d => d.Name, f => f.Commerce.Department())
            .RuleFor(d => d.Description, f => f.Lorem.Sentence())
            .RuleFor(d => d.CreatedBy, f => f.Random.Guid())
            .RuleFor(d => d.CreatedAt, f => f.Date.PastOffset(3));
        var departments = departmentFaker.Generate(5);
        provider.DbContext.Departments.UpdateRange(departments);
        provider.DbContext.SaveChanges();

        var query = new GetAllDepartmentsQuery();
        var handler = new GetAllDepartmentsQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);
        var x = provider.DbContext.Departments.Count();
        // Assert
        var okResult = Assert.IsType<Ok<List<Domain.Entities.Department>>>(result);
        Assert.Equal(7, okResult.Value.Count);
    }
}
