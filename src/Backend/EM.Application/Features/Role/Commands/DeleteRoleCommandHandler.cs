using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

/// <summary>
/// Command to delete a role by ID.
/// </summary>
/// <param name="Id">The role ID.</param>
public sealed record DeleteRoleCommand(int Id) : ICommand<IResult>;

/// <summary>
/// Handles the deletion of a role by ID.
/// </summary>
internal sealed class DeleteRoleCommandHandler(AppDbContext dbContext)
    : ICommandHandler<DeleteRoleCommand, IResult>
{
    /// <summary>
    /// Handles the delete role command.
    /// </summary>
    /// <param name="request">The delete role command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the delete operation.</returns>
    public async Task<IResult> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);

        if (role is null)
            return Results.NotFound();

        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

/// <summary>
/// Validator for <see cref="DeleteRoleCommand"/>.
/// </summary>
internal sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteRoleCommandValidator"/> class.
    /// </summary>
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Role ID is required.");
    }
}
