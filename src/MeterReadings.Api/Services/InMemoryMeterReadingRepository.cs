using System.Collections.Concurrent;
using MeterReadings.Api.Models;

namespace MeterReadings.Api.Services;

/// <summary>Keeps readings in memory. Good enough for a demo; data is lost on restart.</summary>
public class InMemoryMeterReadingRepository : IMeterReadingRepository
{
    private readonly ConcurrentDictionary<Guid, MeterReading> _readings = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryMeterReadingRepository(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public MeterReading Add(MeterReadingRequest request)
    {
        var reading = new MeterReading(
            Guid.NewGuid(),
            request.MeterId,
            request.Volume,
            request.Unit,
            request.ReadAt,
            _timeProvider.GetUtcNow());

        _readings[reading.Id] = reading;
        return reading;
    }

    public MeterReading? GetById(Guid id) =>
        _readings.TryGetValue(id, out var reading) ? reading : null;

    public IReadOnlyList<MeterReading> GetAll(string? meterId) =>
        _readings.Values
            .Where(r => meterId is null || string.Equals(r.MeterId, meterId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.ReadAt)
            .ToList();
}
