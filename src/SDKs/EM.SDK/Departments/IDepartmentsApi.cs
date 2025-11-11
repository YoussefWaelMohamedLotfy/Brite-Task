using EM.Application.Features.Department.Commands;
using EM.Domain.Entities;
using Refit;

namespace EM.SDK.Departments;

public interface IDepartmentsApi
{
    [Get("/departments")]
    Task<ApiResponse<List<Department>>> GetDepartmentsAsync(CancellationToken ct);

    [Get("/departments/{id}")]
    Task<ApiResponse<Department>> GetDepartmentByIdAsync(int id, CancellationToken ct);

    [Post("/departments")]
    Task<ApiResponse<Department>> CreateDepartmentAsync(
        [Body] CreateDepartmentCommand command,
        CancellationToken ct
    );

    [Put("/departments")]
    Task<ApiResponse<Department>> UpdateDepartmentAsync(
        [Body] UpdateDepartmentCommand command,
        CancellationToken ct
    );

    [Delete("/departments/{id}")]
    Task DeleteDepartmentAsync(int id, CancellationToken ct);
}
