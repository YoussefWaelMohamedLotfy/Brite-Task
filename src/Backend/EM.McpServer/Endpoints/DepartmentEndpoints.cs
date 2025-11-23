using System.ComponentModel;
using EM.Application.Features.Department.Commands;
using EM.Application.Features.Department.Queries;
using Mediator;
using ModelContextProtocol.Server;

namespace EM.McpServer.Endpoints;

[McpServerToolType]
public sealed class DepartmentEndpoints(Mediator.Mediator mediator)
{
    [McpServerTool(Name = nameof(GetAllDepartments))]
    [Description("Gets all departments in the Employee Management System")]
    public async Task<IResult> GetAllDepartments(CancellationToken ct)
    {
        return await mediator.Send(new GetAllDepartmentsQuery(), ct);
    }

    [McpServerTool(Name = nameof(GetDepartmentById))]
    [Description("Gets a department by its ID")]
    public async Task<IResult> GetDepartmentById(
        [Description("The department ID")] int id,
        CancellationToken ct
    )
    {
        return await mediator.Send(new GetDepartmentByIdQuery(id), ct);
    }

    [McpServerTool(Name = nameof(CreateDepartment))]
    [Description("Creates a new department")]
    public async Task<IResult> CreateDepartment(
        [Description("Department name")] string name,
        [Description("Department description")] string? description,
        CancellationToken ct
    )
    {
        var command = new CreateDepartmentCommand(name, description);
        return await mediator.Send(command, ct);
    }

    [McpServerTool(Name = nameof(UpdateDepartment))]
    [Description("Updates an existing department")]
    public async Task<IResult> UpdateDepartment(
        [Description("Department ID")] int id,
        [Description("Department name")] string name,
        [Description("Department description")] string? description,
        CancellationToken ct
    )
    {
        var command = new UpdateDepartmentCommand(id, name, description);
        return await mediator.Send(command, ct);
    }

    [McpServerTool(Name = nameof(DeleteDepartment))]
    [Description("Deletes a department")]
    public async Task<IResult> DeleteDepartment(
        [Description("The department ID")] int id,
        CancellationToken ct
    )
    {
        return await mediator.Send(new DeleteDepartmentCommand(id), ct);
    }
}
