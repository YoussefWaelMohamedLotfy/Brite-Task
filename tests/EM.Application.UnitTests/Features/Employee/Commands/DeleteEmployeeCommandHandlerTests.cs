using EM.Application.Features.Employee.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

using Xunit;

namespace EM.Application.UnitTests.Features.Employee.Commands;

[Collection("InMemoryDb")]
public sealed class DeleteEmployeeCommandHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public void Validator_InvalidCommand_ReturnsError()
    {
        var validator = new DeleteEmployeeCommandValidator();
        var command = new DeleteEmployeeCommand(Guid.Empty);
        var result = validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Handle_NonExistentEmployee_ReturnsNotFound()
    {
        var handler = new DeleteEmployeeCommandHandler(provider.DbContext);
        var command = new DeleteEmployeeCommand(Guid.NewGuid());
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Handle_ExistingEmployee_DeletesEmployee()
    {
        var handler = new DeleteEmployeeCommandHandler(provider.DbContext);
        var employee = provider.DbContext.Employees.First();
        var command = new DeleteEmployeeCommand(employee.ID);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        Assert.IsType<NoContent>(result);
    }
}
