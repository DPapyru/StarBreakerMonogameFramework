using GameMake.UI.Core.Components;
using Microsoft.Xna.Framework;
namespace GameMake.UI.Rendering;
public static class LayoutSystem
{
    /// <summary>Fallback canvas when no parent bounds provided (used by tests).</summary>
    static readonly Rectangle DefaultCanvas = new(0, 0, 800, 600);

    public static void CalculateBounds(UIEntity entity, Rectangle? pb = null)
    {
        var rt = entity.Get<RectTransform>();
        if (rt == null) return;
        var p = pb ?? DefaultCanvas;

        // Resolve percentage-based sizing
        var w = rt.RelSize.X > 0 ? (int)(p.Width * rt.RelSize.X) : (int)rt.Size.X;
        var h = rt.RelSize.Y > 0 ? (int)(p.Height * rt.RelSize.Y) : (int)rt.Size.Y;

        rt.Bounds = rt.Anchor switch
        {
            Anchor.Stretch => p,
            Anchor.TopLeft => new Rectangle((int)rt.Position.X, (int)rt.Position.Y, w, h),
            Anchor.TopCenter => new Rectangle(p.Center.X - w / 2 + (int)rt.Position.X, (int)rt.Position.Y, w, h),
            Anchor.TopRight => new Rectangle(p.Right - w + (int)rt.Position.X, (int)rt.Position.Y, w, h),
            Anchor.CenterLeft => new Rectangle((int)rt.Position.X, p.Center.Y - h / 2 + (int)rt.Position.Y, w, h),
            Anchor.Center => new Rectangle(p.Center.X - w / 2 + (int)rt.Position.X, p.Center.Y - h / 2 + (int)rt.Position.Y, w, h),
            Anchor.CenterRight => new Rectangle(p.Right - w + (int)rt.Position.X, p.Center.Y - h / 2 + (int)rt.Position.Y, w, h),
            Anchor.BottomLeft => new Rectangle((int)rt.Position.X, p.Bottom - h + (int)rt.Position.Y, w, h),
            Anchor.BottomCenter => new Rectangle(p.Center.X - w / 2 + (int)rt.Position.X, p.Bottom - h + (int)rt.Position.Y, w, h),
            Anchor.BottomRight => new Rectangle(p.Right - w + (int)rt.Position.X, p.Bottom - h + (int)rt.Position.Y, w, h),
            _ => new Rectangle((int)rt.Position.X, (int)rt.Position.Y, w, h),
        };
        foreach (var c in entity.Children) CalculateBounds(c, rt.Bounds);
    }
}
