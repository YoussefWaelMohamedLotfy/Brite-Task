using EM.Infrastructure.Data;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

/// <summary>
/// Command to create a new role.
/// </summary>
/// <param name="Name">The name of the role.</param>
/// <param name="Permissions">The permissions assigned to the role.</param>
public sealed record CreateRoleCommand(string Name, List<string> Permissions) : ICommand<IResult>;

/// <summary>
/// Handles the creation of a new role.
/// </summary>
public sealed class CreateRoleCommandHandler(AppDbContext dbContext)
    : ICommandHandler<CreateRoleCommand, IResult>
{
    /// <summary>
    /// Handles the create role command.
    /// </summary>
    /// <param name="request">The create role command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the creation operation.</returns>
    public async ValueTask<IResult> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role = new Domain.Entities.Role
        {
            Name = request.Name,
            Permissions = request.Permissions,
        };

        await dbContext.Roles.AddAsync(role, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Created($"/Roles/{role.ID}", role);
    }
}

/// <summary>
/// Validator for <see cref="CreateRoleCommand"/>.
/// </summary>
public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateRoleCommandValidator"/> class.
    /// </summary>
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required.");
        RuleFor(x => x.Permissions).NotNull().WithMessage("Permissions are required.");
    }
}
