using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

public sealed record UpdateRoleCommand(
    int Id,
    string Name,
    List<string> Permissions)
    : ICommand<IResult>;

internal sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Role ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required.");
        RuleFor(x => x.Permissions).NotNull().WithMessage("Permissions are required.");
    }
}

internal sealed class UpdateRoleCommandHandler(
    AppDbContext dbContext)
    : ICommandHandler<UpdateRoleCommand, IResult>
{
    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
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
