namespace XenoServe.OfflineProfile;

public class NitefoxCore(NitefoxTracker tracker)
{
    private readonly NitefoxTracker _tracker = tracker;

    internal async Task<IEnumerable<(string, string)>> GetHaveAsset(string id)
    {
        await Task.Yield();
        var assets = _tracker.GetCopy();

        return assets
            .Where(x => x.Value.Contains(id, StringComparison.OrdinalIgnoreCase))
            .Select(x => (x.Key, x.Value))
            .ToList();
    }
}