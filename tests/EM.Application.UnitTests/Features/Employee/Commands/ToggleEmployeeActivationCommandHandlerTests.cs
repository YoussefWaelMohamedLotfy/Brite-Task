using EM.Application.Features.Employee.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Commands;

//[Collection("InMemoryDb")]
public sealed class ToggleEmployeeActivationCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public void Validator_InvalidCommand_ReturnsError()
    {
        var validator = new ToggleEmployeeActivationCommandValidator();
        var command = new ToggleEmployeeActivationCommand(Guid.Empty);
        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Handle_NonExistentEmployee_ReturnsNotFound()
    {
        var handler = new ToggleEmployeeActivationCommandHandler(provider.DbContext);
        var command = new ToggleEmployeeActivationCommand(Guid.NewGuid());
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Handle_ExistingEmployee_TogglesActivation()
    {
        //Arrange
        Guid x = Guid.Parse("7FC362D1-AC00-49F7-8FF3-0D357D3C6499");
        var employee = new Domain.Entities.Employee {
            ID = x,
            Name = "Test",
            Email = "test@test.com",
            Phone = "0123456",
            IsActive = false,
            Role = new() { ID = 2021, Name = "Test", Permissions = ["Test"] },
            Department = new() { ID = 2022, Name = "Test" }
        };
        provider.DbContext.Employees.Add(employee);
        provider.DbContext.SaveChanges();
        var command = new ToggleEmployeeActivationCommand(employee.ID);
        var handler = new ToggleEmployeeActivationCommandHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<Domain.Entities.Employee>>(result);
        Assert.NotNull(okResult.Value);
        Assert.True(okResult.Value.IsActive);
    }
}
