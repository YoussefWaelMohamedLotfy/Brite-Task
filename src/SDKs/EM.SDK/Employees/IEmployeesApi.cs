using EM.Application.Features.Employee.Commands;
using EM.Domain.Entities;
using Refit;

namespace EM.SDK.Employees;

public interface IEmployeesApi
{
    [Get("/employees")]
    Task<ApiResponse<List<Employee>>> GetEmployeesAsync(
        string? name = null,
        int? departmentId = null,
        bool? isActive = null,
        DateTime? dateOfJoiningFrom = null,
        DateTime? dateOfJoiningTo = null,
        CancellationToken ct = default
    );

    [Get("/employees/{id}")]
    Task<ApiResponse<Employee>> GetEmployeeByIdAsync(Guid id, CancellationToken ct);

    [Post("/employees")]
    Task<ApiResponse<Employee>> CreateEmployeeAsync(
        [Body] CreateEmployeeCommand command,
        CancellationToken ct
    );

    [Put("/employees")]
    Task<ApiResponse<Employee>> UpdateEmployeeAsync(
        [Body] UpdateEmployeeCommand command,
        CancellationToken ct
    );

    [Delete("/employees/{id}")]
    Task DeleteEmployeeAsync(Guid id, CancellationToken ct);

    [Put("/employees/{id}/toggle-activation")]
    Task<ApiResponse<Employee>> ToggleEmployeeActivationAsync(Guid id, CancellationToken ct);
}
