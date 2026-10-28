using System.Net;
using System.Net.Http.Json;
using MeterReadings.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MeterReadings.Api.Tests;

public class MeterReadingEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MeterReadingEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ValidReading_Returns201WithStoredReading()
    {
        var request = new MeterReadingRequest("MTR-1001", 125.4m, "MCF", DateTimeOffset.UtcNow.AddMinutes(-5));

        var response = await _client.PostAsJsonAsync("/api/meter-readings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var reading = await response.Content.ReadFromJsonAsync<MeterReading>();
        Assert.NotNull(reading);
        Assert.NotEqual(Guid.Empty, reading.Id);
        Assert.Equal("MTR-1001", reading.MeterId);
        Assert.Equal(125.4m, reading.Volume);
        Assert.Equal($"/api/meter-readings/{reading.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task GetById_AfterPost_ReturnsSameReading()
    {
        var request = new MeterReadingRequest("MTR-2002", 10m, "CCF", DateTimeOffset.UtcNow.AddHours(-1));
        var created = await (await _client.PostAsJsonAsync("/api/meter-readings", request))
            .Content.ReadFromJsonAsync<MeterReading>();

        var response = await _client.GetAsync($"/api/meter-readings/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<MeterReading>();
        Assert.Equal(created, fetched);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync($"/api/meter-readings/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_FilteredByMeter_ReturnsOnlyThatMeter()
    {
        var meterId = "MTR-" + Guid.NewGuid().ToString("N")[..8];
        await _client.PostAsJsonAsync("/api/meter-readings", new MeterReadingRequest(meterId, 1m, "MCF", DateTimeOffset.UtcNow.AddHours(-2)));
        await _client.PostAsJsonAsync("/api/meter-readings", new MeterReadingRequest(meterId, 2m, "MCF", DateTimeOffset.UtcNow.AddHours(-1)));
        await _client.PostAsJsonAsync("/api/meter-readings", new MeterReadingRequest("MTR-OTHER", 3m, "MCF", DateTimeOffset.UtcNow));

        var readings = await _client.GetFromJsonAsync<List<MeterReading>>($"/api/meter-readings?meterId={meterId}");

        Assert.NotNull(readings);
        Assert.Equal(2, readings.Count);
        Assert.All(readings, r => Assert.Equal(meterId, r.MeterId));
        Assert.Equal(2m, readings[0].Volume); // newest first
    }
}
