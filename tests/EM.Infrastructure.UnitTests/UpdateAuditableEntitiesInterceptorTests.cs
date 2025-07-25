using System.Security.Claims;

using EM.Infrastructure.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Moq;

namespace EM.Infrastructure.UnitTests;

public sealed class UpdateAuditableEntitiesInterceptorTests : InMemoryDbProvider
{
    [Fact]
    public async Task SavingChangesAsync_ContextIsNull_CallsBase()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        var interceptor = new UpdateAuditableEntitiesInterceptor(claimsPrincipal);
        var mockEventData = new Mock<DbContextEventData>(null, null, null);
        mockEventData.Setup(x => x.Context).Returns((DbContext)null);
        var initialResult = new InterceptionResult<int>();

        // Act
        var result = await interceptor.SavingChangesAsync(mockEventData.Object, initialResult, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(initialResult, result);
    }

    [Fact]
    public async Task SavingChangesAsync_AddedEntry_SetsCreatedFields_WithInMemoryDb()
    {
        // Arrange: Use seeded department and role from in-memory context
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
        var interceptor = new UpdateAuditableEntitiesInterceptor(claimsPrincipal);
        var employee = new EM.Domain.Entities.Employee
        {
            Name = "Test User",
            Email = "test.user@example.com",
            Phone = "555-555-5555",
            DateOfJoining = DateTimeOffset.UtcNow.AddMonths(-3),
            IsActive = true,
            Department = _context.Departments.First(),
            Role = _context.Roles.First(),
            CreatedBy = Guid.Empty,
            CreatedAt = DateTimeOffset.MinValue
        };
        _context.Employees.Add(employee);
        // Mark as Added
        var entry = _context.Entry(employee);
        entry.State = EntityState.Added;
        var eventData = new Mock<DbContextEventData>(null, null, null);
        eventData.Setup(x => x.Context).Returns(_context);
        var initialResult = new InterceptionResult<int>();

        // Act
        var result = await interceptor.SavingChangesAsync(eventData.Object, initialResult, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(DateTimeOffset.MinValue, employee.CreatedAt);
        Assert.Equal(userId, employee.CreatedBy);
        Assert.Equal(initialResult, result);
    }

    [Fact]
    public async Task SavingChangesAsync_ModifiedEntry_SetsUpdatedFields_WithInMemoryDb()
    {
        // Arrange: Use seeded employee from in-memory context
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
        var interceptor = new UpdateAuditableEntitiesInterceptor(claimsPrincipal);
        var employee = _context.Employees.First();
        employee.Name = "Updated Name";
        var entry = _context.Entry(employee);
        entry.State = EntityState.Modified;
        var eventData = new Mock<DbContextEventData>(null, null, null);
        eventData.Setup(x => x.Context).Returns(_context);
        var initialResult = new InterceptionResult<int>();

        // Act
        var result = await interceptor.SavingChangesAsync(eventData.Object, initialResult, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(employee.UpdatedAt);
        Assert.Equal(userId, employee.UpdatedBy);
        Assert.Equal(initialResult, result);
    }
}
