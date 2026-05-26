using GameMake.UI;
using GameMake.UI.Core.Components;
using GameMake.UI.HotReload;
using GameMake.UI.Scripting;

namespace GameMake.UI.Tests.HotReload;

public class HotReloadL1Tests
{
    static string Src(string body) => "using GameMake.UI;\npublic class T {\n" + body + "\n}";

    [Fact]
    public void ReplaceScripts_匹配路径的实体脚本被替换()
    {
        var oldScript = ScriptCompiler.CompileSource("Old", Src("public static void OnClick(UIEntity s, object x) { }"));
        var newScript = ScriptCompiler.CompileSource("New", Src("public static int OnCreate(UIEntity s) => 42;"));

        var root = new UIEntity { Id = "root" };
        var child = new UIEntity { Id = "child" };
        var sc = new ScriptComponent { Path = "Scripts/MyScript.cs", Script = oldScript };
        child.Add(sc);
        root.AddChild(child);

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var changedPath = Path.Combine(baseDir, "Scripts/MyScript.cs");
        HotReloadL1.ReplaceScripts(root, baseDir, changedPath, newScript);

        Assert.Same(newScript, sc.Script);
    }

    [Fact]
    public void ReplaceScripts_不匹配路径的实体不受影响()
    {
        var oldScript = ScriptCompiler.CompileSource("Old", Src("public static void OnClick(UIEntity s, object x) { }"));
        var newScript = ScriptCompiler.CompileSource("New", Src("public static int OnCreate(UIEntity s) => 42;"));

        var root = new UIEntity { Id = "root" };
        var sc = new ScriptComponent { Path = "Scripts/Other.cs", Script = oldScript };
        root.Add(sc);

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var changedPath = Path.Combine(baseDir, "Scripts/MyScript.cs");
        HotReloadL1.ReplaceScripts(root, baseDir, changedPath, newScript);

        Assert.Same(oldScript, sc.Script);
    }

    [Fact]
    public void ReplaceScripts_触发OnDestroy然后OnCreate()
    {
        var oldDestroyed = false;
        var oldScript = new CompiledScript { OnDestroy = (_, _) => oldDestroyed = true };
        object? newContext = null;
        var newScript = new CompiledScript { OnCreate = e => { newContext = new object(); return newContext; } };

        var root = new UIEntity { Id = "root" };
        var sc = new ScriptComponent { Path = "Scripts/MyScript.cs", Script = oldScript, Owner = root, Context = new object() };
        root.Add(sc);

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var changedPath = Path.Combine(baseDir, "Scripts/MyScript.cs");
        HotReloadL1.ReplaceScripts(root, baseDir, changedPath, newScript);

        Assert.True(oldDestroyed);
        Assert.Same(newScript, sc.Script);
        Assert.Same(newContext, sc.Context);
    }

    [Fact]
    public void ReplaceScripts_遍历整个树匹配所有实例()
    {
        var newScript = ScriptCompiler.CompileSource("New", Src("public static int OnCreate(UIEntity s) => 99;"));
        var oldScript = ScriptCompiler.CompileSource("Old", Src("public static void OnClick(UIEntity s, object x) { }"));

        var root = new UIEntity { Id = "root" };
        var child1 = new UIEntity { Id = "c1" };
        var child2 = new UIEntity { Id = "c2" };
        var sc1 = new ScriptComponent { Path = "Scripts/Target.cs", Script = oldScript };
        var sc2 = new ScriptComponent { Path = "Scripts/Target.cs", Script = oldScript };
        child1.Add(sc1);
        child2.Add(sc2);
        root.AddChild(child1);
        root.AddChild(child2);

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var changedPath = Path.Combine(baseDir, "Scripts/Target.cs");
        HotReloadL1.ReplaceScripts(root, baseDir, changedPath, newScript);

        Assert.Same(newScript, sc1.Script);
        Assert.Same(newScript, sc2.Script);
    }
}

public class HotReloadL2Tests
{
    [Fact]
    public void DestroyTree_触发所有脚本的OnDestroy()
    {
        var destroyed = new List<string>();
        var root = new UIEntity { Id = "root" };
        var c1 = new UIEntity { Id = "c1" };
        var c2 = new UIEntity { Id = "c2" };
        var sc1 = new ScriptComponent { Script = new CompiledScript { OnDestroy = (_, _) => destroyed.Add("c1") } };
        var sc2 = new ScriptComponent { Script = new CompiledScript { OnDestroy = (_, _) => destroyed.Add("c2") } };
        c1.Add(sc1);
        c2.Add(sc2);
        root.AddChild(c1);
        root.AddChild(c2);

        UISystem.DestroyTree(root);

        Assert.Equal(2, destroyed.Count);
        Assert.Contains("c1", destroyed);
        Assert.Contains("c2", destroyed);
    }

    [Fact]
    public void DestroyTree_清除所有子节点()
    {
        var root = new UIEntity { Id = "root" };
        root.AddChild(new UIEntity { Id = "c1" });
        root.AddChild(new UIEntity { Id = "c2" });

        UISystem.DestroyTree(root);

        Assert.Empty(root.Children);
    }

    [Fact]
    public void DestroyTree_空树不抛异常()
    {
        var root = new UIEntity { Id = "root" };
        UISystem.DestroyTree(root);
    }

    [Fact]
    public void DestroyTree_无脚本实体不抛异常()
    {
        var root = new UIEntity { Id = "root" };
        root.AddChild(new UIEntity { Id = "c1" });

        UISystem.DestroyTree(root);

        Assert.Empty(root.Children);
    }
}
