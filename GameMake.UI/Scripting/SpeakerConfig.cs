using Microsoft.Xna.Framework;
using YamlDotNet.Serialization;

namespace GameMake.UI.Scripting;

public class SpeakerDef
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public Color? Color { get; set; }
    public Dictionary<string, string> Expressions { get; set; } = new();
}

public static class SpeakerConfig
{
    public static Dictionary<string, SpeakerDef> Load(string yamlPath)
    {
        if (!File.Exists(yamlPath)) return new();
        var yaml = File.ReadAllText(yamlPath);
        return Parse(yaml);
    }

    public static Dictionary<string, SpeakerDef> Parse(string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml)) return new();
        var raw = new DeserializerBuilder().Build().Deserialize<Dictionary<string, object>>(yaml);
        if (raw == null || !raw.TryGetValue("speakers", out var speakersObj) || speakersObj is not Dictionary<object, object> speakers)
            return new();

        var result = new Dictionary<string, SpeakerDef>();
        foreach (var kv in speakers)
        {
            var id = kv.Key?.ToString();
            if (string.IsNullOrEmpty(id) || kv.Value is not Dictionary<object, object> data)
                continue;

            var def = new SpeakerDef { Id = id };
            foreach (var prop in data)
            {
                var key = prop.Key?.ToString()?.ToLower();
                switch (key)
                {
                    case "display_name":
                        def.DisplayName = prop.Value?.ToString();
                        break;
                    case "color":
                        if (prop.Value is List<object> cl && cl.Count >= 4)
                            def.Color = new Color(
                                (byte)(Convert.ToSingle(cl[0]) * 255f),
                                (byte)(Convert.ToSingle(cl[1]) * 255f),
                                (byte)(Convert.ToSingle(cl[2]) * 255f),
                                (byte)(Convert.ToSingle(cl[3]) * 255f));
                        break;
                    case "expressions":
                        if (prop.Value is Dictionary<object, object> exprs)
                            foreach (var expr in exprs)
                                if (expr.Key?.ToString() is { } exprName && expr.Value?.ToString() is { } exprPath)
                                    def.Expressions[exprName] = exprPath;
                        break;
                }
            }
            result[id] = def;
        }
        return result;
    }

    public static void LoadAvatars(Dictionary<string, SpeakerDef> speakers, Resources.ResourceManager res, string baseDir)
    {
        var paths = new List<string>();
        foreach (var speaker in speakers.Values)
            foreach (var expr in speaker.Expressions.Values)
                paths.Add(expr);
        res.PreloadTextures(baseDir, paths);
    }
}
