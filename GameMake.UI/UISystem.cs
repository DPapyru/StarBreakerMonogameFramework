using GameMake.UI.Core.Components;
using GameMake.UI.Debug;
using GameMake.UI.HotReload;
using GameMake.UI.Input;
using GameMake.UI.Mods;
using GameMake.UI.Rendering;
using GameMake.UI.Resources;
using GameMake.UI.Scripting;
using GameMake.UI.Yaml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameMake.UI;

public class UISystem
{
    public static UISystem Current { get; private set; }

    UIEntity _root;
    string _baseDir;
    readonly GraphicsDevice _gd;
    readonly ResourceManager _res;
    readonly ScriptCompiler _compiler = new();
    readonly RenderSystem _renderer = new();
    internal UIEntity Root => _root;
    internal string BaseDir => _baseDir;
    internal ScriptCompiler Compiler => _compiler;
    internal ResourceManager Res => _res;
    readonly DebugOverlay _debug = new();
    bool _prevF3;
    bool _prevLeft;
    int _scriptOk, _scriptFail;
    readonly Dictionary<string, Core.Components.ButtonState> _prevButtonStates = new();
    HotReloadL1 _hotReloadL1;
    HotReloadL2 _hotReloadL2;

    public UISystem(GraphicsDevice gd)
    {
        _gd = gd;
        _res = new ResourceManager(gd);
        Current = this;
    }

    public Microsoft.Xna.Framework.Content.ContentManager Content { set => _res.Content = value; }
    public void LoadFont(string key, string assetName) => _res.LoadFont(key, assetName);

    public void LoadContent(GraphicsDevice gd) => _debug.LoadContent(gd);

    public void LoadUI(string baseDir, string yamlDir, string modsRoot)
    {
        _baseDir = baseDir;
        BuildFromYaml(yamlDir, modsRoot);

        var scriptsDir = Path.Combine(baseDir, "Scripts");
        if (Directory.Exists(scriptsDir))
            _hotReloadL1 = new HotReloadL1(scriptsDir, this);

        if (Directory.Exists(yamlDir))
            _hotReloadL2 = new HotReloadL2(this, baseDir, yamlDir, modsRoot);
    }

    internal void RebuildUI(string yamlDir, string modsRoot)
    {
        DestroyTree(_root);
        _res.Unload();
        BuildFromYaml(yamlDir, modsRoot);
    }

    void BuildFromYaml(string yamlDir, string modsRoot)
    {
        var merged = new List<YamlEntity>();
        foreach (var list in LoadYamlFiles(yamlDir))
            merged = YamlMerger.Merge(merged, list);

        foreach (var mod in ModManager.DiscoverMods(modsRoot))
            if (Directory.Exists(mod.YamlDir))
                foreach (var list in LoadYamlFiles(mod.YamlDir))
                    merged = YamlMerger.Merge(merged, list);

        _root = EntityBuilder.Build(merged);

        var paths = new List<string>();
        var fontParams = new List<(string fontName, int fontSize)>();
        void Collect(UIEntity e)
        {
            foreach (var comp in e.Components)
            {
                if (comp is SpriteRenderer s && !string.IsNullOrEmpty(s.TexturePath)) paths.Add(s.TexturePath);
                if (comp is ImageRenderer i && !string.IsNullOrEmpty(i.TexturePath)) paths.Add(i.TexturePath);
                if (comp is TextRenderer t && !string.IsNullOrEmpty(t.FontPath))
                    fontParams.Add((t.FontPath, t.FontSize));
            }
            foreach (var c in e.Children) Collect(c);
        }
        Collect(_root);
        _res.PreloadTextures(_baseDir, paths);
        _res.PreloadFonts(fontParams);

        CompileAll(_root);
        var scriptStatus = $"Scripts: {_scriptOk} OK, {_scriptFail} failed";
        Console.WriteLine($"[UI] {scriptStatus}");
        UIDebug.Log("Script", scriptStatus);
        _scriptOk = _scriptFail = 0;

        UIDebug.Log("Load", $"Entities: {Count(_root)}, Textures: {paths.Count}, Fonts: {fontParams.Count}");
    }

    public void Update(float dt, MouseState ms, KeyboardState kb)
    {
        var vp = _gd.Viewport;
        LayoutSystem.CalculateBounds(_root, new Rectangle(0, 0, vp.Width, vp.Height));

        if (kb.IsKeyDown(Keys.F3) && !_prevF3) _debug.Visible = !_debug.Visible;
        _prevF3 = kb.IsKeyDown(Keys.F3);

        ButtonStateUpdater.Update(_root, new Point(ms.X, ms.Y),
            ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);

        // Fire hover events on button state transitions
        void FireHoverEvents(UIEntity e)
        {
            var btn = e.Get<ButtonRenderer>();
            if (btn != null)
            {
                var id = e.Id;
                _prevButtonStates.TryGetValue(id, out var prev);
                var cur = btn.State;
                if (prev != cur)
                {
                    var sc = e.Get<ScriptComponent>();
                    if (sc?.Script != null)
                    {
                        if (prev == Core.Components.ButtonState.Normal && cur == Core.Components.ButtonState.Hovered)
                            sc.Script.OnHoverStart?.Invoke(sc.Owner ?? e, sc.Context);
                        else if (prev == Core.Components.ButtonState.Hovered && cur == Core.Components.ButtonState.Normal)
                            sc.Script.OnHoverEnd?.Invoke(sc.Owner ?? e, sc.Context);
                    }
                    _prevButtonStates[id] = cur;
                }
            }
            foreach (var c in e.Children) FireHoverEvents(c);
        }
        FireHoverEvents(_root);

        // Execute OnUpdate scripts (sorted by Order)
        var scripts = new List<ScriptComponent>();
        void Collect(UIEntity e)
        {
            foreach (var s in e.Components.OfType<ScriptComponent>())
                if (s.Script?.OnUpdate != null) scripts.Add(s);
            foreach (var c in e.Children) Collect(c);
        }
        Collect(_root);
        scripts.Sort((a, b) => a.Order.CompareTo(b.Order));
        foreach (var s in scripts)
            s.Script.OnUpdate(s.Owner, s.Context, dt);

        // Click dispatch
        if (ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !_prevLeft)
        {
            var hit = InputDispatcher.HitTest(_root, ms.X, ms.Y);
            Console.WriteLine($"[Dbg] Click at ({ms.X},{ms.Y}), hit: {hit?.Id ?? "null"}");
            if (hit != null)
            {
                var sc = hit.Get<ScriptComponent>();
                if (sc?.Script?.OnClick != null)
                {
                    sc.Script.OnClick(sc.Owner ?? hit, sc.Context);
                    UIDebug.Log("Click", $"Entity: {hit.Id}");
                }
            }
        }
        _prevLeft = ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
    }

    public void Draw(SpriteBatch sb)
    {
        _renderer.Render(_root, sb, _res);
        _debug.Draw(sb, _root);
    }

    public UIEntity Find(string id) => _root?.Find(id);

    List<List<YamlEntity>> LoadYamlFiles(string dir)
    {
        var r = new List<List<YamlEntity>>();
        if (!Directory.Exists(dir)) return r;
        foreach (var f in Directory.GetFiles(dir, "*.yaml"))
        {
            r.Add(YamlDeserializer.Deserialize(File.ReadAllText(f)));
            UIDebug.Log("YAML", $"Loaded {f}");
        }
        return r;
    }

    void CompileAll(UIEntity e)
    {
        foreach (var sc in e.Components.OfType<ScriptComponent>())
        {
            var fullPath = !string.IsNullOrEmpty(sc.Path) ? Path.Combine(_baseDir, sc.Path) : null;
            if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                try { sc.Script = _compiler.Compile(fullPath); sc.Owner = e; sc.Context = sc.Script.OnCreate?.Invoke(e); _scriptOk++; }
                catch (Exception ex) { var msg = $"Script compile failed: {sc.Path}: {ex.Message}"; Console.Error.WriteLine(msg); UIDebug.Log("Script", msg); _scriptFail++; }
        }
        foreach (var c in e.Children) CompileAll(c);
    }

    static int Count(UIEntity e) => 1 + e.Children.Sum(Count);

    public void SwitchScene(string name) => ApplySceneVisibility(_root, name);

    internal static void ApplySceneVisibility(UIEntity root, string name)
    {
        void Walk(UIEntity e)
        {
            if (!string.IsNullOrEmpty(e.Scene))
            {
                var rt = e.Get<Core.Components.RectTransform>();
                if (rt != null) rt.Visible = e.Scene == name;
            }
            foreach (var c in e.Children) Walk(c);
        }
        Walk(root);
    }

    internal static void DestroyTree(UIEntity root)
    {
        if (root == null) return;
        foreach (var sc in root.Components.OfType<ScriptComponent>())
            sc.Script?.OnDestroy?.Invoke(sc.Owner ?? root, sc.Context);
        foreach (var c in root.Children.ToList())
            DestroyTree(c);
        root.Children.Clear();
    }

    public void Dispose()
    {
        _hotReloadL1?.Dispose();
        _hotReloadL2?.Dispose();
        _res.Unload();
    }
}
