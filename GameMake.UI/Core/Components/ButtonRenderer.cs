using Microsoft.Xna.Framework;
namespace GameMake.UI.Core.Components;
public enum ButtonState { Normal, Hovered, Pressed }
public class ButtonRenderer : UIComponent
{
    public string Text { get; set; }
    public Color NormalColor { get; set; } = Color.Gray;
    public Color HoverColor { get; set; } = Color.LightGray;
    public Color PressColor { get; set; } = Color.DarkGray;
    public ButtonState State { get; set; } = ButtonState.Normal;
}
