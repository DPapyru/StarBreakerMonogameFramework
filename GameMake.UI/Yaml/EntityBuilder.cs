using System.Reflection;
using GameMake.UI.Core;
using GameMake.UI.Core.Components;
using Microsoft.Xna.Framework;
namespace GameMake.UI.Yaml;
public static class EntityBuilder
{
    static readonly Dictionary<string, Type> _types = new()
    {
        ["RectTransform"] = typeof(RectTransform), ["TextRenderer"] = typeof(TextRenderer),
        ["ButtonRenderer"] = typeof(ButtonRenderer), ["PanelRenderer"] = typeof(PanelRenderer),
        ["SpriteRenderer"] = typeof(SpriteRenderer), ["ImageRenderer"] = typeof(ImageRenderer),
        ["ProgressRenderer"] = typeof(ProgressRenderer), ["ScriptComponent"] = typeof(ScriptComponent),
        ["DialogueRenderer"] = typeof(DialogueRenderer), ["AvatarSprite"] = typeof(AvatarSprite),
    };
    public static UIEntity Build(List<YamlEntity> yamlEntities)
    {
        var root = new UIEntity { Id = "__root__" };
        root.Add(new RectTransform { Anchor = Anchor.Stretch });
        foreach (var ye in yamlEntities) root.AddChild(BuildOne(ye));
        return root;
    }
    static UIEntity BuildOne(YamlEntity ye)
    {
        var e = new UIEntity { Id = ye.Id ?? "", Scene = ye.Scene, IsModal = ye.Modal ?? false };
        foreach (var yc in ye.Components)
            if (_types.TryGetValue(yc.Type, out var t))
            {
                var comp = (UIComponent)Activator.CreateInstance(t);
                ApplyProps(comp, yc.Properties);
                e.Add(comp);
            }
        foreach (var c in ye.Children) e.AddChild(BuildOne(c));
        return e;
    }
    static void ApplyProps(UIComponent target, Dictionary<string, object> props)
    {
        var t = target.GetType();
        foreach (var (k, v) in props)
        {
            var p = t.GetProperty(k, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p?.CanWrite == true) try { p.SetValue(target, ConvertVal(v, p.PropertyType)); } catch { }
        }
    }
    static object ConvertVal(object v, Type t)
    {
        if (t == typeof(Vector2) && v is List<object> l && l.Count == 2) return new Vector2(Convert.ToSingle(l[0]), Convert.ToSingle(l[1]));
        if (t == typeof(Color) && v is List<object> cl && cl.Count == 4) return new Color(
            (byte)(Convert.ToSingle(cl[0]) * 255f),
            (byte)(Convert.ToSingle(cl[1]) * 255f),
            (byte)(Convert.ToSingle(cl[2]) * 255f),
            (byte)(Convert.ToSingle(cl[3]) * 255f));
        if (t == typeof(int) || t == typeof(float)) return Convert.ChangeType(v, t);
        if (t.IsEnum) return Enum.Parse(t, v?.ToString(), ignoreCase: true);
        return v?.ToString();
    }
}
