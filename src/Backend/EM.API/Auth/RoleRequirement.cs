using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace EM.API.Auth;

public sealed record RoleRequirement(params string[] Roles) : IAuthorizationRequirement;

public sealed class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        if (context.Resource is not HttpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        IEnumerable<string> userRoles = context.User.FindAll(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
        bool allRolesMatch = requirement.Roles.Any(role => userRoles.Contains(role));

        if (!allRolesMatch)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
