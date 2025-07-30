using EM.Application.Features.Employee.Commands;
using EM.Domain.Entities;

using Refit;

namespace EM.SDK.Employees;

public interface IEmployeesApi
{
    [Get("/employees")]
    Task<List<Employee>> GetEmployeesAsync(
        string? name = null,
        int? departmentId = null,
        bool? isActive = null,
        DateTime? dateOfJoiningFrom = null,
        DateTime? dateOfJoiningTo = null
    );

    [Get("/employees/{id}")]
    Task<Employee> GetEmployeeByIdAsync(Guid id);

    [Post("/employees")]
    Task<Employee> CreateEmployeeAsync([Body] CreateEmployeeCommand command);

    [Put("/employees")]
    Task<Employee> UpdateEmployeeAsync([Body] UpdateEmployeeCommand command);

    [Delete("/employees/{id}")]
    Task DeleteEmployeeAsync(Guid id);

    [Put("/employees/{id}/toggle-activation")]
    Task<Employee> ToggleEmployeeActivationAsync(Guid id);
}
