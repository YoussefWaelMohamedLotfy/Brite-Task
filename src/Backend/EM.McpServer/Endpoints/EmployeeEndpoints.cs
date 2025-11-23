using System.ComponentModel;
using EM.Application.Features.Employee.Commands;
using EM.Application.Features.Employee.Queries;
using ModelContextProtocol.Server;

namespace EM.McpServer.Endpoints;

[McpServerToolType]
public sealed class EmployeeEndpoints(Mediator.Mediator mediator)
{
    [McpServerTool(Name = nameof(GetAllEmployees))]
    [Description("Gets all employees in the Employee Management System")]
    public async Task<IResult> GetAllEmployees(
        [Description("Employee name filter")] string? name,
        [Description("Department ID filter")] int? departmentId,
        [Description("Is active filter")] bool? isActive,
        [Description("Date of joining from filter")] DateTimeOffset? dateOfJoiningFrom,
        [Description("Date of joining to filter")] DateTimeOffset? dateOfJoiningTo,
        CancellationToken ct
    )
    {
        var query = new GetAllEmployeesQuery(
            name,
            departmentId,
            isActive,
            dateOfJoiningFrom,
            dateOfJoiningTo
        );
        return await mediator.Send(query, ct);
    }

    [McpServerTool(Name = nameof(GetEmployeeById))]
    [Description("Gets an employee by their ID")]
    public async Task<IResult> GetEmployeeById(
        [Description("The employee ID")] Guid id,
        CancellationToken ct
    )
    {
        return await mediator.Send(new GetEmployeeByIdQuery(id), ct);
    }

    [McpServerTool(Name = nameof(CreateEmployee))]
    [Description("Creates a new employee")]
    public async Task<IResult> CreateEmployee(
        [Description("Employee name")] string name,
        [Description("Employee email")] string email,
        [Description("Employee phone")] string phone,
        [Description("Date of joining")] DateTimeOffset dateOfJoining,
        [Description("Is active")] bool isActive,
        [Description("Department ID")] int departmentId,
        [Description("Role ID")] int roleId,
        CancellationToken ct
    )
    {
        var command = new CreateEmployeeCommand(
            name,
            email,
            phone,
            dateOfJoining,
            isActive,
            departmentId,
            roleId
        );
        return await mediator.Send(command, ct);
    }

    [McpServerTool(Name = nameof(UpdateEmployee))]
    [Description("Updates an existing employee")]
    public async Task<IResult> UpdateEmployee(
        [Description("Employee ID")] Guid id,
        [Description("Employee name")] string name,
        [Description("Employee email")] string email,
        [Description("Employee phone")] string phone,
        [Description("Date of joining")] DateTimeOffset dateOfJoining,
        [Description("Is active")] bool isActive,
        [Description("Department ID")] int departmentId,
        [Description("Role ID")] int roleId,
        CancellationToken ct
    )
    {
        var command = new UpdateEmployeeCommand(
            id,
            name,
            email,
            phone,
            dateOfJoining,
            isActive,
            departmentId,
            roleId
        );
        return await mediator.Send(command, ct);
    }

    [McpServerTool(Name = nameof(DeleteEmployee))]
    [Description("Deletes an employee")]
    public async Task<IResult> DeleteEmployee(
        [Description("The employee ID")] Guid id,
        CancellationToken ct
    )
    {
        return await mediator.Send(new DeleteEmployeeCommand(id), ct);
    }
}
