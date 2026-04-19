using EM.Domain.Entities;

namespace EM.Domain.UnitTests;

public sealed class EmployeeTests
{
    [Test]
    public async Task ToggleEmployeeActivationStatus_EmployeeIsInactive_ShouldToggleStatusToTrue()
    {
        // Arrange
        Employee employee = new() { ID = Guid.CreateVersion7(), IsActive = false };

        // Act
        employee.ToggleActivation();

        // Assert
        await Assert.That(employee.IsActive).IsTrue();
    }

    [Test]
    public async Task ToggleEmployeeActivationStatus_EmployeeIsActive_ShouldToggleStatusToFalse()
    {
        // Arrange
        Employee employee = new() { ID = Guid.CreateVersion7(), IsActive = true };

        // Act
        employee.ToggleActivation();

        // Assert
        await Assert.That(employee.IsActive).IsFalse();
    }
}
