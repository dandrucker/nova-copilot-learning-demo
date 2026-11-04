using System.Text.Json.Serialization;
using WorkOrderTracker.Api.Data;
using WorkOrderTracker.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWorkOrderRepository, InMemoryWorkOrderRepository>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapWorkOrderEndpoints();

app.Run();
