using GameMake.UI.Core.Components;
using GameMake.UI.Debug;
using GameMake.UI.Scripting;

namespace GameMake.UI.HotReload;

public class HotReloadL1 : IDisposable
{
    readonly string _scriptsDir;
    readonly UISystem _ui;
    readonly FileSystemWatcher _watcher;
    CancellationTokenSource? _debounceCts;

    public HotReloadL1(string scriptsDir, UISystem ui)
    {
        _scriptsDir = scriptsDir;
        _ui = ui;
        _watcher = new FileSystemWatcher(scriptsDir, "*.cs");
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
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
                ProcessFile(e.FullPath);
        }, token);
    }

    void ProcessFile(string fullPath)
    {
        try
        {
            var compiled = _ui.Compiler.Compile(fullPath);
            ReplaceScripts(_ui.Root, _ui.BaseDir, fullPath, compiled);
            UIDebug.Log("HotReload", $"HotReload: recompiled {Path.GetFileName(fullPath)}");
        }
        catch (Exception ex)
        {
            UIDebug.Log("HotReload", $"HotReload: compile error {Path.GetFileName(fullPath)}: {ex.Message}");
        }
    }

    public static void ReplaceScripts(UIEntity root, string baseDir, string changedFullPath, CompiledScript newScript)
    {
        changedFullPath = Path.GetFullPath(changedFullPath);
        foreach (var e in Traverse(root))
        {
            var sc = e.Get<ScriptComponent>();
            if (sc != null && !string.IsNullOrEmpty(sc.Path))
            {
                var scriptFullPath = Path.GetFullPath(Path.Combine(baseDir, sc.Path));
                if (scriptFullPath == changedFullPath)
                {
                    sc.Script?.OnDestroy?.Invoke(sc.Owner ?? e, sc.Context);
                    sc.Script = newScript;
                    sc.Context = sc.Script?.OnCreate?.Invoke(e);
                }
            }
        }
    }

    static IEnumerable<UIEntity> Traverse(UIEntity root)
    {
        yield return root;
        foreach (var c in root.Children)
            foreach (var e in Traverse(c))
                yield return e;
    }

    public void Dispose()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
    }
}
