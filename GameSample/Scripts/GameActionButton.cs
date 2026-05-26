using System;
using GameMake.UI;
using GameMake.UI.Core;
using GameMake.UI.Core.Components;

public class GameActionButton
{
    public static object OnCreate(UIEntity self) => null;

    public static void OnClick(UIEntity self, object ctx)
    {
        var root = self;
        while (root.Parent != null) root = root.Parent;

        var barFill = root.Find("BarFill");
        if (barFill == null) return;

        var prog = barFill.Get<ProgressRenderer>();
        if (prog == null) return;

        prog.Progress = Math.Min(1f, prog.Progress + 0.1f);

        var label = root.Find("ProgressLabel");
        if (label != null)
        {
            var text = label.Get<TextRenderer>();
            if (text != null) text.Text = $"建造进度：{(int)(prog.Progress * 100)}%";
        }
    }
}
