using FluentValidation;
using MeterReadings.Api.Models;
using MeterReadings.Api.Services;
using MeterReadings.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IMeterReadingRepository, InMemoryMeterReadingRepository>();
builder.Services.AddSingleton<IValidator<MeterReadingRequest>, MeterReadingRequestValidator>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => Results.Ok(new { service = "MeterReadings.Api", status = "ok" }));

var readings = app.MapGroup("/api/meter-readings");

// Records a new meter reading after validating it.
readings.MapPost("/", async (
    MeterReadingRequest request,
    IValidator<MeterReadingRequest> validator,
    IMeterReadingRepository repository) =>
{
    var validation = await validator.ValidateAsync(request);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.ToDictionary());
    }

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
