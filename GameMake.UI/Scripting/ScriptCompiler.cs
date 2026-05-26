using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GameMake.UI.Scripting;

public class ScriptCompiler
{
    public CompiledScript Compile(string path)
    {
        var src = File.ReadAllText(path);
        return CompileSource(Path.GetFileNameWithoutExtension(path), src);
    }

    public static CompiledScript CompileSource(string name, string src)
    {
        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        using var ms = new MemoryStream();
        var compilation = CSharpCompilation.Create(name)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(refs)
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(src));

        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
            throw new InvalidOperationException(
                "编译失败: " + string.Join("; ",
                    emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));

        ms.Seek(0, SeekOrigin.Begin);
        var type = Assembly.Load(ms.ToArray()).GetExportedTypes().FirstOrDefault()
            ?? throw new InvalidOperationException("脚本中无公开类型");
        return Bind(type);
    }

    static CompiledScript Bind(Type t)
    {
        var s = new CompiledScript();
        foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            var pt = m.GetParameters().Select(p => p.ParameterType).ToArray();
            switch (m.Name)
            {
                case "OnCreate" when pt.Length == 1 && pt[0] == typeof(UIEntity):
                    s.OnCreate = e => m.Invoke(null, new[] { e }); break;
                case "OnUpdate" when pt.Length == 3 && pt[0] == typeof(UIEntity) && pt[2] == typeof(float):
                    s.OnUpdate = (e, ctx, dt) => m.Invoke(null, new[] { e, ctx, dt }); break;
                case "OnClick" when pt.Length == 2:
                    s.OnClick = (e, ctx) => m.Invoke(null, new[] { e, ctx }); break;
                case "OnHoverStart" when pt.Length == 2:
                    s.OnHoverStart = (e, ctx) => m.Invoke(null, new[] { e, ctx }); break;
                case "OnHoverEnd" when pt.Length == 2:
                    s.OnHoverEnd = (e, ctx) => m.Invoke(null, new[] { e, ctx }); break;
                case "OnDestroy" when pt.Length == 2:
                    s.OnDestroy = (e, ctx) => m.Invoke(null, new[] { e, ctx }); break;
            }
        }
        return s;
    }
}
