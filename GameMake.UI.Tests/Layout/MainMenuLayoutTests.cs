using GameMake.UI.Core.Components;
using GameMake.UI.Rendering;
using Microsoft.Xna.Framework;

namespace GameMake.UI.Tests.Layout;

public class MainMenuLayoutTests
{
    /// <summary>
    /// Reproduce exact MainMenu layout from main_menu.yaml.
    /// Verify TitleText ends up at the correct screen position.
    /// </summary>
    [Fact]
    public void TitleText_布局位置正确()
    {
        // Root: Anchor=Stretch, full screen 800x600
        var root = new UIEntity { Id = "MainMenu" };
        root.Add(new RectTransform { Anchor = Anchor.Stretch });

        // TitleText: Anchor=TopCenter, Position=[0,130], Size=[400,50]
        var title = new UIEntity { Id = "TitleText" };
        title.Add(new TextRenderer { Text = "碎筑成序", FontPath = "Microsoft YaHei", FontSize = 36 });
        title.Add(new RectTransform { Anchor = Anchor.TopCenter, Position = new Vector2(0, 130), Size = new Vector2(400, 50) });
        root.AddChild(title);

        // Calculate layout
        LayoutSystem.CalculateBounds(root);

        var rt = title.Get<RectTransform>();
        Assert.NotNull(rt);

        // Anchor=TopCenter within Stretch parent (800x600):
        //   X = parentCenter - myWidth/2 + posX = 400 - 200 + 0 = 200
        //   Y = posY = 130
        //   Width = 400, Height = 50
        Assert.Equal(200, rt.Bounds.X);
        Assert.Equal(130, rt.Bounds.Y);
        Assert.Equal(400, rt.Bounds.Width);
        Assert.Equal(50, rt.Bounds.Height);
    }

    [Fact]
    public void StartBtnText_居中于800x600屏幕()
    {
        var root = new UIEntity { Id = "root" };
        root.Add(new RectTransform { Anchor = Anchor.Stretch });

        // StartBtn: Anchor=Center, Position=[0,-50], Size=[200,50]
        var btn = new UIEntity { Id = "StartBtn" };
        btn.Add(new RectTransform { Anchor = Anchor.Center, Position = new Vector2(0, -50), Size = new Vector2(200, 50) });

        // StartBtnText: Anchor=Center (relative to parent), Position=[0,0], Size=[200,50]
        var text = new UIEntity { Id = "StartBtnText" };
        text.Add(new TextRenderer { Text = "开始游戏", FontPath = "Microsoft YaHei", FontSize = 22 });
        text.Add(new RectTransform { Anchor = Anchor.Center, Position = Vector2.Zero, Size = new Vector2(200, 50) });
        btn.AddChild(text);
        root.AddChild(btn);

        LayoutSystem.CalculateBounds(root);

        // StartBtn parent bounds: Center of 800x600 = (300, 250), Size=(200,50)
        //   X = 800/2 - 200/2 + 0 = 300
        //   Y = 600/2 - 50/2 + (-50) = 275 - 50 = 225
        var btnRt = btn.Get<RectTransform>();
        Assert.Equal(300, btnRt.Bounds.X);
        Assert.Equal(225, btnRt.Bounds.Y);
        Assert.Equal(200, btnRt.Bounds.Width);
        Assert.Equal(50, btnRt.Bounds.Height);

        // StartBtnText: Center of btn bounds = (300+100, 225+25) = (400, 250)
        //   Size=(200,50), so:
        //   X = 300 + (200-200)/2 + 0 = 300
        //   Y = 225 + (50-50)/2 + 0 = 225
        var textRt = text.Get<RectTransform>();
        Assert.Equal(300, textRt.Bounds.X);
        Assert.Equal(225, textRt.Bounds.Y);
    }
}
