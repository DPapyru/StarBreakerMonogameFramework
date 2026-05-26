using YamlDotNet.Core;
using YamlDotNet.Serialization;
namespace GameMake.UI.Yaml;
public static class YamlDeserializer
{
    public static List<YamlEntity> Deserialize(string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml)) return new();
        var deserializer = new DeserializerBuilder().Build();
        try
        {
            var raw = deserializer.Deserialize<List<Dictionary<string, object>>>(yaml);
            return raw?.Select(ParseEntity).ToList() ?? new();
        }
        catch (YamlException)
        {
            var raw = deserializer.Deserialize<Dictionary<string, object>>(yaml);
            return raw is not null ? new() { ParseEntity(raw) } : new();
        }
    }
    static YamlEntity ParseEntity(Dictionary<string, object> d)
    {
        var e = new YamlEntity();
        e.Id = d.GetValueOrDefault("Id") as string ?? "";
        if (d.TryGetValue("Modal", out var m) && bool.TryParse(m?.ToString(), out var mv)) e.Modal = mv;
        if (d.TryGetValue("Components", out var c) && c is List<object> cl)
            foreach (var i in cl.OfType<Dictionary<object, object>>()) e.Components.Add(ParseComp(i));
        if (d.TryGetValue("Children", out var ch) && ch is List<object> chl)
            foreach (var i in chl) if (ToStrDict(i) is { } sd) e.Children.Add(ParseEntity(sd));
        return e;
    }
    static YamlComponent ParseComp(Dictionary<object, object> d)
    {
        var c = new YamlComponent();
        foreach (var kv in d) { var k = kv.Key?.ToString(); if (k == "Type") c.Type = kv.Value?.ToString(); else c.Properties[k] = kv.Value; }
        return c;
    }
    static Dictionary<string, object> ToStrDict(object o) => o is Dictionary<object, object> d ? d.ToDictionary(k => k.Key?.ToString(), k => k.Value) : null;
}
