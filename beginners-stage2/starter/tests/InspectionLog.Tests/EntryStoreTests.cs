namespace InspectionLog.Tests;

public class EntryStoreTests
{
    [Fact]
    public void SaveThenLoad_ReturnsTheSameEntries()
    {
        var path = Path.Combine(Path.GetTempPath(), $"entries-{Guid.NewGuid()}.json");
        var store = new EntryStore(path);
        var entries = new List<InspectionEntry>
        {
            new(1, "Meter Station 12", new DateOnly(2026, 9, 14), "J. Ortiz", "Valve 3, minor corrosion"),
        };

        try
        {
            store.Save(entries);
            var loaded = store.Load();

            Assert.Equal(entries, loaded);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Load_MissingFile_ReturnsEmptyList()
    {
        var store = new EntryStore(Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid()}.json"));

        Assert.Empty(store.Load());
    }
}
