using Microsoft.Xna.Framework;
namespace GameMake.UI.Core.Components;
public class TextRenderer : UIComponent
{
    public string Text { get; set; }
    public string FontPath { get; set; }
    public int FontSize { get; set; } = 14;
    public Color Color { get; set; } = Color.White;
}
