using MeterReadings.Api.Models;

namespace MeterReadings.Api.Services;

public interface IMeterReadingRepository
{
    MeterReading Add(MeterReadingRequest request);

    MeterReading? GetById(Guid id);

    IReadOnlyList<MeterReading> GetAll(string? meterId);
}
