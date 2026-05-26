using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameMake.UI.Resources;

public class ResourceManager
{
    readonly GraphicsDevice _device;
    ContentManager _content;
    readonly Dictionary<string, Texture2D> _textures = new();
    readonly Dictionary<string, SpriteFont> _fonts = new();

    public ResourceManager(GraphicsDevice device) => _device = device;

    public ContentManager Content { get => _content; set => _content = value; }

    public void PreloadTextures(string baseDir, IEnumerable<string> paths)
    {
        foreach (var p in paths)
            if (!_textures.ContainsKey(p))
            {
                var fullPath = Path.Combine(baseDir, p);
                if (File.Exists(fullPath))
                    using (var s = File.OpenRead(fullPath))
                        _textures[p] = Texture2D.FromStream(_device, s);
            }
    }

    public Texture2D GetTexture(string path) =>
        _textures.TryGetValue(path, out var t) ? t : null;

    /// <summary>
    /// Load a single font spritefont (.xnb) by key.
    /// </summary>
    public void LoadFont(string key, string assetName)
    {
        if (_content == null) return;
        _fonts[key] = _content.Load<SpriteFont>(assetName);
    }

    /// <summary>
    /// Batch-load fonts from (fontName, fontSize) pairs collected from the UI tree.
    /// Asset name convention: {fontName-no-spaces}-{fontSize}.xnb
    /// Font stored with key:  {fontName}_{fontSize}
    /// </summary>
    public void PreloadFonts(IEnumerable<(string fontName, int fontSize)> fontParams)
    {
        if (_content == null) return;
        foreach (var (fontName, fontSize) in fontParams)
        {
            var key = GetFontKey(fontName, fontSize);
            if (!_fonts.ContainsKey(key))
            {
                var assetName = SanitizeAssetName(fontName) + "-" + fontSize;
                try { _fonts[key] = _content.Load<SpriteFont>(assetName); }
                catch { }
            }
        }
    }

    /// <summary>
    /// Look up a font by family name and size (preferred).
    /// </summary>
    public SpriteFont GetFont(string fontName, int fontSize) =>
        _fonts.TryGetValue(GetFontKey(fontName, fontSize), out var f) ? f : null;

    /// <summary>
    /// Look up a font by precomposed key (backwards compatibility).
    /// </summary>
    public SpriteFont GetFont(string combinedKey) =>
        _fonts.TryGetValue(combinedKey, out var f) ? f : null;

    public void Unload()
    {
        foreach (var t in _textures.Values)
            t?.Dispose();
        _textures.Clear();
        _fonts.Clear();
    }

    static string GetFontKey(string fontName, int fontSize) =>
        $"{fontName}_{fontSize}";

    static string SanitizeAssetName(string fontName) =>
        fontName.Replace(" ", "");
}
