using Microsoft.Xna.Framework;
namespace GameMake.UI.Core.Components;
public class RectTransform : UIComponent
{
    public Anchor Anchor { get; set; } = Anchor.Center;
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    /// <summary>Percentage of parent size (0-1 range). When &gt; 0, overrides Size for that axis.</summary>
    public Vector2 RelSize { get; set; }
    public int ZOrder { get; set; }
    public bool Visible { get; set; } = true;
    public Rectangle Bounds { get; set; }
}
