using EM.Application.Features.Department.Commands;
using EM.Application.Features.Department.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MinimalApis.Discovery;

namespace EM.API.Endpoints;

public sealed class DepartmentEndpoints : IApi
{
    public void Register(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/departments")
            .WithTags("Departments")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetAllDepartments)
            .WithName("GetDepartments")
            .WithSummary("Retrieves a list of departments")
            .WithDescription("This endpoint returns a list of all available departments in the system.")
            .Produces<string>(StatusCodes.Status200OK)
            .CacheOutput();

        group.MapGet("/{id}", GetDepartmentById)
            .WithName("GetDepartmentById")
            .WithSummary("Retrieves a department by ID")
            .WithDescription("This endpoint returns a single department based on its ID in the system.")
            .CacheOutput();

        group.MapPost("/", CreateDepartment)
            .WithName("CreateDepartment")
            .WithSummary("Creates a new department")
            .WithDescription("This endpoint allows the creation of a new department in the system.")
            .Produces<string>(StatusCodes.Status201Created);

        group.MapPut("/", UpdateDepartment)
            .WithName("UpdateDepartment")
            .WithSummary("Updates an existing department")
            .WithDescription("This endpoint allows updating an existing department.")
            .Produces(StatusCodes.Status200OK);

        group.MapDelete("/{id}", DeleteDepartment)
            .WithName("DeleteDepartment")
            .WithSummary("Deletes a department")
            .WithDescription("This endpoint allows deleting a department.")
            .Produces(StatusCodes.Status204NoContent);
    }

    public static async Task<IResult> GetAllDepartments(IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetAllDepartmentsQuery(), ct);

    public static async Task<IResult> GetDepartmentById(int id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetDepartmentByIdQuery(id), ct);

    public static async Task<IResult> CreateDepartment([FromBody] CreateDepartmentCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> UpdateDepartment([FromBody] UpdateDepartmentCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> DeleteDepartment(int id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new DeleteDepartmentCommand(id), ct);
}
