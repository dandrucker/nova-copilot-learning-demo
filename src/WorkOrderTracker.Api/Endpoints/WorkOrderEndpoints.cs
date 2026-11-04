using WorkOrderTracker.Api.Data;
using WorkOrderTracker.Api.Models;

namespace WorkOrderTracker.Api.Endpoints;

public static class WorkOrderEndpoints
{
    public static IEndpointRouteBuilder MapWorkOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/workorders");

        group.MapGet("/", GetAllAsync);
        group.MapGet("/overdue", GetOverdueAsync);
        group.MapGet("/{id:int}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPost("/{id:int}/close", CloseAsync);

        return endpoints;
    }

    private static async Task<IResult> GetOverdueAsync(
        IWorkOrderRepository repository,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var workOrders = await repository.GetOverdueAsync(cancellationToken);
        logger.LogInformation("Returning {WorkOrderCount} overdue work orders", workOrders.Count);
        return Results.Ok(workOrders);
    }

    private static async Task<IResult> GetAllAsync(
        IWorkOrderRepository repository,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var workOrders = await repository.GetAllAsync(cancellationToken);
        logger.LogInformation("Returning {WorkOrderCount} work orders", workOrders.Count);
        return Results.Ok(workOrders);
    }

    private static async Task<IResult> GetByIdAsync(
        int id,
        IWorkOrderRepository repository,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var workOrder = await repository.GetByIdAsync(id, cancellationToken);
        if (workOrder is null)
        {
            logger.LogWarning("Work order {WorkOrderId} was not found", id);
            return Results.NotFound();
        }

        return Results.Ok(workOrder);
    }

    private static async Task<IResult> CreateAsync(
        CreateWorkOrderRequest request,
        IWorkOrderRepository repository,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors[nameof(request.Title)] = ["Title is required."];
        }

        if (string.IsNullOrWhiteSpace(request.PipelineSegment))
        {
            errors[nameof(request.PipelineSegment)] = ["PipelineSegment is required."];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var workOrder = await repository.AddAsync(request, cancellationToken);
        logger.LogInformation(
            "Created work order {WorkOrderId} with priority {WorkOrderPriority}",
            workOrder.Id,
            workOrder.Priority);
        return Results.Created($"/workorders/{workOrder.Id}", workOrder);
    }

    private static async Task<IResult> CloseAsync(
        int id,
        IWorkOrderRepository repository,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        if (!await repository.CloseAsync(id, cancellationToken))
        {
            logger.LogWarning("Could not close work order {WorkOrderId} because it was not found", id);
            return Results.NotFound();
        }

        logger.LogInformation("Closed work order {WorkOrderId}", id);
        return Results.NoContent();
    }
}
