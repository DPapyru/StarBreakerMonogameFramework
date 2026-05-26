using System;
using GameMake.UI;
using Microsoft.Xna.Framework;
using GameMake.UI.Core.Components;

public class QuitButton
{
    public static object OnCreate(UIEntity self) => null;

    public static void OnClick(UIEntity self, object ctx)
    {
        Console.WriteLine("[QuitButton] 退出游戏");
        Environment.Exit(0);
    }

    public static void OnHoverStart(UIEntity self, object ctx)
    {
        // TextRenderer is on the sibling Label child of the parent Panel
        var text = self.Parent?.Find("QuitBtnText")?.Get<TextRenderer>();
        if (text != null) text.Color = Color.Yellow;
    }

    public static void OnHoverEnd(UIEntity self, object ctx)
    {
        var text = self.Parent?.Find("QuitBtnText")?.Get<TextRenderer>();
        if (text != null) text.Color = Color.White;
    }
}
