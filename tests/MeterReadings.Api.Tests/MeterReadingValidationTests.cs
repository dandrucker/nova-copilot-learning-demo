using System.Net;
using System.Net.Http.Json;
using MeterReadings.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MeterReadings.Api.Tests;

public class MeterReadingValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MeterReadingValidationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static MeterReadingRequest ValidRequest() =>
        new("MTR-1001", 125.4m, "MCF", DateTimeOffset.UtcNow.AddMinutes(-5));

    [Theory]
    [InlineData("")]
    [InlineData("MTR-THIS-ID-IS-TOO-LONG")]
    public async Task Post_InvalidMeterId_Returns400WithMeterIdError(string meterId)
    {
        var request = ValidRequest() with { MeterId = meterId };

        var errors = await PostExpectingValidationProblem(request);

        Assert.True(errors.ContainsKey(nameof(MeterReadingRequest.MeterId)));
    }

    [Fact]
    public async Task Post_NegativeVolume_Returns400WithVolumeError()
    {
        var request = ValidRequest() with { Volume = -1m };

        var errors = await PostExpectingValidationProblem(request);

        Assert.True(errors.ContainsKey(nameof(MeterReadingRequest.Volume)));
    }

    [Fact]
    public async Task Post_ZeroVolume_Returns201()
    {
        var request = ValidRequest() with { Volume = 0m };

        var response = await _client.PostAsJsonAsync("/api/meter-readings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [InlineData("gallons")]
    [InlineData("")]
    public async Task Post_UnknownUnit_Returns400WithUnitError(string unit)
    {
        var request = ValidRequest() with { Unit = unit };

        var errors = await PostExpectingValidationProblem(request);

        Assert.True(errors.ContainsKey(nameof(MeterReadingRequest.Unit)));
    }

    [Fact]
    public async Task Post_FutureReadAt_Returns400WithReadAtError()
    {
        var request = ValidRequest() with { ReadAt = DateTimeOffset.UtcNow.AddDays(1) };

        var errors = await PostExpectingValidationProblem(request);

        Assert.True(errors.ContainsKey(nameof(MeterReadingRequest.ReadAt)));
    }

    [Fact]
    public async Task Post_SeveralBadFields_ListsEachError()
    {
        var request = new MeterReadingRequest("", -50m, "gallons", DateTimeOffset.UtcNow.AddYears(1));

        var errors = await PostExpectingValidationProblem(request);

        Assert.Equal(4, errors.Count);
    }

    private async Task<IDictionary<string, string[]>> PostExpectingValidationProblem(MeterReadingRequest request)
    {
        var response = await _client.PostAsJsonAsync("/api/meter-readings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        return problem.Errors;
    }
}
