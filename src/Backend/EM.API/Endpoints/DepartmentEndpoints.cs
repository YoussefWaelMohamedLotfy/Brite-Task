using EM.Application.Features.Department.Commands;
using EM.Application.Features.Department.Queries;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Discovery;

namespace EM.API.Endpoints;

/// <summary>
/// API endpoints for managing departments.
/// </summary>
public sealed class DepartmentEndpoints : IApi
{
    /// <summary>
    /// Registers department endpoints to the route builder.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public void Register(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/departments").WithTags("Departments");

        group
            .MapGet("/", GetAllDepartments)
            .WithName("GetDepartments")
            .WithSummary("Retrieves a list of departments")
            .WithDescription(
                "This endpoint returns a list of all available departments in the system."
            )
            .Produces<IEnumerable<Domain.Entities.Department>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .CacheOutput()
            .RequireAuthorization("HR-Viewer-Admin-Policy");

        group
            .MapGet("/{id}", GetDepartmentById)
            .WithName("GetDepartmentById")
            .WithSummary("Retrieves a department by ID")
            .WithDescription(
                "This endpoint returns a single department based on its ID in the system."
            )
            .CacheOutput()
            .Produces<Domain.Entities.Department>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("HR-Viewer-Admin-Policy");

        group
            .MapPost("/", CreateDepartment)
            .WithName("CreateDepartment")
            .WithSummary("Creates a new department")
            .WithDescription("This endpoint allows the creation of a new department in the system.")
            .Produces<Domain.Entities.Department>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("HR-Admin-Policy");

        group
            .MapPut("/", UpdateDepartment)
            .WithName("UpdateDepartment")
            .WithSummary("Updates an existing department")
            .WithDescription("This endpoint allows updating an existing department.")
            .Produces<Domain.Entities.Department>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem()
            .RequireAuthorization("HR-Admin-Policy");

        group
            .MapDelete("/{id}", DeleteDepartment)
            .WithName("DeleteDepartment")
            .WithSummary("Deletes a department")
            .WithDescription("This endpoint allows deleting a department.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("HR-Admin-Policy");
    }

    /// <summary>
    /// Retrieves all departments.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A list of departments.</returns>
    public static async Task<IResult> GetAllDepartments(
        Mediator.Mediator mediator,
        CancellationToken ct
    ) => await mediator.Send(new GetAllDepartmentsQuery(), ct);

    /// <summary>
    /// Retrieves a department by its ID.
    /// </summary>
    /// <param name="id">The department ID.</param>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The department if found.</returns>
    public static async Task<IResult> GetDepartmentById(
        int id,
        Mediator.Mediator mediator,
        CancellationToken ct
    ) => await mediator.Send(new GetDepartmentByIdQuery(id), ct);

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <param name="request">The create department command.</param>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The result of the creation operation.</returns>
    public static async Task<IResult> CreateDepartment(
        [FromBody] CreateDepartmentCommand request,
        Mediator.Mediator mediator,
        CancellationToken ct
    ) => await mediator.Send(request, ct);

    public static async Task<IResult> UpdateDepartment(
        [FromBody] UpdateDepartmentCommand request,
        Mediator.Mediator mediator,
        CancellationToken ct
    ) => await mediator.Send(request, ct);

    public static async Task<IResult> DeleteDepartment(
        int id,
        Mediator.Mediator mediator,
        CancellationToken ct
    ) => await mediator.Send(new DeleteDepartmentCommand(id), ct);
}
