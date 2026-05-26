using GameMake.UI.Core.Components;
using GameMake.UI.Input;
using GameMake.UI.Scripting;
using Microsoft.Xna.Framework;
namespace GameMake.UI.Tests.Input;
public class InputDispatcherTests
{
    static UIEntity E(string id, Rectangle b, int z = 0, bool v = true, bool clickable = false)
    {
        var e = new UIEntity { Id = id };
        e.Add(new RectTransform { Bounds = b, ZOrder = z, Visible = v });
        if (clickable) e.Add(new ScriptComponent { Script = new CompiledScript { OnClick = (_, _) => {} } });
        return e;
    }
    [Fact] public void 返回最上层可交互Entity() { var root = new UIEntity { Id = "r" }; root.AddChild(E("bg", new Rectangle(0, 0, 800, 600), 0)); root.AddChild(E("btn", new Rectangle(100, 100, 200, 50), 1, clickable: true)); Assert.Equal("btn", InputDispatcher.HitTest(root, 150, 120)?.Id); }
    [Fact] public void Modal阻止穿透() { var root = new UIEntity { Id = "r" }; var m = new UIEntity { Id = "modal", IsModal = true }; m.Add(new RectTransform { Bounds = new Rectangle(0, 0, 800, 600), Visible = true }); root.AddChild(m); root.AddChild(E("bg", new Rectangle(0, 0, 800, 600))); Assert.Equal("modal", InputDispatcher.HitTest(root, 100, 100)?.Id); }
    [Fact] public void 未命中返回Null() { var root = new UIEntity { Id = "r" }; root.AddChild(E("e", new Rectangle(0, 0, 100, 100))); Assert.Null(InputDispatcher.HitTest(root, 500, 500)); }
    [Fact] public void 不可见Entity跳过() { var root = new UIEntity { Id = "r" }; root.AddChild(E("e", new Rectangle(0, 0, 100, 100), 0, false)); Assert.Null(InputDispatcher.HitTest(root, 50, 50)); }
}
