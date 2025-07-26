using EM.Application.Features.Employee.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Commands;

//[Collection("InMemoryDb")]
public sealed class UpdateEmployeeCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
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
        var department = new Domain.Entities.Department { ID = 1000, Name = "Test" };
        var role = new Domain.Entities.Role { ID = 1000, Name = "Test", Permissions = ["Test"] };
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), "Test", "test@example.com", "1234567890", DateTimeOffset.Now.AddDays(-1), true, department.ID, role.ID);
        var handler = new UpdateEmployeeCommandHandler(provider.DbContext);
        
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Handle_InvalidDepartmentOrRole_ReturnsBadRequest()
    {
        var employee = new Domain.Entities.Employee { ID = Guid.NewGuid(), Name = "New", Email = "test@test.com", Phone = "0123456" };
        provider.DbContext.Employees.Add(employee);
        provider.DbContext.SaveChanges();
        var command = new UpdateEmployeeCommand(employee.ID, "Test", "test@example.com", "1234567890", DateTimeOffset.Now.AddDays(-1), true, -1, -1);
        var handler = new UpdateEmployeeCommandHandler(provider.DbContext);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        
        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesEmployee()
    {
        var department = new Domain.Entities.Department { ID = 200, Name = "NewDept" };
        var role = new Domain.Entities.Role { ID = 300, Name = "NewRole", Permissions = ["Permit"] };
        var x = Guid.NewGuid();
        var employee = new Domain.Entities.Employee 
        {
            ID = x,
            Name = "New",
            Email = "test@test.com",
            Phone = "0123456",
            Department = department,
            Role = role
        };
        provider.DbContext.Employees.Add(employee);
        provider.DbContext.SaveChanges();
        var handler = new UpdateEmployeeCommandHandler(provider.DbContext);
        var command = new UpdateEmployeeCommand(x, "Updated Name", employee.Email, employee.Phone, employee.DateOfJoining, !employee.IsActive, department.ID, role.ID);
        
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        
        var okResult = Assert.IsType<Ok<EM.Domain.Entities.Employee>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("Updated Name", okResult.Value.Name);
    }
}
