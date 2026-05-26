using System.Diagnostics;
namespace GameMake.UI.Debug;
public static class UIDebug
{
    public static bool Enabled { get; set; }

    [Conditional("DEBUG")]
    public static void Log(string cat, string msg)
    {
        if (Enabled) Console.WriteLine($"[UI:{cat}] {msg}");
    }
}
