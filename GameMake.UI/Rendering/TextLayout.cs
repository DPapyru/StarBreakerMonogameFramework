using Microsoft.Xna.Framework;

namespace GameMake.UI.Rendering;

public static class TextLayout
{
    public static Vector2 ComputeTextPosition(Rectangle bounds, Vector2 textSize)
    {
        var tx = bounds.X + (int)((bounds.Width - textSize.X) / 2f);
        var ty = bounds.Y + (int)((bounds.Height - textSize.Y) / 2f);
        return new Vector2(tx, ty);
    }
}
