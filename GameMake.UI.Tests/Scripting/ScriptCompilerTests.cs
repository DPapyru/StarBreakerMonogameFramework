using GameMake.UI.Scripting;
namespace GameMake.UI.Tests.Scripting;
public class ScriptCompilerTests
{
    static string Src(string body) => "using GameMake.UI;\npublic class T {\n" + body + "\n}";
    [Fact] public void 编译绑定OnCreate() { Assert.NotNull(ScriptCompiler.CompileSource("T", Src("public static object OnCreate(UIEntity s) => null;")).OnCreate); }
    [Fact] public void 编译绑定OnUpdate() { Assert.NotNull(ScriptCompiler.CompileSource("T", Src("public static void OnUpdate(UIEntity s, object x, float d) {}")).OnUpdate); }
    [Fact] public void 编译绑定OnClick() { Assert.NotNull(ScriptCompiler.CompileSource("T", Src("public static void OnClick(UIEntity s, object x) {}")).OnClick); }
    [Fact] public void OnCreate返回Context() { var c = ScriptCompiler.CompileSource("T", Src("public class Ctx { public int V; } public static Ctx OnCreate(UIEntity s) => new Ctx { V = 42 };")); Assert.Equal(42, (int)c.OnCreate(new UIEntity()).GetType().GetField("V").GetValue(c.OnCreate(new UIEntity()))); }
    [Fact] public void 编译绑定OnHoverStart() { Assert.NotNull(ScriptCompiler.CompileSource("T", Src("public static void OnHoverStart(UIEntity s, object x) {}")).OnHoverStart); }
    [Fact] public void 编译绑定OnHoverEnd() { Assert.NotNull(ScriptCompiler.CompileSource("T", Src("public static void OnHoverEnd(UIEntity s, object x) {}")).OnHoverEnd); }
    [Fact] public void 编译绑定OnDestroy() { Assert.NotNull(ScriptCompiler.CompileSource("T", Src("public static void OnDestroy(UIEntity s, object x) {}")).OnDestroy); }
    [Fact] public void 重新编译返回新实例() { var c1 = ScriptCompiler.CompileSource("T", Src("public static void OnClick(UIEntity s, object x) {}")); var c2 = ScriptCompiler.CompileSource("T", Src("public static int OnCreate(UIEntity s) => 42;")); Assert.NotSame(c1, c2); Assert.NotNull(c1.OnClick); Assert.NotNull(c2.OnCreate); }
    [Fact] public void 编译无效源码抛出异常() { Assert.Throws<InvalidOperationException>(() => ScriptCompiler.CompileSource("Bad", "invalid c# code!!!")); }
}
