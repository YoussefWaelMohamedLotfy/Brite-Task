using EM.Application.Features.Department.Commands;
using EM.Domain.Entities;

using Refit;

namespace EM.SDK.Departments;

public interface IDepartmentsApi
{
    [Get("/departments")]
    Task<List<Department>> GetDepartmentsAsync();

    [Get("/departments/{id}")]
    Task<Department> GetDepartmentByIdAsync(int id);

    [Post("/departments")]
    Task<Department> CreateDepartmentAsync([Body] CreateDepartmentCommand command);

    [Put("/departments")]
    Task<Department> UpdateDepartmentAsync([Body] UpdateDepartmentCommand command);

    [Delete("/departments/{id}")]
    Task DeleteDepartmentAsync(int id);
}
