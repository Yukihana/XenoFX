namespace XenoServe.OfflineProfile;

public sealed partial class NitefoxTracker : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    public const string AssetRoot = "D:\\Desktop\\Workset"; // "J:\\TTX\\Assets";

    public NitefoxTracker()
    {
        _watcher = new FileSystemWatcher(AssetRoot)
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = true
        };

        _watcher.Created += WatcherCreated;
        _watcher.Changed += WatcherChanged;
        _watcher.Renamed += WatcherRenamed;
        _watcher.Deleted += WatcherDeleted;
        _watcher.Error += WatcherError;
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }

    // State change and Indices

    private int _stateIndex = 0;

    public int GetCurrentStateIndex()
        => _stateIndex;

    private void OnFileSystemChanged()
    {
        unchecked
        {
            _stateIndex++;
        }
        ReIndexFiles();
    }

    private void WatcherError(object sender, ErrorEventArgs e) => OnFileSystemChanged();

    private void WatcherDeleted(object sender, FileSystemEventArgs e) => OnFileSystemChanged();

    private void WatcherRenamed(object sender, RenamedEventArgs e) => OnFileSystemChanged();

    private void WatcherChanged(object sender, FileSystemEventArgs e) => OnFileSystemChanged();

    private void WatcherCreated(object sender, FileSystemEventArgs e) => OnFileSystemChanged();

    // Indexing

    private Dictionary<string, string> _files = [];

    private void ReIndexFiles()
    {
        string[] extensions = [".mp4", ".flv", ".webm"];
        var raw = Directory.GetFiles(AssetRoot, "*.*", SearchOption.AllDirectories).ToList();

        raw.RemoveAll(x => !extensions.Contains(Path.GetExtension(x), StringComparer.OrdinalIgnoreCase));
        raw.RemoveAll(x => new FileInfo(x).Length == 0);

        _files = raw.ToDictionary(x => Path.GetRelativePath(AssetRoot, x), x => Path.GetFileName(x));
    }

    public Dictionary<string, string> GetCopy() => new(_files);
}