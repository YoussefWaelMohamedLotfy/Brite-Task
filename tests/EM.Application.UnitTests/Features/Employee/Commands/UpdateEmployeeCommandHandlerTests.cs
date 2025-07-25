using EM.Application.Features.Employee.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Commands;

[Collection("InMemoryDb")]
public sealed class UpdateEmployeeCommandHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public void Validator_InvalidCommand_ReturnsError()
    {
        var validator = new UpdateEmployeeCommandValidator();
        var command = new UpdateEmployeeCommand(Guid.Empty, "", "invalid", "", DateTimeOffset.Now.AddDays(1), true, 0, 0);
        var result = validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Handle_NonExistentEmployee_ReturnsNotFound()
    {
        var handler = new UpdateEmployeeCommandHandler(provider.DbContext);
        var departmentId = provider.DbContext.Departments.First().ID;
        var roleId = provider.DbContext.Roles.First().ID;
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), "Test", "test@example.com", "1234567890", DateTimeOffset.Now.AddDays(-1), true, departmentId, roleId);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Handle_InvalidDepartmentOrRole_ReturnsBadRequest()
    {
        var handler = new UpdateEmployeeCommandHandler(provider.DbContext);
        var employee = provider.DbContext.Employees.First();
        var command = new UpdateEmployeeCommand(employee.ID, "Test", "test@example.com", "1234567890", DateTimeOffset.Now.AddDays(-1), true, -1, -1);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesEmployee()
    {
        var handler = new UpdateEmployeeCommandHandler(provider.DbContext);
        var employee = provider.DbContext.Employees.First();
        var departmentId = provider.DbContext.Departments.First().ID;
        var roleId = provider.DbContext.Roles.First().ID;
        var command = new UpdateEmployeeCommand(employee.ID, "Updated Name", employee.Email, employee.Phone, employee.DateOfJoining, !employee.IsActive, departmentId, roleId);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        var okResult = Assert.IsType<Ok<EM.Domain.Entities.Employee>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("Updated Name", okResult.Value.Name);
    }
}
