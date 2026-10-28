using MeterReadings.Api.Models;
using MeterReadings.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IMeterReadingRepository, InMemoryMeterReadingRepository>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => Results.Ok(new { service = "MeterReadings.Api", status = "ok" }));

var readings = app.MapGroup("/api/meter-readings");

// Records a new meter reading.
// NOTE FOR THE DEMO: there is no input validation yet. Negative volumes, blank meter IDs,
// unknown units and future dates are all accepted. Work item: see WORK-ITEM.md.
readings.MapPost("/", (MeterReadingRequest request, IMeterReadingRepository repository) =>
{
    var reading = repository.Add(request);
    return Results.Created($"/api/meter-readings/{reading.Id}", reading);
});

readings.MapGet("/{id:guid}", (Guid id, IMeterReadingRepository repository) =>
    repository.GetById(id) is { } reading ? Results.Ok(reading) : Results.NotFound());

readings.MapGet("/", (string? meterId, IMeterReadingRepository repository) =>
    Results.Ok(repository.GetAll(meterId)));

app.Run();

// Lets the test project start the API in memory with WebApplicationFactory.
public partial class Program { }
