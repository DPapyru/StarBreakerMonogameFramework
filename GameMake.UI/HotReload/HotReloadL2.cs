using GameMake.UI.Debug;

namespace GameMake.UI.HotReload;

public class HotReloadL2 : IDisposable
{
    readonly UISystem _ui;
    readonly string _baseDir;
    readonly string _yamlDir;
    readonly string _modsRoot;
    readonly FileSystemWatcher _watcher;
    CancellationTokenSource? _debounceCts;

    public HotReloadL2(UISystem ui, string baseDir, string yamlDir, string modsRoot)
    {
        _ui = ui;
        _baseDir = baseDir;
        _yamlDir = yamlDir;
        _modsRoot = modsRoot;
        _watcher = new FileSystemWatcher(yamlDir, "*.yaml");
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
        _watcher.IncludeSubdirectories = true;
        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.EnableRaisingEvents = true;
    }

    void OnChanged(object sender, FileSystemEventArgs e)
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;
        _ = Task.Delay(200, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested)
                Reload();
        }, token);
    }

    void Reload()
    {
        try
        {
            _ui.RebuildUI(_yamlDir, _modsRoot);
            UIDebug.Log("HotReload", "HotReload: layout reloaded");
        }
        catch (Exception ex)
        {
            UIDebug.Log("HotReload", $"HotReload: layout error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
    }
}
