using System.Text.Json;

namespace InspectionLog;

/// <summary>Loads and saves inspection entries as a JSON file.</summary>
public class EntryStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _path;

    public EntryStore(string path)
    {
        _path = path;
    }

    public List<InspectionEntry> Load()
    {
        if (!File.Exists(_path))
        {
            return new List<InspectionEntry>();
        }

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<List<InspectionEntry>>(json, Options)
            ?? new List<InspectionEntry>();
    }

    public void Save(List<InspectionEntry> entries)
    {
        var json = JsonSerializer.Serialize(entries, Options);
        File.WriteAllText(_path, json);
    }
}
