using System.Threading.Tasks;

using EM.Application.Features.Department.Commands;
using EM.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace EM.Application.UnitTests.Features.Department.Commands;

[Collection("InMemoryDb")]
public sealed class UpdateDepartmentCommandHandlerTests(InMemoryDbProvider provider)
{
    [Fact]
    public async Task Handle_ValidCommand_UpdatesDepartment()
    {
        // Arrange
        var department = new EM.Domain.Entities.Department { Name = "Old", Description = "Old Desc" };
        provider.DbContext.Departments.Add(department);
        await provider.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var command = new UpdateDepartmentCommand(department.ID, "New", "New Desc");
        var handler = new UpdateDepartmentCommandHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<EM.Domain.Entities.Department>>(result);
        var updated = okResult.Value;
        Assert.Equal("New", updated.Name);
        Assert.Equal("New Desc", updated.Description);
    }

    [Fact]
    public async Task Handle_NonExistentDepartment_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateDepartmentCommand(999, "Name", "Desc");
        var handler = new UpdateDepartmentCommandHandler(provider.DbContext);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Validator_InvalidCommand_ReturnsError()
    {
        // Arrange
        var validator = new UpdateDepartmentCommandValidator();
        var command = new UpdateDepartmentCommand(0, "", null);

        // Act
        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }
}
