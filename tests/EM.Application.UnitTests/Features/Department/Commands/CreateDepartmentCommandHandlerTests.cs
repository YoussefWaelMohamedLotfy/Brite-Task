using EM.Application.Features.Department.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Department.Commands;

//[Collection("InMemoryDb")]
public sealed class CreateDepartmentCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesDepartment()
    {
        // Arrange
        var command = new CreateDepartmentCommand("HR", "Human Resources");
        var handler = new CreateDepartmentCommandHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<Domain.Entities.Department>>(result);
        var department = createdResult.Value;
        Assert.NotNull(department);
        Assert.Equal("HR", department.Name);
        Assert.Equal("Human Resources", department.Description);
    }

    [Fact]
    public async Task Validator_InvalidName_ReturnsError()
    {
        // Arrange
        var validator = new CreateDepartmentCommandValidator();
        var command = new CreateDepartmentCommand("", "desc");

        // Act
        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }
}
