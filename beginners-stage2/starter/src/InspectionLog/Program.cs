using InspectionLog;

// Entries live in entries.json in the folder you run from (the starter folder).
var store = new EntryStore("entries.json");
var command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (command)
{
    case "list":
        var entries = store.Load();
        if (entries.Count == 0)
        {
            Console.WriteLine("No entries yet.");
            break;
        }

        foreach (var e in entries)
        {
            Console.WriteLine($"{e.Id,3}  {e.InspectionDate:yyyy-MM-dd}  {e.Asset,-20} {e.Inspector,-10} {e.Notes}");
        }

        break;

    case "add":
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: add <asset> <inspector> <notes>");
            return 1;
        }

        var all = store.Load();
        var nextId = all.Count == 0 ? 1 : all.Max(x => x.Id) + 1;
        var entry = new InspectionEntry(
            nextId,
            args[1],
            DateOnly.FromDateTime(DateTime.Today),
            args[2],
            string.Join(' ', args[3..]));
        all.Add(entry);
        store.Save(all);
        Console.WriteLine($"Added entry {nextId}.");
        break;

    default:
        Console.WriteLine("Inspection Log");
        Console.WriteLine("  list                               Show all entries");
        Console.WriteLine("  add <asset> <inspector> <notes>    Add an entry dated today");
        break;
}

return 0;
