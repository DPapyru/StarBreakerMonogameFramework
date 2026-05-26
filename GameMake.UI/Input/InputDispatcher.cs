using GameMake.UI.Core.Components;
namespace GameMake.UI.Input;
public static class InputDispatcher
{
    public static UIEntity HitTest(UIEntity root, int x, int y)
    {
        var all = new List<(UIEntity E, int Z)>();
        Flatten(root, 0, all);
        foreach (var (e, _) in all.Where(e => { var rt = e.E.Get<RectTransform>(); return rt != null && rt.Visible && rt.Bounds.Contains(x, y); }).OrderByDescending(e => e.Z))
        {
            var sc = e.Get<ScriptComponent>();
            if (sc?.Script?.OnClick != null) return e;
            if (e.IsModal) return e;
        }
        return null;
    }
    static void Flatten(UIEntity e, int pz, List<(UIEntity, int)> list)
    {
        var z = pz + (e.Get<RectTransform>()?.ZOrder ?? 0);
        list.Add((e, z));
        foreach (var c in e.Children) Flatten(c, z, list);
    }
}
