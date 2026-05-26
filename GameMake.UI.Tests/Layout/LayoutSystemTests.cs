using GameMake.UI.Core.Components;
using GameMake.UI.Rendering;
using Microsoft.Xna.Framework;
namespace GameMake.UI.Tests.Layout;
public class LayoutSystemTests
{
    static UIEntity Make(string id, Anchor a, Vector2 pos, Vector2 size) { var e = new UIEntity { Id = id }; e.Add(new RectTransform { Anchor = a, Position = pos, Size = size }); return e; }
    [Fact] public void Center锚点居中() { var root = Make("r", Anchor.TopLeft, default, new Vector2(800, 600)); root.AddChild(Make("c", Anchor.Center, default, new Vector2(100, 50))); LayoutSystem.CalculateBounds(root); var rt = root.Find("c").Get<RectTransform>(); Assert.Equal(350, rt.Bounds.X); Assert.Equal(275, rt.Bounds.Y); }
    [Fact] public void TopLeft使用Position偏移() { var root = Make("r", Anchor.TopLeft, default, new Vector2(800, 600)); root.AddChild(Make("c", Anchor.TopLeft, new Vector2(10, 20), new Vector2(100, 50))); LayoutSystem.CalculateBounds(root); var rt = root.Find("c").Get<RectTransform>(); Assert.Equal(10, rt.Bounds.X); Assert.Equal(20, rt.Bounds.Y); }
    [Fact] public void Stretch填满父容器() { var root = Make("r", Anchor.TopLeft, default, new Vector2(800, 600)); root.AddChild(Make("c", Anchor.Stretch, default, default)); LayoutSystem.CalculateBounds(root); var rt = root.Find("c").Get<RectTransform>(); Assert.Equal(0, rt.Bounds.X); Assert.Equal(0, rt.Bounds.Y); Assert.Equal(800, rt.Bounds.Width); Assert.Equal(600, rt.Bounds.Height); }
}
