using GameMake.UI.Core.Components;
using Microsoft.Xna.Framework;

namespace GameMake.UI.Rendering;

public static class ButtonStateUpdater
{
    public static void Update(UIEntity root, Point mousePos, bool leftPressed = false)
    {
        void Walk(UIEntity e)
        {
            var btn = e.Get<ButtonRenderer>();
            if (btn != null)
            {
                var rt = e.Get<RectTransform>();
                if (rt != null && rt.Visible)
                {
                    if (rt.Bounds.Contains(mousePos))
                        btn.State = leftPressed ? ButtonState.Pressed : ButtonState.Hovered;
                    else
                        btn.State = ButtonState.Normal;
                }
            }
            foreach (var c in e.Children) Walk(c);
        }
        Walk(root);
    }
}
