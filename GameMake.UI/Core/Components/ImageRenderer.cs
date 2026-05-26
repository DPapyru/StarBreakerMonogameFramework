using Microsoft.Xna.Framework;
namespace GameMake.UI.Core.Components;
public class ImageRenderer : UIComponent
{
    public string TexturePath { get; set; }
    public Color Tint { get; set; } = Color.White;
}
