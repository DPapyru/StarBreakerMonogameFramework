using Microsoft.Xna.Framework;
namespace GameMake.UI.Core.Components;
public class ProgressRenderer : UIComponent
{
    public float Progress { get; set; }
    public Color FillColor { get; set; } = Color.Green;
    public Color BgColor { get; set; } = Color.DarkGray;
}
