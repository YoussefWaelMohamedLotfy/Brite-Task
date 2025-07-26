using EM.Application.Features.Employee.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Employee.Commands;

//[Collection("InMemoryDb")]
public sealed class DeleteEmployeeCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
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
        Guid x = Guid.Parse("3A01B115-7B1C-4049-9625-24AA59B0985E");

        var employee = new Domain.Entities.Employee
        {
            ID = x,
            Name = "New",
            Email = "test@test.com",
            Phone = "0123456",
            Department = new() { ID = 2000, Name = "Test" },
            Role = new() { ID = 2001, Name = "Test", Permissions = ["Test"] },
        };
        provider.DbContext.Employees.Add(employee);
        provider.DbContext.SaveChanges();
        var command = new DeleteEmployeeCommand(x);
        var handler = new DeleteEmployeeCommandHandler(provider.DbContext);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.IsType<NoContent>(result);
    }
}
