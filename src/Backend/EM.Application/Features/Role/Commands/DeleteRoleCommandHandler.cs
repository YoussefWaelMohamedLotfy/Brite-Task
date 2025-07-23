using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

public sealed record DeleteRoleCommand(int Id) : ICommand<IResult>;

internal sealed class DeleteRoleCommandHandler(
    AppDbContext dbContext)
    : ICommandHandler<DeleteRoleCommand, IResult>
{
    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);

        if (role is null)
            return Results.NotFound();

        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

internal sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Role ID is required.");
    }
}
