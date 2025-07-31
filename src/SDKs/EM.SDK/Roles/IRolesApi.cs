using EM.Application.Features.Role.Commands;
using EM.Domain.Entities;

using Refit;

namespace EM.SDK.Roles;

public interface IRolesApi
{
    [Get("/roles")]
    Task<ApiResponse<List<Role>>> GetRolesAsync(CancellationToken ct);

    [Get("/roles/{id}")]
    Task<Role> GetRoleByIdAsync(int id, CancellationToken ct);

    [Post("/roles")]
    Task<Role> CreateRoleAsync([Body] CreateRoleCommand command, CancellationToken ct);

    [Put("/roles")]
    Task<Role> UpdateRoleAsync([Body] UpdateRoleCommand command, CancellationToken ct);

    [Delete("/roles/{id}")]
    Task DeleteRoleAsync(int id, CancellationToken ct);
}
