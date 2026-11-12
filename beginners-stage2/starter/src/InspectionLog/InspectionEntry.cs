namespace InspectionLog;

/// <summary>One field inspection note.</summary>
public record InspectionEntry(
    int Id,
    string Asset,
    DateOnly InspectionDate,
    string Inspector,
    string Notes);
