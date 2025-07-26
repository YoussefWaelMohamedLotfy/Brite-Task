using EM.Application.Features.Department.Commands;

using Microsoft.AspNetCore.Http.HttpResults;

namespace EM.Application.UnitTests.Features.Department.Commands;

//[Collection("InMemoryDb")]
public sealed class DeleteDepartmentCommandHandlerTests(InMemoryDbProvider provider) : IClassFixture<InMemoryDbProvider>
{
    [Fact]
    public async Task Handle_ValidCommand_DeletesDepartment()
    {
        // Arrange
        var department = new EM.Domain.Entities.Department { ID = 655, Name = "ToDelete" };
        provider.DbContext.Departments.Add(department);
        provider.DbContext.SaveChanges();
        var command = new DeleteDepartmentCommand(department.ID);
        var handler = new DeleteDepartmentCommandHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NoContent>(result);
    }

    [Fact]
    public async Task Handle_NonExistentDepartment_ReturnsNotFound()
    {
        // Arrange
        var command = new DeleteDepartmentCommand(999);
        var handler = new DeleteDepartmentCommandHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Validator_InvalidId_ReturnsError()
    {
        // Arrange
        var validator = new DeleteDepartmentCommandValidator();
        var command = new DeleteDepartmentCommand(0);

        // Act
        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
