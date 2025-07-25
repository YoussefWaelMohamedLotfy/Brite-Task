using EM.Application.Features.Employee.Commands;
using Xunit;

namespace EM.Application.UnitTests.Features.Employee.Commands;

[Collection("InMemoryDb")]
public sealed class CreateEmployeeCommandHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public void Validator_InvalidCommand_ReturnsError()
    {
        var validator = new CreateEmployeeCommandValidator();
        var command = new CreateEmployeeCommand("", "invalid", "", DateTimeOffset.Now.AddDays(1), true, 0, 0);
        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
        Assert.Contains(result.Errors, e => e.PropertyName == "DateOfJoining");
        Assert.Contains(result.Errors, e => e.PropertyName == "DepartmentId");
        Assert.Contains(result.Errors, e => e.PropertyName == "RoleId");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreated()
    {
        // Use seeded department and role IDs
        var departmentId = provider.DbContext.Departments.First().ID;
        var roleId = provider.DbContext.Roles.First().ID;
        var command = new CreateEmployeeCommand("Test User", "test@example.com", "1234567890", DateTimeOffset.Now.AddDays(-1), true, departmentId, roleId);
        var handler = new CreateEmployeeCommandHandler(provider.DbContext);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<EM.Domain.Entities.Employee>>(result);
        Assert.NotNull(createdResult.Value);
        Assert.Equal("Test User", createdResult.Value.Name);
    }

    [Fact]
    public async Task Handle_InvalidDepartmentOrRole_ReturnsBadRequest()
    {
        var command = new CreateEmployeeCommand("Test User", "test@example.com", "1234567890", DateTimeOffset.Now.AddDays(-1), true, -1, -1);
        var handler = new CreateEmployeeCommandHandler(provider.DbContext);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>(result);
    }
}
