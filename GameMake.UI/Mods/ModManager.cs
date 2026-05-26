namespace GameMake.UI.Mods;
public static class ModManager
{
    public record ModInfo(string Name, string YamlDir, string ScriptsDir);
    public static List<ModInfo> DiscoverMods(string root)
    {
        if (!Directory.Exists(root)) return new();
        return Directory.GetDirectories(root).Select(d => { var n = Path.GetFileName(d); return new ModInfo(n, Path.Combine(d, "UILayout"), Path.Combine(d, "UIScripts")); }).ToList();
    }
}
