using Bogus;

using EM.Application.Features.Role.Queries;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Role.Queries;

//[Collection("InMemoryDb")]
public sealed class GetAllRolesQueryHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task GetAllRolesQueryHandler_SendEmptyQuery_ReturnsAllRoles()
    {
        // Arrange
        //var allEntities = provider.DbContext.Roles.ToList();
        //provider.DbContext.Roles.RemoveRange(allEntities);
        //provider.DbContext.SaveChanges();

        var roleFaker = new Faker<Domain.Entities.Role>()
            .RuleFor(r => r.Name, f => f.Person.FullName)
            .RuleFor(r => r.Permissions, f => f.Make(f.Random.Int(1, 5), () => f.PickRandom(new[] { "Create", "Read", "Update", "Delete", "Approve" })))
            .RuleFor(r => r.CreatedBy, f => f.Random.Guid())
            .RuleFor(r => r.CreatedAt, f => f.Date.PastOffset(3));
        var roles = roleFaker.Generate(5);
        provider.DbContext.Roles.AddRange(roles);
        provider.DbContext.SaveChanges();
        var query = new GetAllRolesQuery();
        var handler = new GetAllRolesQueryHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<List<Domain.Entities.Role>>>(result);
        var rolesResult = okResult.Value;
        Assert.NotNull(rolesResult);
        Assert.Equal(7, rolesResult.Count);
    }
}
