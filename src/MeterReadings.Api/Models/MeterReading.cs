namespace MeterReadings.Api.Models;

/// <summary>A reading submitted by a field device or technician.</summary>
public record MeterReadingRequest(string MeterId, decimal Volume, string Unit, DateTimeOffset ReadAt);

/// <summary>A reading as stored by the service.</summary>
public record MeterReading(
    Guid Id,
    string MeterId,
    decimal Volume,
    string Unit,
    DateTimeOffset ReadAt,
    DateTimeOffset RecordedAt);
