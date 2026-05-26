using GameMake.UI;
using GameMake.UI.Core.Components;

namespace GameMake.UI.Tests.Core;

public class UIEntityTests
{
    class TestComp : UIComponent { }
    class OtherComp : UIComponent { }

    [Fact] public void Add_存储组件() { var e = new UIEntity { Id = "t" }; e.Add(new TestComp()); Assert.NotNull(e.Get<TestComp>()); }
    [Fact] public void Get_不存在时返回Null() { Assert.Null(new UIEntity { Id = "t" }.Get<TestComp>()); }
    [Fact] public void Remove_只移除目标类型() { var e = new UIEntity { Id = "t" }; e.Add(new TestComp()); e.Add(new OtherComp()); e.Remove<TestComp>(); Assert.Null(e.Get<TestComp>()); Assert.NotNull(e.Get<OtherComp>()); }
    [Fact] public void Find_返回自身() { var e = new UIEntity { Id = "r" }; Assert.Same(e, e.Find("r")); }
    [Fact] public void Find_返回子节点() { var p = new UIEntity { Id = "p" }; p.AddChild(new UIEntity { Id = "c" }); Assert.NotNull(p.Find("c")); }
    [Fact] public void AddChild_设置Parent() { var p = new UIEntity { Id = "p" }; var c = new UIEntity { Id = "c" }; p.AddChild(c); Assert.Same(p, c.Parent); Assert.Contains(c, p.Children); }
    [Fact] public void Destroy_从父节点移除() { var p = new UIEntity { Id = "p" }; var c = new UIEntity { Id = "c" }; p.AddChild(c); c.Destroy(); Assert.DoesNotContain(c, p.Children); }
    [Fact] public void Scene_默认值为空() { Assert.Null(new UIEntity { Id = "t" }.Scene); }
    [Fact] public void Scene_可设置() { Assert.Equal("MainMenu", new UIEntity { Id = "t", Scene = "MainMenu" }.Scene); }

    [Fact]
    public void Destroy_触发OnDestroy()
    {
        var invoked = false;
        var e = new UIEntity { Id = "t" };
        var sc = new ScriptComponent { Script = new GameMake.UI.Scripting.CompiledScript { OnDestroy = (_, _) => invoked = true } };
        e.Add(sc);
        e.Destroy();
        Assert.True(invoked);
    }
}
