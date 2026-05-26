using GameMake.UI.Core.Components;
using GameMake.UI.Rendering;
using Microsoft.Xna.Framework;

namespace GameMake.UI.Tests.Rendering;

public class ButtonStateUpdaterTests
{
    static UIEntity MakeButton(string id, Rectangle bounds, int z = 0)
    {
        var e = new UIEntity { Id = id };
        e.Add(new RectTransform { Bounds = bounds, ZOrder = z, Visible = true });
        e.Add(new ButtonRenderer());
        return e;
    }

    [Fact]
    public void 鼠标在按钮区域内_状态变Hovered()
    {
        var root = MakeButton("btn", new Rectangle(100, 100, 200, 50));
        ButtonStateUpdater.Update(root, new Point(150, 120));
        var btn = root.Get<ButtonRenderer>();
        Assert.Equal(ButtonState.Hovered, btn.State);
    }

    [Fact]
    public void 鼠标不在按钮区域内_状态Normal()
    {
        var root = MakeButton("btn", new Rectangle(100, 100, 200, 50));
        ButtonStateUpdater.Update(root, new Point(50, 50));
        var btn = root.Get<ButtonRenderer>();
        Assert.Equal(ButtonState.Normal, btn.State);
    }

    [Fact]
    public void 按下鼠标左键_状态变Pressed()
    {
        var root = MakeButton("btn", new Rectangle(100, 100, 200, 50));
        ButtonStateUpdater.Update(root, new Point(150, 120), leftPressed: true);
        var btn = root.Get<ButtonRenderer>();
        Assert.Equal(ButtonState.Pressed, btn.State);
    }

    [Fact]
    public void 不可见按钮不改变状态()
    {
        var root = MakeButton("btn", new Rectangle(100, 100, 200, 50));
        root.Get<RectTransform>()!.Visible = false;
        ButtonStateUpdater.Update(root, new Point(150, 120));
        var btn = root.Get<ButtonRenderer>();
        Assert.Equal(ButtonState.Normal, btn.State);
    }

    [Fact]
    public void 无ButtonRenderer的Entity跳过()
    {
        var root = new UIEntity { Id = "no-btn" };
        root.Add(new RectTransform { Bounds = new Rectangle(0, 0, 800, 600), Visible = true });
        var ex = Record.Exception(() => ButtonStateUpdater.Update(root, new Point(100, 100)));
        Assert.Null(ex);
    }
}
