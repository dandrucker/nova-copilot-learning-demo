using System.Text.Json.Serialization;
using WorkOrderTracker.Api.Data;
using WorkOrderTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWorkOrderRepository, InMemoryWorkOrderRepository>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapGet("/workorders", (IWorkOrderRepository repository, ILogger<Program> logger) =>
{
    var workOrders = repository.GetAll().ToList();
    logger.LogInformation($"Returning {workOrders.Count} work orders");
    return Results.Ok(workOrders);
});

app.MapGet("/workorders/{id}", (int id, IWorkOrderRepository repository, ILogger<Program> logger) =>
{
    var workOrder = repository.GetById(id);
    if (workOrder == null)
    {
        logger.LogWarning($"Work order {id} was not found");
        return Results.NotFound();
    }

    return Results.Ok(workOrder);
});

app.MapPost("/workorders", (CreateWorkOrderRequest request, IWorkOrderRepository repository, ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    if (string.IsNullOrWhiteSpace(request.PipelineSegment))
    {
        return Results.BadRequest("PipelineSegment is required.");
    }

    var workOrder = repository.Add(request);
    logger.LogInformation($"Created work order {workOrder.Id} with priority {workOrder.Priority}");
    return Results.Created($"/workorders/{workOrder.Id}", workOrder);
});

app.MapPost("/workorders/{id}/close", (int id, IWorkOrderRepository repository, ILogger<Program> logger) =>
{
    if (!repository.Close(id))
    {
        logger.LogWarning($"Could not close work order {id} because it was not found");
        return Results.NotFound();
    }

    logger.LogInformation($"Closed work order {id}");
    return Results.NoContent();
});

app.Run();
