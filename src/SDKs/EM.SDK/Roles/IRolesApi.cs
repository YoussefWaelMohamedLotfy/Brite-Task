using EM.Application.Features.Role.Commands;
using EM.Domain.Entities;

using Refit;

namespace EM.SDK.Roles;

public interface IRolesApi
{
    [Get("/roles")]
    Task<List<Role>> GetRolesAsync();

    [Get("/roles/{id}")]
    Task<Role> GetRoleByIdAsync(int id);

    [Post("/roles")]
    Task<Role> CreateRoleAsync([Body] CreateRoleCommand command);

    [Put("/roles")]
    Task<Role> UpdateRoleAsync([Body] UpdateRoleCommand command);

    [Delete("/roles/{id}")]
    Task DeleteRoleAsync(int id);
}
