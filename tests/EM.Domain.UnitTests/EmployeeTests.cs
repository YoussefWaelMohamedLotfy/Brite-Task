using EM.Domain.Entities;

namespace EM.Domain.UnitTests;

public sealed class EmployeeTests
{
    [Fact]
    public void ToggleEmployeeActivationStatus_EmployeeIsInactive_ShouldToggleStatusToTrue()
    {
        // Arrange
        Employee employee = new() { ID = Guid.CreateVersion7(), IsActive = false };

        // Act
        employee.ToggleActivation();

        // Assert
        Assert.True(employee.IsActive);
    }

    [Fact]
    public void ToggleEmployeeActivationStatus_EmployeeIsActive_ShouldToggleStatusToFalse()
    {
        // Arrange
        Employee employee = new() { ID = Guid.CreateVersion7(), IsActive = true };

        // Act
        employee.ToggleActivation();

        // Assert
        Assert.False(employee.IsActive);
    }
}
