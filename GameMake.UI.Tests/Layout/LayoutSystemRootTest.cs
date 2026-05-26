using GameMake.UI.Core.Components;
using GameMake.UI.Rendering;
using Microsoft.Xna.Framework;

namespace GameMake.UI.Tests.Layout;

public class LayoutSystemRootTest
{
    [Fact]
    public void 根节点无RectTransform时_布局不计算()
    {
        // Simulates UISystem's __root__ (has NO RectTransform)
        var root = new UIEntity { Id = "__root__" };
        var child = new UIEntity { Id = "MainMenu" };
        child.Add(new RectTransform { Anchor = Anchor.Stretch });
        root.AddChild(child);

        // This is how UISystem calls it
        LayoutSystem.CalculateBounds(root);

        var rt = child.Get<RectTransform>();
        // If layout worked, bounds would be (0, 0, 800, 600) for Stretch
        // If layout didn't run because root has no RT, bounds stay default (0, 0, 0, 0)
        Assert.Equal(0, rt.Bounds.Width);
        Assert.Equal(0, rt.Bounds.Height);
    }

    [Fact]
    public void 根节点有RectTransform时_布局正确传播()
    {
        var root = new UIEntity { Id = "__root__" };
        root.Add(new RectTransform { Anchor = Anchor.Stretch });
        var child = new UIEntity { Id = "MainMenu" };
        child.Add(new RectTransform { Anchor = Anchor.Stretch });
        root.AddChild(child);

        LayoutSystem.CalculateBounds(root);

        var rt = child.Get<RectTransform>();
        Assert.Equal(800, rt.Bounds.Width);
        Assert.Equal(600, rt.Bounds.Height);
    }
}
