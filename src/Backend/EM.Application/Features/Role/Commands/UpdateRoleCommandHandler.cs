using EM.Infrastructure.Data;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

/// <summary>
/// Command to update an existing role.
/// </summary>
/// <param name="Id">The role ID.</param>
/// <param name="Name">The new name of the role.</param>
/// <param name="Permissions">The new permissions for the role.</param>
public sealed record UpdateRoleCommand(int Id, string Name, List<string> Permissions)
    : ICommand<IResult>;

/// <summary>
/// Validator for <see cref="UpdateRoleCommand"/>.
/// </summary>
public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateRoleCommandValidator"/> class.
    /// </summary>
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Role ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required.");
        RuleFor(x => x.Permissions).NotNull().WithMessage("Permissions are required.");
    }
}

/// <summary>
/// Handles the update of an existing role.
/// </summary>
public sealed class UpdateRoleCommandHandler(AppDbContext dbContext)
    : ICommandHandler<UpdateRoleCommand, IResult>
{
    /// <summary>
    /// Handles the update role command.
    /// </summary>
    /// <param name="request">The update role command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the update operation.</returns>
    public async ValueTask<IResult> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);

        if (role is null)
            return Results.NotFound();

        role.Name = request.Name;
        role.Permissions = request.Permissions;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(role);
    }
}
