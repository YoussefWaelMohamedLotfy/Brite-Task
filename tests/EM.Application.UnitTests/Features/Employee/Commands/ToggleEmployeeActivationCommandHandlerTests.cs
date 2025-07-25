using EM.Application.Features.Employee.Commands;
using Xunit;

namespace EM.Application.UnitTests.Features.Employee.Commands;

[Collection("InMemoryDb")]
public sealed class ToggleEmployeeActivationCommandHandlerTests(InMemoryDbProvider provider)
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
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound>(result);
    }

    [Fact]
    public async Task Handle_ExistingEmployee_TogglesActivation()
    {
        var handler = new ToggleEmployeeActivationCommandHandler(provider.DbContext);
        var employee = provider.DbContext.Employees.First();
        bool originalIsActive = employee.IsActive;
        var command = new ToggleEmployeeActivationCommand(employee.ID);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<EM.Domain.Entities.Employee>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(!originalIsActive, okResult.Value.IsActive);
    }
}
